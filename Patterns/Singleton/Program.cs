using System;
using System.Collections.Generic;

// === Singleton — Config Manager ===

public class ConfigManager
{
    private static ConfigManager? _instance;
    private static readonly object _lock = new();
    private Dictionary<string, string> _settings;

    private ConfigManager()
    {
        Console.WriteLine("Reading config.json from disk... (EXPENSIVE!)");
        _settings = new Dictionary<string, string>
        {
            ["db_url"] = "Server=prod-db;Database=MyApp;",
            ["api_key"] = "sk-abc123xyz",
            ["max_retries"] = "3",
            ["feature_dark_mode"] = "true"
        };
    }

    public string? Get(string key) => _settings.ContainsKey(key) ? _settings[key] : null;

    public static ConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new ConfigManager();
                }
            }
            return _instance;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Singleton Pattern ===\n");
        var a = ConfigManager.Instance;
        var b = ConfigManager.Instance;
        Console.WriteLine($"Same instance? {a == b}");
        Console.WriteLine($"API Key: {a.Get("api_key")}");
        Console.WriteLine($"DB URL: {b.Get("db_url")}");
    }
}
