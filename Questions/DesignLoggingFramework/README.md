# Design Logging Framework

A low-level design implementation of a pluggable logging framework in C# using **Singleton**, **Observer**, and **Strategy** patterns.

---

## Problem Statement

Design a logging framework where:
- A single logger instance exists throughout the application (Singleton)
- Log messages have severity levels: DEBUG, INFO, WARNING, ERROR, FATAL
- Multiple output destinations (sinks) can be attached: console, file, JSON
- Each sink independently decides which log levels it cares about
- Adding a new sink requires zero changes to existing code (Open/Closed Principle)

---

## Real-World Analogy

Think of a news desk at a TV station. One anchor reads the news (Logger). Multiple channels are broadcasting:
- **Channel 1** (Console): Shows everything — even minor updates
- **Channel 2** (File): Only records warnings and above — for the record
- **Channel 3** (JSON API): Only captures errors — feeds the monitoring dashboard

Each channel has its own threshold. The anchor doesn't care how many channels are listening or what they filter — they just read the news.

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Logger (Singleton)                         │
│                                                              │
│  Debug(msg)  Info(msg)  Warning(msg)  Error(msg)  Fatal(msg) │
│                         │                                    │
│                         ▼                                    │
│                   Log(level, msg)                            │
│                         │                                    │
│                         ▼                                    │
│              ┌─── for each sink ────┐                        │
│              │                      │                        │
│              │  sink.ShouldLog()?   │                        │
│              │    YES → Handle()    │                        │
│              │    NO  → skip        │                        │
│              └──────────────────────┘                        │
└─────────────────────────────────────────────────────────────┘
                         │
         ┌───────────────┼──────────────────┐
         ▼               ▼                  ▼
  ┌─────────────┐ ┌─────────────┐  ┌──────────────┐
  │ ConsoleSink  │ │  FileSink    │  │   JsonSink    │
  │              │ │              │  │               │
  │ min: DEBUG   │ │ min: WARNING │  │ min: ERROR    │
  │ → stdout     │ │ → file list  │  │ → JSON list   │
  └─────────────┘ └─────────────┘  └──────────────┘
```

---

## Log Flow

```
logger.Error("Database connection failed", "OrderService")
  │
  ▼
Create LogEntry:
  Timestamp: 2026-05-30 14:32:01
  Level:     ERROR
  Message:   "Database connection failed"
  Context:   "OrderService"
  │
  ▼
Fan out to all sinks:
  │
  ├── ConsoleSink (min: DEBUG)
  │   ShouldLog(ERROR)? → YES (ERROR ≥ DEBUG)
  │   Output: [2026-05-30 14:32:01] [ERROR] [OrderService] Database connection failed
  │
  ├── FileSink (min: WARNING)
  │   ShouldLog(ERROR)? → YES (ERROR ≥ WARNING)
  │   Output: stored in file list
  │
  └── JsonSink (min: ERROR)
      ShouldLog(ERROR)? → YES (ERROR ≥ ERROR)
      Output: {"timestamp":"...","level":"ERROR","message":"Database connection failed","context":"OrderService"}
```

---

## Class Diagram

```
                ┌──────────────────────────┐
                │      Logger (Singleton)   │
                │                           │
                │ - _instance: Logger       │
                │ - _lock: object           │
                │ - _sinks: List<ILogSink>  │
                │                           │
                │ + GetInstance(): Logger    │
                │ + AddSink(sink)           │
                │ + Debug/Info/Warning/     │
                │   Error/Fatal(msg, ctx)   │
                └────────────┬──────────────┘
                             │ notifies
                             ▼
                ┌──────────────────────┐
                │     ILogSink          │
                │                       │
                │ ShouldLog(level): bool│
                │ Handle(entry): void   │
                └──────────┬────────────┘
                           │ implements
              ┌────────────┼────────────────┐
              ▼            ▼                ▼
      ┌────────────┐ ┌──────────┐  ┌──────────────┐
      │ ConsoleSink │ │ FileSink  │  │  JsonSink     │
      │             │ │           │  │               │
      │ Writes to   │ │ Appends   │  │ Serializes    │
      │ stdout      │ │ to list   │  │ to JSON       │
      └────────────┘ └──────────┘  └──────────────┘

      ┌─────────────────────────┐
      │        LogEntry          │
      │                          │
      │  Timestamp: DateTime     │
      │  Level: LogLevel         │
      │  Message: string         │
      │  Context: string         │
      │                          │
      │  ToString() → formatted  │
      └─────────────────────────┘
