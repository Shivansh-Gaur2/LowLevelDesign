# Design Pub-Sub Messaging System

A low-level design implementation of an in-memory publish-subscribe message broker in C# — similar to a simplified Apache Kafka.

## Problem Statement

Design an in-memory message broker where:
- Publishers send messages to named **topics** without knowing who will read them
- Subscribers register interest in topics and **pull** messages at their own pace
- Each subscriber independently tracks their position (offset) in each topic
- Message filtering is pluggable — subscribers can filter by keyword, priority, or receive everything
- Topics are created automatically when first published to or subscribed to

## How It Works (Plain English)

Think of it as a **bulletin board system**:
- Publishers pin messages to specific boards (topics)
- Subscribers check boards when they want to, reading from where they last left off
- Each subscriber has their own bookmark — one slow reader doesn't block anyone else
- Some subscribers only care about certain messages (filters)

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        MESSAGE BROKER                           │
│                                                                 │
│  ┌──────────────────────────────────────────────────────┐       │
│  │  Topics Dictionary                                    │       │
│  │                                                       │       │
│  │  "order-placed" ──→ Topic                             │       │
│  │                      ├── Messages: [M1, M2, M4]       │       │
│  │                      └── MessageCount: 3               │       │
│  │                                                       │       │
│  │  "payment-done" ──→ Topic                             │       │
│  │                      ├── Messages: [M3]                │       │
│  │                      └── MessageCount: 1               │       │
│  └──────────────────────────────────────────────────────┘       │
│                                                                 │
│  ┌──────────────────────────────────────────────────────┐       │
│  │  Subscribers List                                     │       │
│  │                                                       │       │
│  │  Kitchen (S1)                                         │       │
│  │    ├── Filter: NoFilter                               │       │
│  │    └── Offsets: { "order-placed": 3 }                 │       │
│  │                                                       │       │
│  │  Fraud Detector (S2)                                  │       │
│  │    ├── Filter: PriorityFilter(4)                      │       │
│  │    └── Offsets: { "payment-done": 0 }                 │       │
│  │                                                       │       │
│  │  SMS Service (S3)                                     │       │
│  │    ├── Filter: KeywordFilter("Butter Chicken")        │       │
│  │    └── Offsets: { "order-placed": 3 }                 │       │
│  └──────────────────────────────────────────────────────┘       │
└─────────────────────────────────────────────────────────────────┘
         ▲                                          ▲
         │ Publish()                                │ Poll()
         │                                          │
   ┌─────┴─────┐                            ┌──────┴──────┐
   │ Order Svc  │                            │   Kitchen   │
   │ Payment Svc│                            │   SMS Svc   │
   │ (any code) │                            │   Fraud Det │
   └────────────┘                            └─────────────┘
     PUBLISHERS                               SUBSCRIBERS
   (fire and forget)                        (pull when ready)
```

## The Offset System (Key Concept)

This is the heart of the design. Each subscriber has an independent **offset** (bookmark) per topic:

```
Topic "order-placed":

  Index:   0        1        2
  Msgs:  [ M1 ]  [ M2 ]  [ M4 ]
           ▲                 ▲
           │                 │
   SMS read up to here    Kitchen read up to here
   (offset = 1)           (offset = 3 = "read all")

   Next time SMS polls → gets [M2, M4] → offset becomes 3
   Next time Kitchen polls → gets [] (nothing new)
```

**Critical detail:** The offset advances past ALL messages, not just filtered ones. If a filter skips a message, the subscriber still moves past it — "I saw it, decided to skip it, move on." This prevents re-scanning.

## Class Diagram

```
┌──────────────────────┐
│  IMessageFilter       │ ◄── Strategy interface
│                       │
│  ShouldDeliver(msg)   │
└──────────┬───────────┘
           │ implements
     ┌─────┼──────────┐
     │     │          │
┌────┴──┐ ┌┴────────┐ ┌┴──────────────┐
│NoFilter│ │Keyword  │ │Priority       │
│        │ │Filter   │ │Filter         │
│always  │ │contains │ │priority >=    │
│true    │ │keyword? │ │threshold?     │
└────────┘ └─────────┘ └──────────────┘

