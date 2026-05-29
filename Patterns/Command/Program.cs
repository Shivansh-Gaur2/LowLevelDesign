using System;
using System.Collections.Generic;

// === Command — Text Editor with Undo/Redo ===

public interface ICommand { void Execute(); void Undo(); }

public class TextEditor
{
    public string Content { get; set; } = "";
    public void Print() => Console.WriteLine($"  Editor: \"{Content}\"");
}

public class InsertCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly string _text;
    private readonly int _position;

    public InsertCommand(TextEditor editor, string text, int position)
    {
        _editor = editor; _text = text; _position = position;
    }

    public void Execute()
    {
        _editor.Content = _editor.Content.Insert(_position, _text);
        Console.WriteLine($"  INSERT \"{_text}\" at {_position}");
    }

    public void Undo()
    {
        _editor.Content = _editor.Content.Remove(_position, _text.Length);
        Console.WriteLine($"  UNDO insert \"{_text}\"");
    }
}

public class DeleteCommand : ICommand
{
    private readonly TextEditor _editor;
    private readonly int _position;
    private readonly int _length;
    private string _deleted = "";

    public DeleteCommand(TextEditor editor, int position, int length)
    {
        _editor = editor; _position = position; _length = length;
    }

    public void Execute()
    {
        _deleted = _editor.Content.Substring(_position, _length);
        _editor.Content = _editor.Content.Remove(_position, _length);
        Console.WriteLine($"  DELETE \"{_deleted}\" from {_position}");
    }

    public void Undo()
    {
        _editor.Content = _editor.Content.Insert(_position, _deleted);
        Console.WriteLine($"  UNDO delete \"{_deleted}\"");
    }
}

public class CommandHistory
{
    private readonly Stack<ICommand> _undo = new();
    private readonly Stack<ICommand> _redo = new();

    public void Execute(ICommand cmd) { cmd.Execute(); _undo.Push(cmd); _redo.Clear(); }
    public void Undo() { if (_undo.Count > 0) { var cmd = _undo.Pop(); cmd.Undo(); _redo.Push(cmd); } }
    public void Redo() { if (_redo.Count > 0) { var cmd = _redo.Pop(); cmd.Execute(); _undo.Push(cmd); } }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Command Pattern ===\n");
        var editor = new TextEditor();
        var history = new CommandHistory();

        history.Execute(new InsertCommand(editor, "Hello", 0));
        history.Execute(new InsertCommand(editor, " World", 5));
        editor.Print();

        history.Undo();
        editor.Print();

        history.Redo();
        editor.Print();
    }
}