```

---

## Log Levels (Severity Hierarchy)

```
DEBUG (0)  →  Granular development info     │  Most verbose
INFO  (1)  →  Normal operational events     │
WARNING(2) →  Something unexpected happened │
ERROR (3)  →  Operation failed              │
FATAL (4)  →  System is going down          ▼  Least verbose
```

Each sink has a `_minLevel`. A sink with `minLevel = WARNING` will process WARNING, ERROR, and FATAL — but ignore DEBUG and INFO.

---

## Design Patterns Used

| Pattern | Where | Why |
|---------|-------|-----|
| **Singleton** | `Logger.GetInstance()` | One logger per application. Thread-safe with double-checked locking |
| **Observer** | `ILogSink` list | Logger notifies all registered sinks. Sinks are independent observers |
| **Strategy** | `ShouldLog()` per sink | Each sink decides its own filtering threshold independently |

---

## Key Design Decisions

### 1. Thread-Safe Singleton
Uses `lock` + double-checked locking pattern. Only one `Logger` instance is ever created, even under concurrent access.

### 2. Sink-Level Filtering (Not Logger-Level)
The `Logger` doesn't filter — it broadcasts to all sinks. Each sink decides independently. This means one sink can capture DEBUG while another only captures FATAL.

### 3. LogEntry as a Value Object
`LogEntry` bundles timestamp, level, message, and context into one object. Sinks receive the full entry and format it however they want (plain text, JSON, etc.).

### 4. Context String for Categorization
The optional `context` parameter ("OrderService", "Startup", "Monitor") lets you trace which component generated the log without creating separate loggers.

---

## Usage Example

```csharp
var logger = Logger.GetInstance();

// Attach sinks with different thresholds
logger.AddSink(new ConsoleSink(LogLevel.DEBUG));       // shows everything
logger.AddSink(new FileSink(LogLevel.WARNING, "app.log")); // warnings+
logger.AddSink(new JsonSink(LogLevel.ERROR));          // errors only

// Log at different levels
logger.Debug("Starting up...");
logger.Info("Server listening on port 5000", "Startup");
logger.Warning("High memory usage detected", "Monitor");
logger.Error("Database connection failed", "OrderService");
logger.Fatal("System out of memory!", "Runtime");
```

**Console output** (all levels):
```
[2026-05-30 14:32:01] [DEBUG] Starting up...
[2026-05-30 14:32:01] [INFO] [Startup] Server listening on port 5000
[2026-05-30 14:32:01] [WARNING] [Monitor] High memory usage detected
[2026-05-30 14:32:01] [ERROR] [OrderService] Database connection failed
[2026-05-30 14:32:01] [FATAL] [Runtime] System out of memory!
```

**File sink** captures only the last 3. **JSON sink** captures only the last 2.

---

## Possible Extensions

- **Async sinks**: Write logs in background thread to avoid blocking the caller
- **Log rotation**: FileSink auto-rotates when file size exceeds a threshold
- **Structured logging**: Key-value pairs instead of plain string messages
- **Correlation IDs**: Track a request across multiple log entries
- **Rate limiting**: Throttle repeated log messages (e.g., same error 1000x/sec)
- **Remote sink**: Ship logs to ELK Stack, Datadog, or Splunk via HTTP
