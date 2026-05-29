using System;
using System.Collections.Generic;

// === Prototype — Game Soldier Cloning ===

public class Soldier
{
    public string Type { get; set; } = "";
    public int Health { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public List<string> Abilities { get; set; } = new();

    public Soldier(string type)
    {
        Console.WriteLine($"Loading {type} from database... (EXPENSIVE!)");
        Type = type;
        switch (type.ToLower())
        {
            case "knight": Health = 100; Attack = 25; Defense = 20; Speed = 5; Abilities = new() { "Shield Bash", "Charge" }; break;
            case "archer": Health = 60; Attack = 35; Defense = 8; Speed = 12; Abilities = new() { "Fire Arrow", "Multi-Shot" }; break;
            case "mage": Health = 45; Attack = 50; Defense = 5; Speed = 8; Abilities = new() { "Fireball", "Freeze", "Teleport" }; break;
        }
    }

    private Soldier() { }

    public Soldier Clone() => new Soldier
    {
        Type = this.Type, Health = this.Health, Attack = this.Attack,
        Defense = this.Defense, Speed = this.Speed,
        Abilities = new List<string>(this.Abilities)
    };

    public override string ToString() => $"[{Type}] HP:{Health} ATK:{Attack} DEF:{Defense} | {string.Join(", ", Abilities)}";
}

public class SoldierRegistry
{
    private readonly Dictionary<string, Soldier> _prototypes = new();
    public void Register(string name, Soldier prototype) => _prototypes[name] = prototype;
    public Soldier Get(string name) => _prototypes.ContainsKey(name) ? _prototypes[name].Clone() : throw new ArgumentException($"No prototype: {name}");
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Prototype Pattern ===\n");
        var registry = new SoldierRegistry();
        registry.Register("knight", new Soldier("knight"));
        registry.Register("archer", new Soldier("archer"));

        Console.WriteLine("\nSpawning clones (no DB calls):");
        var k1 = registry.Get("knight");
        var k2 = registry.Get("knight");
        k1.Abilities.Add("Rage Mode");
        Console.WriteLine($"Knight 1: {k1}");
        Console.WriteLine($"Knight 2: {k2}");
    }
}
