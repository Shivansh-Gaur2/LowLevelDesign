# Design Vending Machine

A low-level design implementation of a vending machine in C# using the **State pattern** — the textbook use case for finite state machines in OOP.

---

## Problem Statement

Design a vending machine where:
- The machine holds multiple products with names, prices, and quantities
- Users insert money, select a product, and receive it with any change
- The machine transitions through distinct states: Idle → HasMoney → Dispensing → Idle
- Each state restricts what operations are allowed (e.g., can't select a product before inserting money)
- An operator can refill products and collect accumulated cash

---

## Real-World Analogy

Stand in front of a real vending machine. Notice the behavior changes based on state:

| You try to... | No money inserted | Money inserted | Dispensing |
|---------------|-------------------|---------------|------------|
| Insert money | ✓ Accepted | ✓ Adds more | ✗ Wait |
| Select product | ✗ "Insert money first" | ✓ If affordable | ✗ Wait |
| Press cancel | ✗ Nothing to refund | ✓ Money returned | ✗ Too late |

The machine acts differently based on what state it's in — that's the **State pattern**.

---

## State Machine

```
                    InsertMoney()
         ┌──────────────────────────────┐
         │                              ▼
    ┌─────────┐                  ┌─────────────┐
    │         │   InsertMoney()  │             │
    │  IDLE   │ ───────────────▶ │ HAS_MONEY   │◄──┐
    │         │                  │             │   │ InsertMoney()
    └─────────┘                  └──────┬──────┘───┘
         ▲                              │
         │                     SelectProduct()
         │                     (valid + affordable)
         │                              │
         │      ┌───────────────────────▼──────┐
         │      │                              │
         └──────│         DISPENSING            │
      auto-     │                              │
      return    │  Dispense product             │
      to idle   │  Calculate change             │
                │  Decrement quantity            │
                └──────────────────────────────┘
                
    ┌─────────┐  Cancel()   ┌─────────────┐
    │  IDLE   │ ◄───────────│ HAS_MONEY   │  (refund + return to idle)
    └─────────┘             └─────────────┘
```

---

## Architecture

```
┌──────────────────────────────────────────────────────────┐
│                     VendingMachine                         │
│                                                           │
│  _products: Dict<string, Product>                         │
│  _currentBalance: decimal                                 │
│  _totalCash: decimal                                      │
│  _state: IVendingMachineState  ◄── current state          │
│                                                           │
│  Public API:                                              │
│    InsertMoney(amount) → delegates to _state              │
│    SelectProduct(name) → delegates to _state              │
│    Cancel()            → delegates to _state              │
│                                                           │
│  Operator API:                                            │
│    Refill(name, qty, price)                               │
│    CollectCash() → returns total                          │
│    DisplayProducts()                                      │
│                                                           │
│  Internal (called by states):                             │
│    AddMoney(amount)                                       │
│    SetState(newState)                                     │
│    GetCurrentBalance()                                    │
│    GetProducts()                                          │
│    ResetBalance()                                         │
│    Dispense(productName) ← actual dispensing logic        │
└──────────────────────────────────────────────────────────┘
                         │
          delegates to   │
                         ▼
              ┌──────────────────────┐
              │ IVendingMachineState  │
              │                       │
              │ InsertMoney(amt, vm)  │
              │ SelectProduct(nm, vm) │
              │ Cancel(vm)            │
              └──────────┬────────────┘
                         │ implements
            ┌────────────┼───────────────┐
            ▼            ▼               ▼
     ┌────────────┐ ┌──────────┐ ┌──────────────┐
     │ IdleState   │ │HasMoney  │ │DispenseState │
     │             │ │State     │ │              │
     │ Insert: ✓   │ │Insert: ✓ │ │Insert: ✗     │
     │ Select: ✗   │ │Select: ✓ │ │Select: ✗     │
     │ Cancel: ✗   │ │Cancel: ✓ │ │Cancel: ✗     │
     └────────────┘ └──────────┘ └──────────────┘
```

---

## Transaction Flow

```
State: IDLE                              State: HAS_MONEY
┌────────────────────┐                   ┌────────────────────────┐
│ machine.InsertMoney │                   │ machine.SelectProduct  │
│ (20)                │                   │ ("Chips")              │
│                     │                   │                        │
│ IdleState handles:  │                   │ HasMoneyState handles: │
│  → balance += 20    │                   │  → product exists? ✓   │
│  → "Balance: 20"    │                   │  → in stock? ✓         │
│  → switch to        │                   │  → balance >= price? ✓ │
│    HasMoneyState    │───────────────────│  → switch to           │
└────────────────────┘                   │    DispenseState       │
                                          │  → call Dispense()    │
                                          └────────────────────────┘
                                                    │
                                                    ▼
                                          ┌────────────────────────┐
                                          │ Dispense("Chips")       │
                                          │                        │
                                          │ price = 20              │
                                          │ change = 30 - 20 = 10   │
                                          │ totalCash += 20          │
                                          │ quantity--               │
                                          │ "Dispensing... Chips"    │
                                          │ "Change: Rs.10"          │
                                          │ → switch to IdleState   │
                                          └────────────────────────┘
```

---

## Design Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **State** | `IVendingMachineState` | The machine's behavior changes completely based on its current state. Each state is a separate class that handles the same methods differently |

### Why State Pattern Here?

Without the State pattern, you'd have this everywhere:

```csharp
// WITHOUT State pattern — conditional spaghetti
public void SelectProduct(string name)
{
    if (_state == "idle") Console.WriteLine("Insert money first");
    else if (_state == "has_money") { /* actual logic */ }
    else if (_state == "dispensing") Console.WriteLine("Wait...");
}
```

With the State pattern, each state class handles its own behavior. Adding a new state (e.g., `MaintenanceState`) requires zero changes to existing code.

---

## Key Design Decisions

### 1. States Receive the Machine as a Parameter
Each state method takes `VendingMachine machine` as a parameter. This lets states call machine methods (add money, set state, dispense) without holding a permanent reference.

### 2. Dispense Lives on VendingMachine, Not DispenseState
The actual dispensing logic (price calculation, change, quantity update) is on `VendingMachine.Dispense()`. The `DispenseState` only exists to block other operations during dispensing. Clean separation of "what to do" vs. "what's allowed."

### 3. Auto-Return to Idle After Dispense
After dispensing, the machine automatically switches back to `IdleState`. There's no manual step needed — the transaction is complete.

### 4. Product as a Simple Data Class
`Product` has `Name`, `Price`, and `Quantity` as public fields. For an interview, this is sufficient. In production, you'd use private setters and methods.

---

## Usage Example

```csharp
var machine = new VendingMachine();
machine.Refill("Chips", 10, 20);
machine.Refill("Cold Coffee", 5, 40);
machine.Refill("Samosa", 8, 15);

machine.DisplayProducts();
// Chips - Rs.20 (10 in stock)
// Cold Coffee - Rs.40 (5 in stock)
// Samosa - Rs.15 (8 in stock)

machine.InsertMoney(20);    // "Adding amount 20, Balance: 20"
machine.InsertMoney(10);    // "Added more money 10, balance is 30"
machine.SelectProduct("Chips");
// "Dispensing... Chips."
// "Returning the change back to you: Rs.10"

machine.InsertMoney(10);
machine.SelectProduct("Cold Coffee");
// "Not enough balance here"
machine.Cancel();
// "Returning the 10 back to you"

Console.WriteLine($"Total cash: Rs.{machine.CollectCash()}");
// "Total cash: Rs.20"
```

---

## Possible Extensions

- **Multiple payment methods**: Strategy pattern for cash, UPI, card
- **Maintenance state**: Block all operations when machine needs servicing
- **Inventory alerts**: Observer pattern to notify operator when stock is low
- **Admin panel**: Secure interface for refilling and collecting cash
- **Timeout**: Auto-cancel and refund if no product selected within N seconds
- **Display screen**: Show product grid with availability indicators
