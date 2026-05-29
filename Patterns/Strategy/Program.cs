using System;

// === Strategy — Payment Processing ===

public interface IPaymentStrategy { void Pay(decimal amount); }

public class CreditCardPayment : IPaymentStrategy
{
    private readonly string _cardNumber;
    public CreditCardPayment(string cardNumber) { _cardNumber = cardNumber; }
    public void Pay(decimal amount) => Console.WriteLine($"Charged Rs.{amount} to card ending {_cardNumber[^4..]}");
}

public class UPIPayment : IPaymentStrategy
{
    private readonly string _vpa;
    public UPIPayment(string vpa) { _vpa = vpa; }
    public void Pay(decimal amount) => Console.WriteLine($"UPI: Rs.{amount} sent to {_vpa}");
}

public class CODPayment : IPaymentStrategy
{
    public void Pay(decimal amount) => Console.WriteLine($"COD: Rs.{amount} to be collected on delivery");
}

public class ShoppingCart
{
    private IPaymentStrategy? _strategy;
    public void SetPaymentMethod(IPaymentStrategy strategy) => _strategy = strategy;
    public void Checkout(decimal total)
    {
        if (_strategy == null) throw new InvalidOperationException("Set a payment method first");
        _strategy.Pay(total);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Strategy Pattern ===\n");
        var cart = new ShoppingCart();

        cart.SetPaymentMethod(new UPIPayment("shivansh@upi"));
        cart.Checkout(599);

        cart.SetPaymentMethod(new CreditCardPayment("4242424242421234"));
        cart.Checkout(1299);

        cart.SetPaymentMethod(new CODPayment());
        cart.Checkout(399);
    }
}
