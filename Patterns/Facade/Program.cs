using System;

// === Facade — Hotel Booking System ===

public class HotelInventory
{
    public bool IsAvailable(string roomType, DateTime date)
    {
        Console.WriteLine($"  Checking {roomType} for {date:yyyy-MM-dd}...");
        return true;
    }
}

public class PaymentGateway
{
    public bool Charge(string card, decimal amount)
    {
        Console.WriteLine($"  Charging {amount:C} to {card}...");
        return true;
    }
}

public class NotificationService
{
    public void SendEmail(string email, string message) => Console.WriteLine($"  Email to {email}: {message}");
    public void SendSms(string phone, string message) => Console.WriteLine($"  SMS to {phone}: {message}");
}

public class BookingFacade
{
    private readonly HotelInventory _inventory = new();
    private readonly PaymentGateway _payment = new();
    private readonly NotificationService _notify = new();

    public bool BookRoom(string roomType, DateTime checkIn, string cardNumber, string email, string phone)
    {
        Console.WriteLine($"Booking {roomType}...");
        if (!_inventory.IsAvailable(roomType, checkIn)) return false;
        if (!_payment.Charge(cardNumber, 5000m)) return false;
        _notify.SendEmail(email, $"Booking confirmed: {roomType}");
        _notify.SendSms(phone, "Your room is booked!");
        return true;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Facade Pattern ===\n");
        var facade = new BookingFacade();
        facade.BookRoom("Deluxe", DateTime.Today.AddDays(7), "4242-xxxx", "test@mail.com", "+919876543210");
    }
}
