<div align="center">

# Low-Level Design in C# / .NET

**17 Design Patterns** · **9 Classic Interview Problems** · **All in C#**

A hands-on collection of design pattern implementations and classic LLD interview problems — built from scratch with real-world analogies, detailed architecture diagrams, and step-by-step walkthroughs.

[![C#](https://img.shields.io/badge/C%23-13-239120?style=flat&logo=csharp&logoColor=white)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-9-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

</div>

---

## What's Inside

This isn't a copy-paste pattern catalog. Every problem was **designed from scratch** — entity identification first, then patterns where they naturally fit, then code. Each README walks through the thought process, not just the final answer.

```
LowLevelDesign/
│
├── 📐 Patterns/                     17 GoF design pattern implementations
│   ├── Creational (5)               Singleton, Factory, Abstract Factory, Builder, Prototype
│   ├── Structural (5)               Adapter, Composite, Decorator, Facade, Proxy
│   └── Behavioral (7)               Chain of Resp, Command, Mediator, Observer, State, Strategy, Template
│
├── 💡 Questions/                    9 classic LLD interview problems
│   ├── DesignBookMyShow/            Movie ticket booking with pricing strategies
│   ├── DesignElevatorSystem/        Multi-elevator SCAN algorithm with strategy selection
│   ├── DesignLoggingFramework/      Pluggable sinks with singleton logger
│   ├── DesignLRUCache/              O(1) cache with HashMap + doubly linked list
│   ├── DesignParkingLot/            Multi-level parking with spot allocation strategies
│   ├── DesignPubSubMessaging/       In-memory message broker (simplified Kafka)
│   ├── DesignSnakeAndLadders/       Turn-based board game with clean entity model
│   ├── DesignTicTacToe/             Interactive console game with win detection
│   └── DesignVendingMachine/        Finite state machine with State pattern
│
└── README.md
```

---

## Design Patterns — 17 Implementations

Each pattern is a standalone .NET console project (`Program.cs`) with a real-world example you can run immediately.

### Creational Patterns

> *How objects are created — controlling instantiation to avoid tight coupling.*

| Pattern | One-Line Summary | Real-World Analogy |
|---------|-----------------|-------------------|
| [**Singleton**](Patterns/Singleton/) | One instance, global access | One principal per school |
| [**Factory Method**](Patterns/FactoryMethod/) | Subclass decides which object to create | A pizza shop where each branch makes different styles |
| [**Abstract Factory**](Patterns/AbstractFactory/) | Create families of related objects | IKEA furniture sets — everything matches |
| [**Builder**](Patterns/Builder/) | Construct complex objects step by step | Subway sandwich — choose bread, filling, sauce |
| [**Prototype**](Patterns/Prototype/) | Clone existing objects | Copy-paste a document, then edit the copy |

### Structural Patterns

> *How objects are composed — building larger structures from smaller pieces.*

| Pattern | One-Line Summary | Real-World Analogy |
|---------|-----------------|-------------------|
| [**Adapter**](Patterns/Adapter/) | Convert one interface to another | Travel power adapter — US plug into Indian socket |
| [**Composite**](Patterns/Composite/) | Treat individual and groups uniformly | File system — files and folders both have "size" |
| [**Decorator**](Patterns/Decorator/) | Add behavior dynamically | Adding toppings to a pizza — base stays the same |
| [**Facade**](Patterns/Facade/) | Simple interface to complex subsystem | Hotel concierge — one person handles everything |
| [**Proxy**](Patterns/Proxy/) | Control access through a surrogate | Security guard checking ID before entry |

### Behavioral Patterns

> *How objects communicate — defining interactions and responsibilities.*

| Pattern | One-Line Summary | Real-World Analogy |
|---------|-----------------|-------------------|
| [**Chain of Responsibility**](Patterns/ChainOfResponsibility/) | Pass request down a handler chain | Customer support escalation — L1 → L2 → L3 |
| [**Command**](Patterns/Command/) | Encapsulate requests as objects | Restaurant order slip — waiter carries it to kitchen |
| [**Mediator**](Patterns/Mediator/) | Centralize communication | Air traffic control — planes talk through ATC |
| [**Observer**](Patterns/Observer/) | Notify dependents of state changes | YouTube subscriptions — new video notifies all subs |
| [**State**](Patterns/State/) | Behavior changes with internal state | Vending machine — acts differently based on state |
| [**Strategy**](Patterns/Strategy/) | Swap algorithms at runtime | GPS navigation — fastest vs. shortest vs. scenic route |
| [**Template Method**](Patterns/TemplateMethod/) | Define algorithm skeleton, defer steps | Making tea vs. coffee — same process, different ingredients |

---

## LLD Interview Problems — 9 Deep Dives

Every problem includes a detailed README with architecture diagrams, data flow visualizations, pattern explanations, and usage examples.

| # | Problem | Difficulty | Patterns Used | What You'll Learn |
|---|---------|:----------:|---------------|-------------------|
| 1 | [**Parking Lot**](Questions/DesignParkingLot/) | ⭐⭐ | Strategy, Factory, Facade | Multi-level spot allocation, pluggable pricing, entry/exit flow |
| 2 | [**LRU Cache**](Questions/DesignLRUCache/) | ⭐⭐ | Data Structure | HashMap + doubly linked list for O(1) get/put with eviction |
| 3 | [**Tic-Tac-Toe**](Questions/DesignTicTacToe/) | ⭐ | Clean OOP | Board state management, win detection across 8 lines |
| 4 | [**Vending Machine**](Questions/DesignVendingMachine/) | ⭐⭐ | State | Finite state machine — Idle → HasMoney → Dispensing → Idle |
| 5 | [**Snake & Ladders**](Questions/DesignSnakeAndLadders/) | ⭐⭐ | Entity Modeling | Turn-based game loop, board mapping, overshoot rules |
| 6 | [**Elevator System**](Questions/DesignElevatorSystem/) | ⭐⭐⭐ | Strategy | SCAN algorithm, two-queue scheduling, nearest-elevator selection |
| 7 | [**Logging Framework**](Questions/DesignLoggingFramework/) | ⭐⭐ | Singleton, Observer | Multi-sink fanout, per-sink filtering, thread-safe singleton |
| 8 | [**BookMyShow**](Questions/DesignBookMyShow/) | ⭐⭐⭐ | Strategy, Decorator | Two-phase seat locking, city→theatre→screen hierarchy, weekend pricing |
| 9 | [**Pub-Sub Messaging**](Questions/DesignPubSubMessaging/) | ⭐⭐ | Strategy, Mediator | Per-subscriber offsets, pull-based consumption, keyword/priority filters |

### Quick Preview

<details>
<summary><b>BookMyShow</b> — Seat booking flow with two-phase locking</summary>

```
User selects seats → TryLockSeats (Available → Pending) → Calculate price → Create Booking
                                                                              │
                                                                    ┌────────┴────────┐
                                                                    ▼                 ▼
                                                             ConfirmBooking     CancelBooking
                                                             Seats → BOOKED    Seats → AVAILABLE
```
</details>

<details>
<summary><b>Pub-Sub Messaging</b> — Per-subscriber offset system (like Kafka)</summary>

```
Topic "order-placed":

  Index:   0        1        2
  Msgs:  [ M1 ]  [ M2 ]  [ M4 ]
           ▲                 ▲
   SMS offset = 1      Kitchen offset = 3
   (filtered M2)       (read everything)
```
</details>

<details>
<summary><b>Vending Machine</b> — State pattern transitions</summary>

```
  IDLE ──[insert money]──▶ HAS_MONEY ──[select product]──▶ DISPENSING ──▶ IDLE
    ▲                         │                                            │
    └────────[cancel]─────────┘            (auto-return)───────────────────┘
```
</details>

<details>
<summary><b>Elevator System</b> — SCAN algorithm with SortedSet queues</summary>

```
  Elevator going UP: serves all UpStops in order
  When UpStops empty: switches to DOWN, serves DownStops
  When both empty: goes IDLE

  UpStops: {4, 7, 9}   DownStops: {2}
  Floor: 3 → 4(stop) → 5 → 6 → 7(stop) → 8 → 9(stop) → reverse → ... → 2(stop) → IDLE
```
</details>

<details>
<summary><b>LRU Cache</b> — O(1) with HashMap + doubly linked list</summary>

```
  Dictionary: {"rahul" → Node, "amit" → Node, "neha" → Node}
                                │
  Linked List: HEAD ←→ [amit] ←→ [neha] ←→ [rahul] ←→ TAIL
                        ▲ evict first              ▲ most recent
```
</details>

---

## How to Run

Each pattern and problem is a standalone .NET project:

```bash
# Run a design pattern
cd Patterns/Strategy
dotnet run

# Run an LLD problem
cd Questions/DesignVendingMachine
dotnet run
```

For single `.cs` file problems (BookMyShow, PubSubMessaging), the code is self-contained — drop it into any .NET console project.

---

## How Each README is Structured

Every problem README follows the same format for consistency:

```
1. Problem Statement          — what you're designing
2. Real-World Analogy         — grounding in something familiar
3. Architecture Diagram       — ASCII art showing class relationships
4. Data/Request Flow          — step-by-step trace through the system
5. Design Patterns Used       — which patterns and WHY (not just which)
6. Key Design Decisions       — tradeoffs and reasoning
7. Usage Example              — runnable C# code snippet
8. Possible Extensions        — interview follow-up preparation
```

---

## Tech Stack

| | |
|---|---|
| **Language** | C# 13 |
| **Runtime** | .NET 9 |
| **Dependencies** | None — standard library only |
| **IDE** | Visual Studio / VS Code |

---

## Learning Approach

This repo follows a deliberate learning path:

1. **Understand the real-world problem** — what entities exist, what actions happen
2. **Identify what changes** — the parts that vary are where patterns fit naturally
3. **Don't force patterns** — if a problem doesn't need a pattern, don't add one
4. **Code it from scratch** — don't memorize UML, build understanding through implementation
5. **Review the walkthrough** — each README traces the design thinking, not just the code

---

## License

[MIT](LICENSE)