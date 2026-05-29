using System;
using System.Linq;

// === Decorator — Data Stream Processing ===

public interface IDataWriter { void Write(string data); }

public class FileDataWriter : IDataWriter
{
    private readonly string _fileName;
    public FileDataWriter(string fileName) { _fileName = fileName; }
    public void Write(string data) => Console.WriteLine($"[FILE] Writing to {_fileName}: \"{data}\"");
}

public class EncryptionDecorator : IDataWriter
{
    private readonly IDataWriter _writer;
    public EncryptionDecorator(IDataWriter writer) { _writer = writer; }
    public void Write(string data)
    {
        string encrypted = new string(data.Reverse().ToArray());
        Console.WriteLine($"[ENCRYPT] \"{encrypted}\"");
        _writer.Write(encrypted);
    }
}

public class CompressionDecorator : IDataWriter
{
    private readonly IDataWriter _writer;
    public CompressionDecorator(IDataWriter writer) { _writer = writer; }
    public void Write(string data)
    {
        string compressed = data.Length > 10 ? data.Substring(0, 10) + "..." : data;
        Console.WriteLine($"[COMPRESS] \"{compressed}\"");
        _writer.Write(compressed);
    }
}

public class LoggingDecorator : IDataWriter
{
    private readonly IDataWriter _writer;
    public LoggingDecorator(IDataWriter writer) { _writer = writer; }
    public void Write(string data)
    {
        Console.WriteLine($"[LOG] Writing {data.Length} chars");
        _writer.Write(data);
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Decorator Pattern ===\n");
        IDataWriter writer = new FileDataWriter("output.txt");
        writer = new EncryptionDecorator(writer);
        writer = new CompressionDecorator(writer);
        writer = new LoggingDecorator(writer);
        writer.Write("Hello World This Is A Long Message");
    }
}
