using System;
using System.Collections.Generic;

// === Vending Machine — State Pattern ===

public class Product
{
    public readonly string Name;
    public readonly decimal Price;
    public int Quantity;

    public Product(string name, int quantity, decimal price)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }
}

public interface IVendingMachineState
{
    void InsertMoney(decimal amount, VendingMachine machine);
    void SelectProduct(string productName, VendingMachine machine);
    void Cancel(VendingMachine machine);
}

public class IdleState : IVendingMachineState
{
    public void InsertMoney(decimal amount, VendingMachine machine)
    {
        machine.AddMoney(amount);
        Console.WriteLine($"Adding the amount {amount} to the machine, Balance : {machine.GetCurrentBalance()}");
        machine.SetState(new HasMoneyState());
    }

    public void SelectProduct(string productName, VendingMachine machine)
    {
        Console.WriteLine("Need to insert money first");
    }

    public void Cancel(VendingMachine machine)
    {
        Console.WriteLine("No money to return");
    }
}

public class HasMoneyState : IVendingMachineState
{
    public void InsertMoney(decimal amount, VendingMachine machine)
    {
        machine.AddMoney(amount);
        Console.WriteLine($"Added more money {amount} current balance is {machine.GetCurrentBalance()}");
    }

    public void SelectProduct(string productName, VendingMachine machine)
    {
        var products = machine.GetProducts();
        if (!products.ContainsKey(productName))
        {
            Console.WriteLine("Product does not exist");
            return;
        }
        Product product = products[productName];
        if (product.Quantity == 0)
        {
            Console.WriteLine("Out of stock!");
            return;
        }
        if (machine.GetCurrentBalance() < product.Price)
        {
            Console.WriteLine("Not enough balance here");
            return;
        }
        machine.SetState(new DispenseState());
        machine.Dispense(productName);
    }

    public void Cancel(VendingMachine machine)
    {
        Console.WriteLine($"Returning the {machine.GetCurrentBalance()} back to you");
        machine.ResetBalance();
        machine.SetState(new IdleState());
    }
}

public class DispenseState : IVendingMachineState
{
    public void InsertMoney(decimal amount, VendingMachine machine)
    {
        Console.WriteLine("Wait product is dispensing...");
    }

    public void SelectProduct(string productName, VendingMachine machine)
    {
        Console.WriteLine("Wait the machine is dispensing your order...");
    }

    public void Cancel(VendingMachine machine)
    {
        Console.WriteLine("Cancel? No way bud!");
    }
}

public class VendingMachine
{
    private Dictionary<string, Product> _products;
    private decimal _currentBalance;
    private decimal _totalCash;
    private IVendingMachineState _state;

    public VendingMachine()
    {
        _products = new();
        _currentBalance = 0;
        _totalCash = 0;
        _state = new IdleState();
    }

    public decimal GetCurrentBalance() => _currentBalance;
    public void SetState(IVendingMachineState state) => _state = state;
    public void AddMoney(decimal amount) => _currentBalance += amount;
    public Dictionary<string, Product> GetProducts() => _products;
    public void ResetBalance() => _currentBalance = 0;

    public void InsertMoney(decimal money) => _state.InsertMoney(money, this);
    public void SelectProduct(string productName) => _state.SelectProduct(productName, this);
    public void Cancel() => _state.Cancel(this);

    public void Refill(string productName, int quantity, decimal price)
    {
        if (_products.ContainsKey(productName))
            _products[productName].Quantity += quantity;
        else
            _products[productName] = new Product(productName, quantity, price);
    }

    public decimal CollectCash()
    {
        var cash = _totalCash;
        _totalCash = 0;
        return cash;
    }

    public void DisplayProducts()
    {
        foreach (Product product in _products.Values)
            Console.WriteLine($"{product.Name} - Rs.{product.Price} ({product.Quantity} in stock)");
    }

    public void Dispense(string productName)
    {
        var product = _products[productName];
        decimal change = _currentBalance - product.Price;
        _totalCash += product.Price;
        Console.WriteLine($"Dispensing... {productName}.");
        if (change > 0)
            Console.WriteLine($"Returning the change back to you: Rs.{change}");
        _currentBalance = 0;
        product.Quantity--;
        _state = new IdleState();
    }
}

class Program
{
    static void Main()
    {
        var machine = new VendingMachine();
        machine.Refill("Chips", 10, 20);
        machine.Refill("Cold Coffee", 5, 40);
        machine.Refill("Samosa", 8, 15);

        Console.WriteLine("=== Vending Machine ===");
        machine.DisplayProducts();

        Console.WriteLine("\n--- Transaction 1 ---");
        machine.InsertMoney(20);
        machine.InsertMoney(10);
        machine.SelectProduct("Chips");

        Console.WriteLine("\n--- Transaction 2 ---");
        machine.InsertMoney(10);
        machine.SelectProduct("Cold Coffee");
        machine.Cancel();

        Console.WriteLine($"\nTotal cash collected: Rs.{machine.CollectCash()}");
    }
}
