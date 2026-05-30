// PROBLEM: Pub-Sub Messaging System — Classic LLD
// Difficulty: Medium
//
// You're building an in-memory message broker for a food delivery startup
// in Koramangala, Bangalore. Different services need to communicate without
// knowing about each other — the order service shouldn't care who's listening.
//
// REQUIREMENTS:
//
// 1. Topics:
//    - Messages are published to named topics (e.g., "order-placed", "payment-done")
//    - Topics are created on-the-fly when first published to or subscribed to
//    - Each topic maintains an ordered list of messages
//
// 2. Publishers:
//    - Any publisher can publish a message to any topic
//    - A message has: Id, Topic, Payload (string), and Timestamp
//    - Publishing to a topic that doesn't exist creates it automatically
//
// 3. Subscribers:
//    - Subscribers register interest in specific topics
//    - A subscriber can subscribe to multiple topics
//    - Each subscriber has its own offset per topic (tracks what they've read)
//    - Subscribing to a non-existent topic creates it automatically
//
// 4. Message delivery:
//    - Pull-based: subscribers call Poll() to get unread messages
//    - Poll returns all messages from their current offset onward
//    - After polling, their offset advances past the returned messages
//    - If no new messages, Poll returns empty list
//
// 5. Message filtering (Strategy pattern):
//    - NoFilter: deliver all messages (default)
//    - KeywordFilter: only deliver messages whose payload contains a keyword
//    - PriorityFilter: messages can have a priority tag, only deliver above threshold
//
// ENTITIES:
//
//   class Message
//     - string Id
//     - string Topic
//     - string Payload
//     - DateTime Timestamp
//     - int Priority          (1=low, 5=critical, default=3)
//
//   class Topic
//     - string Name
//     - List<Message> Messages
//     - void Publish(Message message)   → appends to list
//     - List<Message> GetMessagesFrom(int offset) → messages from offset onward
//     - int MessageCount                → total messages so far
//
//   interface IMessageFilter
//     - bool ShouldDeliver(Message message)
//
//   class NoFilter : IMessageFilter         → always returns true
//   class KeywordFilter : IMessageFilter    → true if payload contains keyword
//   class PriorityFilter : IMessageFilter   → true if message.Priority >= threshold
//
//   class Subscriber
//     - string Id
//     - string Name
//     - Dictionary<string, int> Offsets    (topic name → current offset)
//     - IMessageFilter Filter              (default: NoFilter)
//     - List<Message> Poll(Topic topic)    → get unread messages, apply filter, advance offset
//
//   class MessageBroker
//     - Dictionary<string, Topic> Topics
//     - List<Subscriber> Subscribers
//     - void CreateTopic(string name)
//     - Topic GetOrCreateTopic(string name)
//     - void Subscribe(Subscriber subscriber, string topicName)
//     - void Publish(string topicName, Message message)
//     - List<Message> Poll(string subscriberId, string topicName)
//
// POLL() LOGIC:
//
//   1. Find the subscriber and topic
//   2. Get subscriber's current offset for that topic (default 0)
//   3. Fetch all messages from that offset onward
//   4. Apply the subscriber's filter to each message
//   5. Update the offset to topic.MessageCount (advance past ALL messages, not just filtered ones)
//   6. Return the filtered messages
//
//   WHY advance past ALL messages, not just filtered ones?
//   → Otherwise the subscriber would re-check already-seen messages every poll
//   → "I saw them, decided to skip them, move on"
//
// USAGE EXAMPLE:
//
//   var broker = new MessageBroker();
//
//   // Publishers just publish — they don't know who's listening
//   broker.Publish("order-placed", new Message("M1", "order-placed", "Order #101: 2x Butter Chicken", 3));
//   broker.Publish("order-placed", new Message("M2", "order-placed", "Order #102: 1x Masala Dosa", 3));
//   broker.Publish("payment-done", new Message("M3", "payment-done", "Payment received for #101", 4));
//
//   // Kitchen subscribes to order-placed (no filter)
//   var kitchen = new Subscriber("S1", "Kitchen Display");
//   broker.Subscribe(kitchen, "order-placed");
//
//   // Fraud detector subscribes to payment-done (priority filter: only high priority)
//   var fraud = new Subscriber("S2", "Fraud Detector", new PriorityFilter(4));
//   broker.Subscribe(fraud, "payment-done");
//
//   // SMS service subscribes to order-placed (keyword filter: only "Butter Chicken")
//   var sms = new Subscriber("S3", "SMS Service", new KeywordFilter("Butter Chicken"));
//   broker.Subscribe(sms, "order-placed");
//
//   // Kitchen polls — gets both orders
//   var kitchenMsgs = broker.Poll("S1", "order-placed");
//   // → [M1: "Order #101: 2x Butter Chicken", M2: "Order #102: 1x Masala Dosa"]
//
//   // Kitchen polls again — nothing new
//   var kitchenMsgs2 = broker.Poll("S1", "order-placed");
//   // → [] (empty — offset has advanced)
//
//   // SMS polls — only gets the Butter Chicken order (keyword filter)
//   var smsMsgs = broker.Poll("S3", "order-placed");
//   // → [M1: "Order #101: 2x Butter Chicken"]
//   // (M2 was skipped by filter, but offset still advances past it)
//
//   // New message arrives
//   broker.Publish("order-placed", new Message("M4", "order-placed", "Order #103: 3x Paneer Tikka", 3));
//
//   // Kitchen polls — only gets M4 (M1, M2 already consumed)
//   var kitchenMsgs3 = broker.Poll("S1", "order-placed");
//   // → [M4: "Order #103: 3x Paneer Tikka"]
//
// THINK ABOUT:
//   - Why pull-based (Poll) instead of push-based?
//     → Each subscriber controls their own pace. No backpressure issues.
//     → In real systems: Kafka is pull, RabbitMQ is push. Both have tradeoffs.
//   - Why does each subscriber have its own offset?
//     → Subscribers are independent. Kitchen might poll every 1s, SMS every 10s.
//     → One slow subscriber doesn't block others.
//   - Why advance offset past filtered-out messages?
//     → Prevents re-scanning. The filter said "skip it" — don't ask again.
//   - How is this different from Observer pattern?
//     → Observer: subject pushes to all observers immediately (synchronous, coupled)
//     → Pub-Sub: broker stores messages, subscribers pull when ready (async, decoupled)
//     → Pub-Sub adds: persistence (messages survive), independent consumption, filtering
//
// PATTERNS USED:
//   - Strategy: IMessageFilter (swap filtering behavior per subscriber)
//   - Observer-like: but decoupled via broker (no direct subject→observer link)
//   - Mediator: MessageBroker mediates between publishers and subscribers
//
// Write your solution below:

