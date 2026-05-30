# Design Elevator System

A low-level design implementation of a multi-elevator control system in C# using the **Strategy pattern** for elevator selection.

---

## Problem Statement

Design an elevator system for a building where:
- Multiple elevators serve multiple floors
- External requests come from hallway buttons (floor + direction)
- Internal requests come from passengers pressing floor buttons inside an elevator
- The system intelligently assigns the best elevator to each request
- The selection algorithm is pluggable (nearest-first, load-balanced, etc.)

---

## Real-World Analogy

Think of a mall with 3 elevators. You press the UP button on floor 2. The system doesn't just pick any elevator — it finds the one that's:
1. Already heading UP and will pass floor 2, OR
2. Idle and closest to floor 2, OR
3. As a last resort, any elevator

That decision-making is the **Strategy pattern** in action.

---

## Architecture

```
                    ┌─────────────────────────────────┐
                    │       ElevatorController         │
                    │                                  │
                    │  HandleExternalRequest(floor, dir)│
                    │  HandleInternalRequest(elevId, fl)│
                    │  Step()  ← advances simulation   │
                    └───────────┬───────────────────────┘
                                │
                    uses ◄──────┘
                                │
               ┌────────────────┴────────────────┐
               │   IElevatorSelectionStrategy     │
               │                                  │
               │   SelectElevator(elevators, req) │
               └────────────────┬─────────────────┘
                                │ implements
                                ▼
               ┌─────────────────────────────────┐
               │    NearestElevatorStrategy       │
               │                                  │
               │  Prefers: idle or same-direction │
               │  Tiebreaker: shortest distance   │
               │  Fallback: any closest elevator  │
               └─────────────────────────────────┘

               ┌──────────────────────────────────┐
               │           Elevator                │
               │                                   │
               │  CurrentFloor                     │
               │  Direction (UP / DOWN / IDLE)     │
               │  DoorState (OPEN / CLOSED)        │
               │  UpStops (SortedSet)              │
               │  DownStops (SortedSet)            │
               │                                   │
               │  AddStop(floor)                   │
               │  Step()  ← move one floor         │
               └──────────────────────────────────┘
```

---

## How the Elevator Moves (Step-by-Step)

Each call to `Step()` simulates one time unit. Here's the SCAN algorithm in action:

```
Elevator at Floor 0, Direction: UP
UpStops: {1, 4, 7}    DownStops: {}

Step 1: Floor 0 → 1  (stop! remove 1 from UpStops, open/close door)
Step 2: Floor 1 → 2
Step 3: Floor 2 → 3
Step 4: Floor 3 → 4  (stop! remove 4 from UpStops)
Step 5: Floor 4 → 5
Step 6: Floor 5 → 6
Step 7: Floor 6 → 7  (stop! remove 7 from UpStops)
Step 8: UpStops empty, DownStops empty → IDLE
```

If a DOWN request comes in while going UP, it gets queued in `DownStops` and served after all UP stops are done — just like a real elevator (SCAN/elevator algorithm).

---

## Request Flow

```
External: "Floor 4, going UP"
        │
        ▼
┌───────────────────────┐
│  ElevatorController    │
│                        │
│  1. Create Request     │
│  2. Ask Strategy to    │──── IElevatorSelectionStrategy
│     pick best elevator │     .SelectElevator(elevators, req)
│  3. Add stop to chosen │
│     elevator           │
└───────────────────────┘

Internal: "Elevator 0, go to floor 7"
        │
        ▼
┌───────────────────────┐
│  ElevatorController    │
│                        │
│  1. Find elevator by ID│
│  2. Call AddStop(7)    │
│     directly           │
└───────────────────────┘
```

---

## Selection Strategy Logic

The `NearestElevatorStrategy` picks an elevator in two passes:

| Priority | Criteria | Why |
|----------|----------|-----|
| **1st** | Idle OR moving in same direction AND hasn't passed the floor yet | Most efficient — no direction change needed |
| **2nd** | Any closest elevator (fallback) | When no ideal match exists |

```
Scenario: Request at Floor 4 going UP

  Elevator 0: Floor 2, going UP   → distance 2  ✓ (same direction, hasn't passed)
  Elevator 1: Floor 6, going DOWN → distance 2  ✗ (wrong direction)
  Elevator 2: Floor 3, IDLE       → distance 1  ✓ (idle = always eligible)

  Winner: Elevator 2 (distance 1, idle)
```

---

## Design Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **Strategy** | `IElevatorSelectionStrategy` | Elevator selection logic can vary (nearest, least-loaded, zone-based). Swap without changing controller |

---

## Key Design Decisions

### 1. Two Separate Stop Queues (UpStops / DownStops)
Using `SortedSet<int>` for each direction implements the SCAN (elevator) algorithm naturally. The elevator serves all stops in one direction before reversing.

### 2. Step-Based Simulation
Instead of jumping to the destination, `Step()` moves one floor at a time. This allows real-time simulation, animation, and picking up passengers along the way.

### 3. Smart Stop Routing in AddStop()
When a floor is added, it goes to the right queue based on current direction. If the elevator is going UP and you request a floor below, it goes to `DownStops` (served later).

---

## Usage Example

```csharp
var controller = new ElevatorController(2, 10, new NearestElevatorStrategy());

// Someone on floor 1 presses UP
controller.HandleExternalRequest(1, Direction.UP);

// Inside elevator 0, passenger presses floor 7
controller.HandleInternalRequest(0, 7);

// Someone on floor 4 presses UP
controller.HandleExternalRequest(4, Direction.UP);

// Simulate 8 time steps
for (int i = 0; i < 8; i++)
{
    controller.Step();
    controller.Display();
}
```

---

## Possible Extensions

- **Load balancing strategy**: Pick elevator with fewest queued stops
- **Zone-based strategy**: Elevators 0-1 serve floors 0-5, elevators 2-3 serve 6-10
- **VIP/express mode**: Certain elevators skip intermediate floors
- **Weight tracking**: Don't assign if elevator is near capacity
- **Door timeout**: Auto-close after N seconds
- **Emergency mode**: All elevators return to ground floor
