# Design LRU Cache

A low-level design implementation of a Least Recently Used (LRU) Cache in C# using a **HashMap + Doubly Linked List** — the classic O(1) approach.

---

## Problem Statement

Design a cache with fixed capacity where:
- `Get(key)` returns the value if present, -1 otherwise — **O(1)**
- `Put(key, value)` inserts or updates a key-value pair — **O(1)**
- When the cache is full and a new item is added, the **least recently used** item is evicted
- Both `Get` and `Put` count as "using" an item (it becomes most recently used)

---

## Real-World Analogy

Think of your browser's "recently visited" tabs. You have space for 3 tabs:

```
You visit: Rahul's page → Priya's page → Amit's page
Tabs: [Rahul] [Priya] [Amit]     (Amit = most recent)

You visit Rahul's page again:
Tabs: [Priya] [Amit] [Rahul]     (Rahul moves to front)

You visit Neha's page (cache full!):
Evict Priya (least recent)
Tabs: [Amit] [Rahul] [Neha]
```

---

## Why HashMap + Doubly Linked List?

We need two things to be O(1):
1. **Find an item by key** → HashMap gives us O(1) lookup
2. **Know which item is least/most recent** → Linked List gives us O(1) ordering

Neither data structure alone is enough:

| Data Structure | Lookup | Insert/Remove | Ordering |
|---------------|--------|---------------|----------|
| HashMap | O(1) ✓ | O(1) ✓ | No ordering ✗ |
| Linked List | O(n) ✗ | O(1) ✓ | Maintains order ✓ |
| **Both together** | **O(1) ✓** | **O(1) ✓** | **Order ✓** |

---

## Architecture

```
Dictionary<string, Node>              Doubly Linked List
┌──────────────────────┐
│ "rahul" ──→ Node(100)│──────┐    HEAD ←→ [amit|300] ←→ [neha|400] ←→ [rahul|100]
│ "amit"  ──→ Node(300)│──────┤    (dummy)     ▲              ▲              ▲
│ "neha"  ──→ Node(400)│──────┘            least recent                 most recent
└──────────────────────┘                                                   (TAIL)
         │                                     │
         │ O(1) find                           │ O(1) add/remove/move
         ▼                                     ▼
    "Which node?"                      "Where in the order?"
```

---

## Operations Visualized

### Get("rahul") — Cache Hit
```
Before: HEAD ←→ [rahul|100] ←→ [priya|200] ←→ [amit|300]
                                                     ▲ TAIL

Step 1: Find "rahul" in HashMap → O(1) → found Node
Step 2: Remove node from current position
Step 3: Add node to TAIL (most recent)

After:  HEAD ←→ [priya|200] ←→ [amit|300] ←→ [rahul|100]
                                                    ▲ TAIL
Return: 100
```

### Put("neha", 400) — Cache Full, Eviction
```
Before: HEAD ←→ [priya|200] ←→ [amit|300] ←→ [rahul|100]    capacity: 3
                  ▲ least recent                   ▲ TAIL

Step 1: Cache full (3/3) → evict least recent
Step 2: Remove HEAD.Next (priya), delete from HashMap
Step 3: Create new Node("neha", 400)
Step 4: Add to TAIL

After:  HEAD ←→ [amit|300] ←→ [rahul|100] ←→ [neha|400]     capacity: 3
                                                   ▲ TAIL
HashMap: {"amit": Node, "rahul": Node, "neha": Node}
         (priya removed)
```

---

## Class Diagram

```
┌────────────────────┐         ┌──────────────────────────┐
│       Node          │         │        LRUCache           │
│                     │         │                           │
│  Key: string        │         │  _capacity: int           │
│  Value: int         │◄────────│  _cache: Dict<str, Node>  │
│  Prev: Node?        │         │  _head: Node (dummy)      │
│  Next: Node?        │         │  _tail: Node              │
│                     │         │                           │
└────────────────────┘         │  Get(key): int             │
                                │  Put(key, value): void     │
   Internal helpers:            │                           │
   AddToFront(node)             │  ── private ──            │
   RemoveNode(node)             │  AddToFront(node)         │
   MoveToFront(node)            │  RemoveNode(node)         │
   RemoveLast() → Node          │  MoveToFront(node)        │
                                │  RemoveLast(): Node       │
                                └──────────────────────────┘
```

---

## Complexity Analysis

| Operation | Time | Space |
|-----------|------|-------|
| `Get(key)` | O(1) | — |
| `Put(key, value)` | O(1) | — |
| Space overall | — | O(capacity) |

Every operation is constant time because:
- HashMap lookup is O(1)
- Doubly linked list insert/remove is O(1) when you have the node reference
- The HashMap stores the node reference directly (no searching needed)

---

## Key Design Decisions

### 1. Dummy Head Node
The head is a sentinel node (dummy) that never gets evicted. This avoids null checks when the list is empty — `HEAD.Next` is always the least recently used item.

### 2. "Front" = Tail (Most Recent)
New items and accessed items go to the tail. The item right after HEAD (dummy) is the eviction candidate. This convention is consistent with "the end of the list = most recent."

### 3. HashMap Values Are Nodes (Not Just Values)
The dictionary maps `string → Node`, not `string → int`. This is the key insight — when we need to move a node in the linked list, we already have its reference from the HashMap.

---

## Usage Example

```csharp
var cache = new LRUCache(3);

cache.Put("rahul", 100);
cache.Put("priya", 200);
cache.Put("amit", 300);

cache.Get("rahul");     // 100 (rahul moves to most recent)
cache.Put("neha", 400); // evicts priya (least recently used)
cache.Get("priya");     // -1 (evicted!)
cache.Get("amit");      // 300
cache.Get("neha");      // 400
```

---

## Where LRU Cache Appears in Real Systems

- **CPU caches**: L1/L2/L3 use LRU-like eviction
- **Database buffer pools**: PostgreSQL, MySQL keep hot pages in memory
- **CDN edge caches**: Cloudflare, Akamai evict cold content
- **Redis**: `maxmemory-policy allkeys-lru`
- **Operating systems**: Page replacement algorithms (LRU approximations)
- **DNS resolvers**: Cache recent lookups, evict stale ones

---

## Possible Extensions

- **TTL (Time-to-Live)**: Auto-expire entries after a duration
- **LFU variant**: Evict least *frequently* used instead of least *recently* used
- **Thread safety**: Add `lock` around Get/Put for concurrent access
- **Generic types**: `LRUCache<TKey, TValue>` instead of hardcoded string/int
- **Eviction callback**: Notify when an item is evicted (for cleanup)
- **Size-based eviction**: Evict based on total memory, not just count