using System;
using System.Collections.Generic;
using System.Linq;

// --- Data ---

public class Message
{
    public string Id;
    public string Topic;
    public string Payload;
    public DateTime Timestamp;
    public int Priority;

    public Message(string id, string topic, string payload, int priority = 3)
    {
        Id = id;
        Topic = topic;
        Payload = payload;
        Priority = priority;
        Timestamp = DateTime.Now;
    }
}

public class Topic
{
    public string Name;
    public List<Message> Messages = new();
    public int MessageCount => Messages.Count;  // computed, not manual

    public Topic(string name) { Name = name; }

    public void Publish(Message message) => Messages.Add(message);

    public List<Message> GetMessagesFrom(int offset)
    {
        if (offset >= Messages.Count) return new List<Message>();
        return Messages.GetRange(offset, Messages.Count - offset);
    }
}

// --- Strategy: Filters ---

public interface IMessageFilter
{
    bool ShouldDeliver(Message message);
}

public class NoFilter : IMessageFilter
{
    public bool ShouldDeliver(Message message) => true;
}

public class KeywordFilter : IMessageFilter
{
    private string _keyword;
    public KeywordFilter(string keyword) { _keyword = keyword; }
    public bool ShouldDeliver(Message message) => message.Payload.Contains(_keyword);
}

public class PriorityFilter : IMessageFilter
{
    private int _threshold;
    public PriorityFilter(int threshold) { _threshold = threshold; }
    public bool ShouldDeliver(Message message) => message.Priority >= _threshold;
}

// --- Subscriber (DTO — no logic, just data) ---

public class Subscriber
{
    public string Id;
    public string Name;
    public Dictionary<string, int> Offsets = new();
    public IMessageFilter Filter;

    public Subscriber(string id, string name, IMessageFilter filter = null)
    {
        Id = id;
        Name = name;
        Filter = filter ?? new NoFilter();
    }
}

// --- Mediator: MessageBroker ---

public class MessageBroker
{
    private Dictionary<string, Topic> Topics = new();
    private List<Subscriber> Subscribers = new();

    public Topic GetOrCreateTopic(string name)
    {
        if (!Topics.ContainsKey(name))
            Topics[name] = new Topic(name);
        return Topics[name];
    }

    public void Subscribe(Subscriber subscriber, string topicName)
    {
        GetOrCreateTopic(topicName);
        if (!Subscribers.Contains(subscriber))
            Subscribers.Add(subscriber);
        subscriber.Offsets[topicName] = 0;
    }

    public void Publish(string topicName, Message message)
    {
        var topic = GetOrCreateTopic(topicName);
        topic.Publish(message);
    }

    public List<Message> Poll(string subscriberId, string topicName)
    {
        var sub = Subscribers.FirstOrDefault(s => s.Id == subscriberId);
        if (sub == null) throw new Exception("Subscriber not found");

        var topic = GetOrCreateTopic(topicName);
        int offset = sub.Offsets.ContainsKey(topicName) ? sub.Offsets[topicName] : 0;

        var allNew = topic.GetMessagesFrom(offset);
        var filtered = allNew.Where(m => sub.Filter.ShouldDeliver(m)).ToList();

        sub.Offsets[topicName] = topic.MessageCount;
        return filtered;
    }
}
