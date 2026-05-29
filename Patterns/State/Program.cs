using System;

// === State — Order Lifecycle ===

public interface IOrderState
{
    void Ship(OrderContext order);
    void Deliver(OrderContext order);
    void Cancel(OrderContext order);
}

public class OrderContext
{
    private IOrderState _state;
    public OrderContext() { _state = new PlacedState(); }
    public void SetState(IOrderState state) { _state = state; Console.WriteLine($"  -> State: {state.GetType().Name}"); }
    public void Ship() => _state.Ship(this);
    public void Deliver() => _state.Deliver(this);
    public void Cancel() => _state.Cancel(this);
}

public class PlacedState : IOrderState
{
    public void Ship(OrderContext order) { Console.WriteLine("Order is shipped!"); order.SetState(new ShippedState()); }
    public void Deliver(OrderContext order) { throw new InvalidOperationException("Cannot deliver — not shipped yet"); }
    public void Cancel(OrderContext order) { Console.WriteLine("Order cancelled."); order.SetState(new CancelledState()); }
}

public class ShippedState : IOrderState
{
    public void Ship(OrderContext order) { throw new InvalidOperationException("Already shipped"); }
    public void Deliver(OrderContext order) { Console.WriteLine("Order delivered!"); order.SetState(new DeliveredState()); }
    public void Cancel(OrderContext order) { throw new InvalidOperationException("Cannot cancel — in transit"); }
}

public class CancelledState : IOrderState
{
    public void Ship(OrderContext order) { throw new InvalidOperationException("Cancelled"); }
    public void Deliver(OrderContext order) { throw new InvalidOperationException("Cancelled"); }
    public void Cancel(OrderContext order) { throw new InvalidOperationException("Already cancelled"); }
}

public class DeliveredState : IOrderState
{
    public void Ship(OrderContext order) { throw new InvalidOperationException("Already delivered"); }
    public void Deliver(OrderContext order) { throw new InvalidOperationException("Already delivered"); }
    public void Cancel(OrderContext order) { throw new InvalidOperationException("Already delivered"); }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== State Pattern ===\n");
        var order = new OrderContext();
        order.Ship();
        order.Deliver();

        Console.WriteLine();
        var order2 = new OrderContext();
        order2.Cancel();

        try { order2.Ship(); }
        catch (InvalidOperationException e) { Console.WriteLine($"  Error: {e.Message}"); }
    }
}
