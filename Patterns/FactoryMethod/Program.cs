using System;
using System.Collections.Generic;

// === Factory Method — Shipping Cost Calculator ===

public interface IShippingCalculator
{
    double Calculate(double weightKg, double distanceKm);
}

public class BlueDartCalculator : IShippingCalculator
{
    public double Calculate(double weightKg, double distanceKm) => 50 + (weightKg * 15) + (distanceKm * 0.10);
}

public class DelhiveryCalculator : IShippingCalculator
{
    public double Calculate(double weightKg, double distanceKm) => 30 + (weightKg * 10) + (distanceKm * 0.05);
}

public class IndiaPostCalculator : IShippingCalculator
{
    public double Calculate(double weightKg, double distanceKm) => 15 + (weightKg * 5) + (distanceKm * 0.02);
}

public static class ShippingFactory
{
    private static readonly Dictionary<string, Func<IShippingCalculator>> Creators = new()
    {
        ["bluedart"] = () => new BlueDartCalculator(),
        ["delhivery"] = () => new DelhiveryCalculator(),
        ["indiapost"] = () => new IndiaPostCalculator()
    };

    public static IShippingCalculator Create(string type)
    {
        if (!Creators.ContainsKey(type))
            throw new ArgumentException($"Unknown carrier: {type}");
        return Creators[type]();
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Factory Method ===\n");
        var carriers = new[] { "bluedart", "delhivery", "indiapost" };
        foreach (var c in carriers)
        {
            var calc = ShippingFactory.Create(c);
            Console.WriteLine($"{c}: Rs.{calc.Calculate(5, 500):F2}");
        }
    }
}
