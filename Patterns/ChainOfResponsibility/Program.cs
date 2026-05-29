using System;

// === Chain of Responsibility — Support Ticket Escalation ===

public enum Severity { Low, Medium, High, Critical }

public class SupportTicket
{
    public string Description { get; }
    public Severity Severity { get; }
    public SupportTicket(string description, Severity severity) { Description = description; Severity = severity; }
}

public abstract class SupportHandler
{
    private SupportHandler? _next;
    public SupportHandler SetNext(SupportHandler next) { _next = next; return next; }
    public virtual void Handle(SupportTicket ticket)
    {
        if (_next != null) _next.Handle(ticket);
        else Console.WriteLine($"  No handler available for: {ticket.Description}");
    }
}

public class L1Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Severity == Severity.Low) Console.WriteLine($"  L1 resolved: {ticket.Description}");
        else base.Handle(ticket);
    }
}

public class L2Support : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Severity == Severity.Medium) Console.WriteLine($"  L2 resolved: {ticket.Description}");
        else base.Handle(ticket);
    }
}

public class L3Engineering : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Severity == Severity.High) Console.WriteLine($"  L3 Engineering fixed: {ticket.Description}");
        else base.Handle(ticket);
    }
}

public class ManagerHandler : SupportHandler
{
    public override void Handle(SupportTicket ticket)
    {
        if (ticket.Severity == Severity.Critical) Console.WriteLine($"  Manager escalated: {ticket.Description}");
        else base.Handle(ticket);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Chain of Responsibility ===\n");
        var l1 = new L1Support();
        var l2 = new L2Support();
        var l3 = new L3Engineering();
        var mgr = new ManagerHandler();
        l1.SetNext(l2).SetNext(l3).SetNext(mgr);

        l1.Handle(new SupportTicket("Reset my password", Severity.Low));
        l1.Handle(new SupportTicket("Billing overcharge", Severity.Medium));
        l1.Handle(new SupportTicket("Data corruption in prod DB", Severity.High));
        l1.Handle(new SupportTicket("Security breach detected", Severity.Critical));
    }
}
