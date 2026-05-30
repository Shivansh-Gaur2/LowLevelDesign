# Design Tic-Tac-Toe

A low-level design implementation of the classic Tic-Tac-Toe game in C# with clean OOP — a 3x3 grid, two players, and an interactive console game loop.

---

## Problem Statement

Design a Tic-Tac-Toe game where:
- Two players (X and O) take turns placing their symbol on a 3x3 grid
- A player wins by getting 3 in a row (horizontal, vertical, or diagonal)
- The game ends in a draw if all 9 cells are filled with no winner
- Invalid moves (out-of-bounds, occupied cell) are rejected and the player retries

---

## Architecture

```
┌──────────────────────────────────────────────────────┐
│                        Game                           │
│                                                       │
│  Board           ← owns the grid                      │
│  CurrentPlayer   ← alternates X / O                   │
│                                                       │
│  Play()          ← main game loop                     │
│    1. Display board                                   │
│    2. Get player input (row, col)                     │
│    3. Place piece → validate                          │
│    4. Check win → check draw → switch turns           │
└──────────────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────┐
│                       Board                           │
│                                                       │
│  _grid: PlayerType[3,3]                               │
│  _moveCount: int                                      │
│                                                       │
│  Place(row, col, player) → bool                       │
│  CheckWin(player) → bool                              │
│  IsFull() → bool                                      │
│  Display() → renders to console                       │
└──────────────────────────────────────────────────────┘
```

---

## Win Detection Logic

The `CheckWin()` method checks 8 possible winning lines:

```
ROWS (3)          COLUMNS (3)       DIAGONALS (2)
─────────         ────────────      ──────────────
[X][X][X]         [X][ ][ ]        [X][ ][ ]        [ ][ ][X]
[ ][ ][ ]         [X][ ][ ]        [ ][X][ ]        [ ][X][ ]
[ ][ ][ ]         [X][ ][ ]        [ ][ ][X]        [X][ ][ ]
```

```csharp
// All 8 checks in one method:
for (int i = 0; i < 3; i++)
{
    // Row i
    if (grid[i,0] == p && grid[i,1] == p && grid[i,2] == p) return true;
    // Column i
    if (grid[0,i] == p && grid[1,i] == p && grid[2,i] == p) return true;
}
// Main diagonal
if (grid[0,0] == p && grid[1,1] == p && grid[2,2] == p) return true;
// Anti-diagonal
if (grid[0,2] == p && grid[1,1] == p && grid[2,0] == p) return true;
```

---

## Game Flow

```
         ┌───────────┐
         │ Player X's │
         │   turn     │
         └─────┬──────┘
               │
         ┌─────▼──────┐
         │ Display     │
         │ board       │
         └─────┬──────┘
               │
         ┌─────▼──────────────┐
         │ Input: row col      │
         └─────┬──────────────┘
               │
         ┌─────▼──────────────┐     ┌──────────────┐
         │ Valid move?         │─NO─▶│ "Cannot place │
         └─────┬──────────────┘     │  here"        │──┐
              YES                    └──────────────┘  │
               │                                       │
         ┌─────▼──────────────┐                        │
         │ Player wins?       │─YES─▶ "Player X wins!" │
         └─────┬──────────────┘                        │
               NO                                      │
               │                                       │
         ┌─────▼──────────────┐                        │
         │ Board full?        │─YES─▶ "Game draw!"     │
         └─────┬──────────────┘                        │
               NO                                      │
               │                                       │
         ┌─────▼──────────────┐                        │
         │ Switch to Player O  │◄──────────────────────┘
         └─────┬──────────────┘
               │
               └──── loop back ────┘
```

---

## Board Rendering

```
 X | O | X
-----------
 O | X |  
-----------
   | O |  

Cell coordinates:
 (0,0) | (0,1) | (0,2)
 (1,0) | (1,1) | (1,2)
 (2,0) | (2,1) | (2,2)
```

---

## Class Diagram

```
┌─────────────────────┐        ┌──────────────────────────┐
│     PlayerType       │        │         Board             │
│     (enum)           │        │                           │
│                      │        │  _grid: PlayerType[3,3]   │
│  None = empty cell   │        │  _moveCount: int          │
│  X    = player X     │        │                           │
│  O    = player O     │        │  Place(r, c, p): bool     │
└─────────────────────┘        │  CheckWin(p): bool        │
                                │  IsFull(): bool           │
┌─────────────────────┐        │  Display(): void          │
│       Game           │        └──────────────────────────┘
│                      │
│  _board: Board       │
│  _currentPlayer: X/O │
│                      │
│  Play(): void        │
│  (main game loop)    │
└─────────────────────┘
```

---

## Key Design Decisions

### 1. Board Owns All Grid Logic
The `Board` class handles placement, validation, win detection, and rendering. The `Game` class only orchestrates the loop and player switching. Clean separation.

### 2. Move Counter Instead of Scanning
`_moveCount` tracks total moves. `IsFull()` just checks `_moveCount == 9` instead of scanning all 9 cells. Small but smart optimization.

### 3. Validation at Placement
`Place()` returns `false` for invalid moves (out-of-bounds or occupied). The game loop retries without switching players. No exceptions for expected user errors.

### 4. Enum for Cell State
`PlayerType.None` represents an empty cell. This avoids nullable types and makes win-checking comparisons clean (`grid[r,c] == player`).

---

## Usage Example

```
=== Tic-Tac-Toe ===
   |   |  
-----------
   |   |  
-----------
   |   |  
Player X, enter row and col (0-2): 1 1
   |   |  
-----------
   | X |  
-----------
   |   |  
Player O, enter row and col (0-2): 0 0
 O |   |  
-----------
   | X |  
-----------
   |   |  
...
Player X wins!
```

---

## Possible Extensions

- **NxN board**: Generalize from 3x3 to any size with configurable win length
- **AI opponent**: Minimax algorithm for unbeatable computer player
- **Undo/redo**: Command pattern to reverse moves
- **Strategy pattern**: Pluggable win-checking strategies for different board sizes
- **Score tracking**: Best-of-N series between two players
- **Network play**: Two players on different machines via sockets
