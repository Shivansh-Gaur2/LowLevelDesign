using System;
using System.Collections.Generic;
using System.Text.Json;

// === Logging Framework — Observer + Singleton ===

public enum LogLevel
{
    DEBUG = 0, INFO = 1, WARNING = 2, ERROR = 3, FATAL = 4
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; } = "";
    public string Context { get; set; } = "";

    public override string ToString()
    {
        string contextStr = string.IsNullOrEmpty(Context) ? "" : $" [{Context}]";
        return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}]{contextStr} {Message}";
    }
}

public interface ILogSink
{
    bool ShouldLog(LogLevel level);
    void Handle(LogEntry entry);
}

public class ConsoleSink : ILogSink
{
    private LogLevel _minLevel;
    public ConsoleSink(LogLevel minLevel) { _minLevel = minLevel; }
    public bool ShouldLog(LogLevel level) => (int)level >= (int)_minLevel;
    public void Handle(LogEntry entry) => Console.WriteLine(entry.ToString());
}

public class FileSink : ILogSink
{
    private LogLevel _minLevel;
    private List<string> _logs = new();
    private string _filename;

    public FileSink(LogLevel minLevel, string filename)
    {
        _minLevel = minLevel;
        _filename = filename;
    }

    public bool ShouldLog(LogLevel level) => (int)level >= (int)_minLevel;
    public void Handle(LogEntry entry) => _logs.Add(entry.ToString());
    public List<string> GetLogs() => _logs;
}

public class JsonSink : ILogSink
{
    private LogLevel _minLevel;
    private List<string> _jsonLogs = new();

    public JsonSink(LogLevel minLevel) { _minLevel = minLevel; }
    public bool ShouldLog(LogLevel level) => (int)level >= (int)_minLevel;

    public void Handle(LogEntry entry)
    {
        var json = new { timestamp = entry.Timestamp, level = entry.Level.ToString(), message = entry.Message, context = entry.Context };
        _jsonLogs.Add(JsonSerializer.Serialize(json));
    }

    public List<string> GetLogs() => _jsonLogs;
}

public class Logger
{
    private static Logger? _instance;
    private static readonly object _lock = new();
    private List<ILogSink> _sinks;

    private Logger() { _sinks = new(); }

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new Logger();
            }
        }
        return _instance;
    }

    public void AddSink(ILogSink sink) => _sinks.Add(sink);

    private void Log(LogLevel level, string message, string context = "")
    {
        var entry = new LogEntry { Timestamp = DateTime.Now, Level = level, Message = message, Context = context };
        foreach (var sink in _sinks)
            if (sink.ShouldLog(level))
                sink.Handle(entry);
    }

    public void Debug(string message, string context = "") => Log(LogLevel.DEBUG, message, context);
    public void Info(string message, string context = "") => Log(LogLevel.INFO, message, context);
    public void Warning(string message, string context = "") => Log(LogLevel.WARNING, message, context);
    public void Error(string message, string context = "") => Log(LogLevel.ERROR, message, context);
    public void Fatal(string message, string context = "") => Log(LogLevel.FATAL, message, context);
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Logging Framework ===\n");
        var logger = Logger.GetInstance();
        logger.AddSink(new ConsoleSink(LogLevel.DEBUG));
        logger.AddSink(new FileSink(LogLevel.WARNING, "app.log"));
        logger.AddSink(new JsonSink(LogLevel.ERROR));

        logger.Debug("Starting up...");
        logger.Info("Server listening on port 5000", "Startup");
        logger.Warning("High memory usage detected", "Monitor");
        logger.Error("Database connection failed", "OrderService");
        logger.Fatal("System out of memory!", "Runtime");
    }
}
