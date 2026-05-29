using System;
using System.Collections.Generic;

// === Observer — Stock Price Alerts ===

public interface IStockObserver { void Update(string stockName, decimal price); }

public class Stock
{
    public string Name { get; }
    private decimal _price;
    private readonly List<IStockObserver> _observers = new();

    public Stock(string name, decimal initialPrice) { Name = name; _price = initialPrice; }
    public void Subscribe(IStockObserver observer) => _observers.Add(observer);
    public void Unsubscribe(IStockObserver observer) => _observers.Remove(observer);

    public decimal Price
    {
        get => _price;
        set { _price = value; foreach (var o in _observers) o.Update(Name, _price); }
    }
}

public class PhoneAlert : IStockObserver
{
    private readonly string _userName;
    public PhoneAlert(string userName) { _userName = userName; }
    public void Update(string stockName, decimal price) =>
        Console.WriteLine($"  [PHONE] {_userName}: {stockName} is now Rs.{price}");
}

public class EmailAlert : IStockObserver
{
    private readonly string _email;
    public EmailAlert(string email) { _email = email; }
    public void Update(string stockName, decimal price) =>
        Console.WriteLine($"  [EMAIL] {_email}: {stockName} is now Rs.{price}");
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Observer Pattern ===\n");
        var tcs = new Stock("TCS", 3500);
        tcs.Subscribe(new PhoneAlert("Rahul"));
        tcs.Subscribe(new EmailAlert("priya@mail.com"));

        tcs.Price = 3650;
        tcs.Price = 3580;
    }
}
