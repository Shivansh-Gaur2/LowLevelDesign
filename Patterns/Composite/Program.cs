using System;
using System.Collections.Generic;
using System.Linq;

// === Composite — Shopping Cart (Categories + Items) ===

public interface ICartComponent
{
    string Name { get; }
    decimal GetPrice();
    void Display(int indent = 0);
}

public class CartItem : ICartComponent
{
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }

    public CartItem(string name, decimal price, int quantity = 1)
    {
        Name = name; Price = price; Quantity = quantity;
    }

    public decimal GetPrice() => Price * Quantity;

    public void Display(int indent = 0)
    {
        string pad = new string(' ', indent);
        Console.WriteLine($"{pad}├─ {Name} (x{Quantity}) = Rs.{GetPrice()}");
    }
}

public class CartCategory : ICartComponent
{
    public string Name { get; }
    private readonly List<ICartComponent> _children = new();

    public CartCategory(string name) { Name = name; }
    public void Add(ICartComponent component) => _children.Add(component);
    public decimal GetPrice() => _children.Sum(c => c.GetPrice());

    public void Display(int indent = 0)
    {
        string pad = new string(' ', indent);
        Console.WriteLine($"{pad}┌─ {Name} (subtotal: Rs.{GetPrice()})");
        foreach (var child in _children)
            child.Display(indent + 2);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Composite Pattern ===\n");
        var electronics = new CartCategory("Electronics");
        electronics.Add(new CartItem("Phone Case", 299));
        electronics.Add(new CartItem("USB Cable", 150, 2));

        var groceries = new CartCategory("Groceries");
        groceries.Add(new CartItem("Basmati Rice 5kg", 450));
        groceries.Add(new CartItem("Toor Dal 1kg", 180));

        var myCart = new CartCategory("My Cart");
        myCart.Add(electronics);
        myCart.Add(groceries);
        myCart.Display();
        Console.WriteLine($"\nTotal: Rs.{myCart.GetPrice()}");
    }
}
