using System;
using System.Collections.Generic;

// === Proxy — Caching Weather API ===

public interface IWeatherService { string GetForecast(string city); }

public class RealWeatherService : IWeatherService
{
    public string GetForecast(string city)
    {
        Console.WriteLine($"  [API CALL] Fetching weather for {city}...");
        return $"{city}: 32°C, Sunny";
    }
}

public class CachingWeatherProxy : IWeatherService
{
    private readonly RealWeatherService _real = new();
    private readonly Dictionary<string, (string data, DateTime expiry)> _cache = new();
    private readonly int _ttlSeconds;

    public CachingWeatherProxy(int ttlSeconds = 300) { _ttlSeconds = ttlSeconds; }

    public string GetForecast(string city)
    {
        if (_cache.ContainsKey(city) && _cache[city].expiry > DateTime.Now)
        {
            Console.WriteLine($"  [CACHE HIT] {city}");
            return _cache[city].data;
        }
        var result = _real.GetForecast(city);
        _cache[city] = (result, DateTime.Now.AddSeconds(_ttlSeconds));
        return result;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Proxy Pattern ===\n");
        IWeatherService weather = new CachingWeatherProxy(60);
        Console.WriteLine(weather.GetForecast("Mumbai"));
        Console.WriteLine(weather.GetForecast("Mumbai"));  // cache hit
        Console.WriteLine(weather.GetForecast("Delhi"));   // miss
    }
}