┌────────────────────┐     ┌────────────────────┐
│     Message         │     │      Topic          │
│                     │     │                     │
│  Id                 │     │  Name               │
│  Topic              │────▶│  Messages           │
│  Payload            │     │  MessageCount       │
│  Priority           │     │                     │
│  Timestamp          │     │  Publish(msg)       │
└────────────────────┘     │  GetMessagesFrom()  │
                            └────────────────────┘

┌────────────────────┐     ┌────────────────────────────┐
│   Subscriber        │     │     MessageBroker           │
│                     │     │                             │
│  Id                 │     │  Topics (Dictionary)        │
│  Name               │     │  Subscribers (List)         │
│  Offsets (per topic)│     │                             │
│  Filter             │     │  GetOrCreateTopic(name)     │
│                     │     │  Subscribe(sub, topic)      │
└────────────────────┘     │  Publish(topic, msg)        │
                            │  Poll(subId, topic)         │
                            └────────────────────────────┘
```

## Poll() — Step by Step

```
broker.Poll("S3", "order-placed")     // SMS Service polls

  Step 1: Find subscriber S3 (SMS Service)

  Step 2: Find topic "order-placed"

  Step 3: Check S3's offset for "order-placed" → 0 (never polled)

  Step 4: topic.GetMessagesFrom(0) → [M1, M2, M4] (all 3)

  Step 5: Apply S3's filter: KeywordFilter("Butter Chicken")
          M1: "2x Butter Chicken" → YES ✓
          M2: "1x Masala Dosa"    → NO  ✗
          M4: "3x Paneer Tikka"   → NO  ✗
          filtered = [M1]

  Step 6: Advance offset to MessageCount (3)
          → Skipped messages won't be re-checked

  Step 7: Return [M1]
```

## Design Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **Strategy** | `IMessageFilter` | Filtering logic varies per subscriber. Swap between NoFilter, KeywordFilter, PriorityFilter without changing broker logic |
| **Mediator** | `MessageBroker` | Publishers and subscribers never talk directly. The broker sits in the middle, decoupling both sides |

## Pub-Sub vs Observer Pattern

| Aspect | Observer | Pub-Sub |
|--------|----------|---------|
| Coupling | Subject knows its observers | Publishers don't know subscribers |
| Delivery | Push (synchronous) | Pull (subscriber controls pace) |
| Persistence | No — miss it, lose it | Yes — messages stored in topic |
| Filtering | No built-in | Strategy-based filters |
| Middleman | None | Broker mediates everything |

## Usage Example

```csharp
var broker = new MessageBroker();

// Publish messages (fire and forget)
broker.Publish("order-placed", new Message("M1", "order-placed", "Order #101: 2x Butter Chicken", 3));
broker.Publish("order-placed", new Message("M2", "order-placed", "Order #102: 1x Masala Dosa", 3));

// Subscribe with different filters
var kitchen = new Subscriber("S1", "Kitchen Display");
broker.Subscribe(kitchen, "order-placed");

var sms = new Subscriber("S3", "SMS Service", new KeywordFilter("Butter Chicken"));
broker.Subscribe(sms, "order-placed");

// Kitchen gets everything
var kitchenMsgs = broker.Poll("S1", "order-placed");
// → [M1, M2]

// SMS only gets Butter Chicken orders
var smsMsgs = broker.Poll("S3", "order-placed");
// → [M1] (M2 filtered out, but offset still advances)

// Both poll again — nothing new
broker.Poll("S1", "order-placed");  // → []
broker.Poll("S3", "order-placed");  // → []
```

## Real-World Connections

- **Apache Kafka**: Pull-based, per-consumer offsets, topic partitions — this design is a simplified Kafka
- **RabbitMQ**: Push-based (opposite approach), message acknowledgment instead of offsets
- **Redis Streams**: Consumer groups with independent offsets, very similar to this model
- **AWS SQS/SNS**: SNS for pub-sub fanout, SQS for queue-based consumption

## Possible Extensions

- **Consumer groups**: Multiple subscribers sharing a topic's load (each message goes to only one member)
- **Dead letter queue**: Store messages that repeatedly fail filtering/processing
- **Message TTL**: Auto-expire old messages from topics
- **Acknowledgment**: Require explicit ack before advancing offset (at-least-once delivery)
- **Partitioning**: Split a topic into partitions for parallel consumption
- **Persistence**: Write messages to disk instead of in-memory list
