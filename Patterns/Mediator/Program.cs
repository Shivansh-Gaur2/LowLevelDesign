using System;
using System.Collections.Generic;

// === Mediator — Chat Room ===

public interface IChatMediator
{
    void Register(ChatUser user);
    void SendMessage(string message, ChatUser sender);
}

public class ChatUser
{
    public readonly string Name;
    private IChatMediator? _mediator;
    private readonly List<string> _messages = new();

    public ChatUser(string name) { Name = name; }
    public void SetMediator(IChatMediator mediator) => _mediator = mediator;
    public void Send(string message) => _mediator?.SendMessage(message, this);
    public void Receive(string message, string fromUser)
    {
        Console.WriteLine($"  [{fromUser} → {Name}]: {message}");
        _messages.Add(message);
    }
    public List<string> GetMessages() => _messages;
}

public class ChatRoom : IChatMediator
{
    private readonly List<ChatUser> _users = new();
    public void Register(ChatUser user) { _users.Add(user); user.SetMediator(this); }
    public void SendMessage(string message, ChatUser sender)
    {
        foreach (var user in _users)
            if (user != sender) user.Receive(message, sender.Name);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Mediator Pattern ===\n");
        var chatRoom = new ChatRoom();
        var rahul = new ChatUser("Rahul");
        var priya = new ChatUser("Priya");
        var amit = new ChatUser("Amit");

        chatRoom.Register(rahul);
        chatRoom.Register(priya);
        chatRoom.Register(amit);

        rahul.Send("Trip to Goa this weekend?");
        Console.WriteLine();
        priya.Send("I'm in! Book the bus?");
    }
}
