# Design Snake & Ladders

A low-level design implementation of the classic Snake & Ladders board game in C# with clean entity separation.

---

## Problem Statement

Design a Snake & Ladders game where:
- A board has 100 cells (numbered 1 to 100)
- Snakes move a player DOWN (head → tail)
- Ladders move a player UP (bottom → top)
- Players take turns rolling a dice (1-6)
- If a move would exceed 100, the player stays put
- The first player to land on exactly 100 wins

---

## Real-World Mapping

```
Real World                    Code
──────────                    ────
Physical board          →     Board (snakes + ladders as dictionaries)
Plastic tokens          →     Player (name + position)
Rolling a cube          →     Dice.Roll() → Random(1, 7)
Moving your token       →     player.Position += roll
Landing on snake head   →     Board.GetEndPosition() returns tail
Landing on ladder base  →     Board.GetEndPosition() returns top
"Your turn"             →     Game.CurrentPlayerIndex cycles
"You win!"              →     player.Position == 100
```

---

## Architecture

```
┌──────────────────────────────────────────────────────────┐
│                         Game                              │
│                                                           │
│  Board         ← owns the snake/ladder mapping            │
│  Dice          ← generates random rolls                   │
│  Players[]     ← list of all players                      │
│  CurrentIndex  ← whose turn is it                         │
│                                                           │
│  PlayTurn()    ← roll, move, check snake/ladder, display  │
│  IsGameOver()  ← did current player reach 100?            │
│  Play()        ← main game loop                           │
└──────────────────────────────────────────────────────────┘
         │              │              │
         ▼              ▼              ▼
  ┌────────────┐  ┌──────────┐  ┌──────────┐
  │   Board     │  │   Dice    │  │  Player   │
  │             │  │           │  │           │
  │  Snakes{}   │  │  Roll()   │  │  Name     │
  │  Ladders{}  │  │  → 1..6   │  │  Position │
  │             │  └──────────┘  └──────────┘
  │  GetEnd     │
  │  Position() │
  └────────────┘
```

---

## Game Flow

```
                    ┌─────────┐
                    │  Start   │
                    │ Pos = 0  │
                    └────┬─────┘
                         │
                    ┌────▼─────┐
              ┌─────│ Roll Dice │
              │     └────┬─────┘
              │          │
              │     ┌────▼──────────────┐
              │     │ pos + roll > 100? │
              │     └────┬──────┬───────┘
              │         YES     NO
              │          │      │
              │          │ ┌────▼────────────┐
              │          │ │ Move to new pos  │
              │          │ └────┬─────────────┘
              │          │      │
              │          │ ┌────▼────────────────────┐
              │          │ │ Snake or Ladder here?    │
              │          │ └────┬───────────┬─────────┘
              │          │    SNAKE       LADDER
              │          │      │            │
              │          │  Slide DOWN   Climb UP
              │          │      │            │
              │          │      └─────┬──────┘
              │          │            │
              │     ┌────▼────────────▼──┐
              │     │  Position == 100?   │
              │     └────┬──────────┬────┘
              │         YES         NO
              │          │          │
              │     ┌────▼───┐  ┌──▼───────────┐
              │     │ YOU WIN │  │ Next player's │
              │     └────────┘  │ turn          │──┐
              │                 └───────────────┘  │
              │                                    │
              └────────────────────────────────────┘
```

---

## Board Configuration (Example)

```
     SNAKES (head → tail)           LADDERS (bottom → top)
     ──────────────────             ───────────────────────
     27 → 5   (drop 22)            3  → 22  (climb 19)
     40 → 3   (drop 37)            15 → 35  (climb 20)
     72 → 12  (drop 60)            28 → 76  (climb 48)
     98 → 56  (drop 42)            50 → 97  (climb 47)
```

The `Board.GetEndPosition(position)` method checks both dictionaries:
- If position is a snake head → return tail (lower number)
- If position is a ladder base → return top (higher number)
- Otherwise → return same position (nothing happens)

---

## Sample Game Output

```
=== Snake & Ladders ===

Rahul rolls 3: 0 -> 3 -> LADDER -> 22
Priya rolls 5: 0 -> 5
Rahul rolls 6: 22 -> 28 -> LADDER -> 76
Priya rolls 4: 5 -> 9
Rahul rolls 2: 76 -> 78
Priya rolls 6: 9 -> 15 -> LADDER -> 35
Rahul rolls 4: 78 -> 82
...
Rahul rolls 3: 97 -> 100
Rahul reaches 100! Rahul wins!!
```

---

## Class Diagram

```
┌─────────────┐       ┌──────────────────────────┐
│    Dice      │       │         Board             │
│              │       │                           │
│  Roll(): int │       │  Snakes: Dict<int,int>    │
│  → Random    │       │  Ladders: Dict<int,int>   │
│    (1..6)    │       │                           │
└─────────────┘       │  AddSnake(head, tail)     │
                       │  AddLadder(bottom, top)   │
┌─────────────┐       │  GetEndPosition(pos): int │
│   Player     │       └──────────────────────────┘
│              │
│  Name: str   │       ┌──────────────────────────┐
│  Position: 0 │       │          Game             │
└─────────────┘       │                           │
                       │  Board, Dice, Players[]   │
                       │  CurrentPlayerIndex       │
                       │                           │
                       │  PlayTurn()               │
                       │  IsGameOver(): bool       │
                       │  Play() ← main loop       │
                       └──────────────────────────┘
```

---

## Design Patterns Used

This is intentionally pattern-light — the problem is about clean entity modeling, not pattern stacking.

| Concept | Where | Why |
|---------|-------|-----|
| **Entity separation** | Board, Dice, Player, Game | Each class has a single clear responsibility |
| **Turn-based loop** | `Game.Play()` | Cycles through players with modular index |

---

## Key Design Decisions

### 1. Board Stores Mappings, Not Cells
Instead of creating 100 cell objects, the board uses two dictionaries. Most cells are empty — only special positions (snake heads, ladder bases) matter. This is memory-efficient and simple.

### 2. "Overshoot" Rule
If a player on position 97 rolls a 5, they'd land on 102 — which doesn't exist. The game skips their move. This matches the real board game rule.

### 3. Single GetEndPosition() Method
The board doesn't have separate `CheckSnake()` and `CheckLadder()` methods. One method handles both — check snakes dict, then ladders dict, then return position unchanged. Clean and simple.

---

## Usage Example

```csharp
var board = new Board();
board.AddSnake(27, 5);
board.AddSnake(98, 56);
board.AddLadder(3, 22);
board.AddLadder(50, 97);

var players = new List<Player> { new Player("Rahul"), new Player("Priya") };
var game = new Game(board, players);
game.Play();
```

---

## Possible Extensions

- **Custom dice**: Strategy pattern for different dice (weighted, multiple dice)
- **Power-ups**: Land on special cells for double roll, immunity from next snake
- **Board builder**: Factory pattern to generate random boards with balanced snake/ladder ratios
- **Undo**: Command pattern to reverse the last move
- **Multiplayer**: Support N players with dynamic turn order
- **GUI**: Render the board visually with player positions
