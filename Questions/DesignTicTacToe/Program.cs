using System;

// === Tic-Tac-Toe — Clean OOP ===

public enum PlayerType { None, X, O }

public class Board
{
    private PlayerType[,] _grid = new PlayerType[3, 3];
    private int _moveCount;

    public Board() { _moveCount = 0; }

    public bool Place(int row, int col, PlayerType player)
    {
        if (row < 0 || row >= 3 || col < 0 || col >= 3 || _grid[row, col] != PlayerType.None)
            return false;
        _grid[row, col] = player;
        _moveCount++;
        return true;
    }

    public bool IsFull() => _moveCount == 9;

    public bool CheckWin(PlayerType player)
    {
        for (int i = 0; i < 3; i++)
        {
            if (_grid[i, 0] == player && _grid[i, 1] == player && _grid[i, 2] == player) return true;
            if (_grid[0, i] == player && _grid[1, i] == player && _grid[2, i] == player) return true;
        }
        if (_grid[0, 0] == player && _grid[1, 1] == player && _grid[2, 2] == player) return true;
        if (_grid[0, 2] == player && _grid[1, 1] == player && _grid[2, 0] == player) return true;
        return false;
    }

    public void Display()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                string symbol = _grid[i, j] == PlayerType.None ? " " : _grid[i, j].ToString();
                Console.Write($" {symbol} ");
                if (j < 2) Console.Write("|");
            }
            Console.WriteLine();
            if (i < 2) Console.WriteLine("-----------");
        }
    }
}

public class Game
{
    private Board _board;
    private PlayerType _currentPlayer;

    public Game()
    {
        _board = new Board();
        _currentPlayer = PlayerType.X;
    }

    public void Play()
    {
        while (true)
        {
            _board.Display();
            Console.Write($"Player {_currentPlayer}, enter row and col (0-2): ");
            string[] parts = Console.ReadLine()!.Split(' ');
            int row = int.Parse(parts[0]);
            int col = int.Parse(parts[1]);

            if (!_board.Place(row, col, _currentPlayer))
            {
                Console.WriteLine("Cannot place here");
                continue;
            }
            if (_board.CheckWin(_currentPlayer))
            {
                _board.Display();
                Console.WriteLine($"Player {_currentPlayer} wins!");
                break;
            }
            if (_board.IsFull())
            {
                _board.Display();
                Console.WriteLine("Game draw!");
                break;
            }
            _currentPlayer = _currentPlayer == PlayerType.X ? PlayerType.O : PlayerType.X;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Tic-Tac-Toe ===");
        var game = new Game();
        game.Play();
    }
}
