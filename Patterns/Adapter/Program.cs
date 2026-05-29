using System;

// === Adapter — SMS Provider Integration ===

public interface ISmsSender { bool Send(string phoneNumber, string message); }

public class QuickSmsSender : ISmsSender
{
    public bool Send(string phoneNumber, string message)
    {
        Console.WriteLine($"[QuickSMS] Sending to {phoneNumber}: {message}");
        return true;
    }
}

public class TwilioClient
{
    private readonly string _accountSid;
    public TwilioClient(string accountSid, string authToken) { _accountSid = accountSid; }
    public TwilioResponse CreateMessage(TwilioMessage message)
    {
        Console.WriteLine($"[Twilio] Sending via SID:{_accountSid} to {message.To}: {message.Body}");
        return new TwilioResponse { Status = "sent", MessageId = Guid.NewGuid().ToString() };
    }
}

public class TwilioMessage { public string To { get; set; } = ""; public string From { get; set; } = ""; public string Body { get; set; } = ""; }
public class TwilioResponse { public string Status { get; set; } = ""; public string MessageId { get; set; } = ""; }

public class TwilioAdapter : ISmsSender
{
    private readonly TwilioClient _client;
    public TwilioAdapter(TwilioClient client) { _client = client; }

    public bool Send(string phoneNumber, string message)
    {
        var msg = new TwilioMessage { To = phoneNumber, Body = message };
        var response = _client.CreateMessage(msg);
        return response.Status == "sent";
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Adapter Pattern ===\n");
        ISmsSender sms = new TwilioAdapter(new TwilioClient("sid123", "token456"));
        sms.Send("+919876543210", "Your OTP is 4523");
    }
}
