# Low-Level Design in C# / .NET

A growing collection of **design pattern implementations** and **classic LLD interview problems**, all written in C# (.NET 9). Built as a hands-on learning reference — concepts before code, real-world analogies, and clean separation of concerns.

---

## Repository Structure

```
LowLevelDesign/
├── Patterns/                       # GoF Design Pattern implementations
│   ├── AbstractFactory/
│   ├── Adapter/
│   ├── Builder/
│   ├── ChainOfResponsibility/
│   ├── Command/
│   ├── Composite/
│   ├── Decorator/
│   ├── Facade/
│   ├── FactoryMethod/
│   ├── Mediator/
│   ├── Observer/
│   ├── Prototype/
│   ├── Proxy/
│   ├── Singleton/
│   ├── State/
│   ├── Strategy/
│   └── TemplateMethod/
│
├── Questions/                      # Classic LLD interview problems
│   ├── DesignBookMyShow/           ★ NEW
│   ├── DesignElevatorSystem/
│   ├── DesignLoggingFramework/
│   ├── DesignLRUCache/
│   ├── DesignParkingLot/
│   ├── DesignPubSubMessaging/      ★ NEW
│   ├── DesignSnakeAndLadders/
│   ├── DesignTicTacToe/
│   └── DesignVendingMachine/
│
└── README.md
```

---

## Design Patterns (17)

Each pattern is a standalone .NET console project with a real-world example in `Program.cs`.

### Creational

| Pattern | Description |
|---------|-------------|
| [**Singleton**](Patterns/Singleton/) | Ensure a class has only one instance with a global access point |
| [**Factory Method**](Patterns/FactoryMethod/) | Define an interface for creating objects, let subclasses decide which class to instantiate |
| [**Abstract Factory**](Patterns/AbstractFactory/) | Create families of related objects without specifying concrete classes |
| [**Builder**](Patterns/Builder/) | Construct complex objects step by step, separating construction from representation |
| [**Prototype**](Patterns/Prototype/) | Create new objects by cloning an existing instance |

### Structural

| Pattern | Description |
|---------|-------------|
| [**Adapter**](Patterns/Adapter/) | Convert one interface into another that clients expect |
| [**Composite**](Patterns/Composite/) | Treat individual objects and compositions of objects uniformly (tree structure) |
| [**Decorator**](Patterns/Decorator/) | Attach additional responsibilities to an object dynamically |
| [**Facade**](Patterns/Facade/) | Provide a simplified interface to a complex subsystem |
| [**Proxy**](Patterns/Proxy/) | Control access to an object through a surrogate/placeholder |

### Behavioral

| Pattern | Description |
|---------|-------------|
| [**Chain of Responsibility**](Patterns/ChainOfResponsibility/) | Pass a request along a chain of handlers until one handles it |
| [**Command**](Patterns/Command/) | Encapsulate a request as an object, enabling undo/redo and queuing |
| [**Mediator**](Patterns/Mediator/) | Reduce chaotic dependencies by centralizing communication between objects |
| [**Observer**](Patterns/Observer/) | Notify multiple objects about state changes (one-to-many) |
| [**State**](Patterns/State/) | Alter an object's behavior when its internal state changes |
| [**Strategy**](Patterns/Strategy/) | Define a family of algorithms, make them interchangeable |
| [**Template Method**](Patterns/TemplateMethod/) | Define the skeleton of an algorithm, let subclasses override specific steps |

---

## LLD Interview Problems (9)

Classic problems frequently asked in software engineering interviews, implemented end-to-end.

| # | Problem | Difficulty | Key Patterns | README |
|---|---------|-----------|-------------|--------|
| 1 | [**Parking Lot**](Questions/DesignParkingLot/) | Medium | Strategy, Factory, Facade | [✓](Questions/DesignParkingLot/README.md) |
| 2 | [**LRU Cache**](Questions/DesignLRUCache/) | Medium | — (Data Structure) | — |
| 3 | [**Tic-Tac-Toe**](Questions/DesignTicTacToe/) | Easy | Strategy | — |
| 4 | [**Vending Machine**](Questions/DesignVendingMachine/) | Medium | State | — |
| 5 | [**Snake & Ladders**](Questions/DesignSnakeAndLadders/) | Medium | Strategy, Template Method | — |
| 6 | [**Elevator System**](Questions/DesignElevatorSystem/) | Hard | Strategy, State, Observer | — |
| 7 | [**Logging Framework**](Questions/DesignLoggingFramework/) | Medium | Chain of Responsibility, Singleton, Strategy | — |
| 8 | [**BookMyShow**](Questions/DesignBookMyShow/) | Advanced | Strategy, Decorator | [✓](Questions/DesignBookMyShow/README.md) |
| 9 | [**Pub-Sub Messaging**](Questions/DesignPubSubMessaging/) | Medium | Strategy, Mediator | [✓](Questions/DesignPubSubMessaging/README.md) |

### Recently Added

#### [BookMyShow — Movie Ticket Booking System](Questions/DesignBookMyShow/)
Design an online movie ticket booking platform with cities, theatres, screens, seat types, show scheduling, two-phase booking (lock → confirm/cancel), and pluggable pricing strategies with decorator-based weekend surcharges.

#### [Pub-Sub Messaging System](Questions/DesignPubSubMessaging/)
Design an in-memory message broker (simplified Kafka) with named topics, per-subscriber offsets, pull-based consumption, and strategy-based message filtering (keyword, priority, no-filter).

---

## Tech Stack

- **Language**: C# 13
- **Runtime**: .NET 9
- **IDE**: Visual Studio / VS Code
- **No external dependencies** — all implementations use only the .NET standard library

## How to Run

Each pattern and problem is a standalone .NET project. Navigate to any folder and run:

```bash
cd Patterns/Strategy
dotnet run

# or

cd Questions/DesignParkingLot
dotnet run
```

For single `.cs` file problems (BookMyShow, PubSubMessaging), the code is self-contained and can be integrated into any .NET console project.

---

## Learning Approach

1. **Understand the real-world problem first** — what entities exist, what actions happen
2. **Identify what changes** — the parts that vary are where patterns fit
3. **Code it from scratch** — don't memorize UML diagrams, build understanding
4. **Review the README** — each detailed README walks through the design step by step

---

## License

[MIT](LICENSE)