using System;
using System.Collections.Generic;

// === Builder — Pizza Order System ===

public class Pizza
{
    public string Size { get; set; } = "";
    public string Crust { get; set; } = "";
    public string Sauce { get; set; } = "";
    public List<string> Toppings { get; set; } = new();
    public bool ExtraCheese { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool Delivered { get; set; }

    public override string ToString()
    {
        string cheese = ExtraCheese ? " + EXTRA CHEESE" : "";
        string gf = IsGlutenFree ? " (GF)" : "";
        string method = Delivered ? "Delivery" : "Pickup";
        return $"{Size} {Crust} crust, {Sauce} sauce, [{string.Join(", ", Toppings)}]{cheese}{gf} — {method}";
    }
}

public class PizzaBuilder
{
    private Pizza _pizza = new();

    public PizzaBuilder SetSize(string size) { _pizza.Size = size; return this; }
    public PizzaBuilder SetCrust(string crust) { _pizza.Crust = crust; return this; }
    public PizzaBuilder SetSauce(string sauce) { _pizza.Sauce = sauce; return this; }
    public PizzaBuilder AddTopping(string topping) { _pizza.Toppings.Add(topping); return this; }
    public PizzaBuilder WithExtraCheese() { _pizza.ExtraCheese = true; return this; }
    public PizzaBuilder ForDelivery() { _pizza.Delivered = true; return this; }
    public Pizza Build() => _pizza;
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Builder Pattern ===\n");
        var pizza1 = new PizzaBuilder()
            .SetSize("Large").SetCrust("Stuffed").SetSauce("BBQ")
            .AddTopping("Chicken").AddTopping("Onion").WithExtraCheese().ForDelivery()
            .Build();
        Console.WriteLine($"Pizza 1: {pizza1}");

        var pizza2 = new PizzaBuilder()
            .SetSize("Medium").SetCrust("Thin").SetSauce("Tomato")
            .AddTopping("Mozzarella").AddTopping("Basil")
            .Build();
        Console.WriteLine($"Pizza 2: {pizza2}");
    }
}
