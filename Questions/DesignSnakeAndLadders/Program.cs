using System;
using System.Collections.Generic;

// === Snake & Ladders — Clean OOP ===

public class Dice
{
    private Random _random = new();
    public int Roll() => _random.Next(1, 7);
}

public class Player
{
    public string Name;
    public int Position;
    public Player(string name) { Name = name; Position = 0; }
}

public class Board
{
    public Dictionary<int, int> Snakes = new();
    public Dictionary<int, int> Ladders = new();

    public void AddSnake(int head, int tail) => Snakes[head] = tail;
    public void AddLadder(int bottom, int top) => Ladders[bottom] = top;

    public int GetEndPosition(int position)
    {
        if (Snakes.ContainsKey(position)) return Snakes[position];
        if (Ladders.ContainsKey(position)) return Ladders[position];
        return position;
    }
}

public class Game
{
    public Board Board;
    public Dice Dice;
    public List<Player> Players;
    public int CurrentPlayerIndex;

    public Game(Board board, List<Player> players)
    {
        Board = board;
        Players = players;
        CurrentPlayerIndex = 0;
        Dice = new Dice();
    }

    public void PlayTurn()
    {
        int roll = Dice.Roll();
        Player player = Players[CurrentPlayerIndex];
        int oldPos = player.Position;

        if (player.Position + roll > 100)
        {
            Console.WriteLine($"{player.Name} rolls {roll}: {oldPos} -> can't move (would exceed 100)");
            return;
        }

        player.Position += roll;
        int afterMove = player.Position;
        player.Position = Board.GetEndPosition(player.Position);

        if (player.Position != afterMove)
        {
            string type = player.Position > afterMove ? "LADDER" : "SNAKE";
            Console.WriteLine($"{player.Name} rolls {roll}: {oldPos} -> {afterMove} -> {type} -> {player.Position}");
        }
        else
        {
            Console.WriteLine($"{player.Name} rolls {roll}: {oldPos} -> {player.Position}");
        }
    }

    public bool IsGameOver() => Players[CurrentPlayerIndex].Position == 100;

    public void Play()
    {
        while (true)
        {
            PlayTurn();
            if (IsGameOver())
            {
                Console.WriteLine($"{Players[CurrentPlayerIndex].Name} reaches 100! {Players[CurrentPlayerIndex].Name} wins!!");
                break;
            }
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Snake & Ladders ===\n");
        var board = new Board();
        board.AddSnake(27, 5);
        board.AddSnake(40, 3);
        board.AddSnake(72, 12);
        board.AddSnake(98, 56);

        board.AddLadder(3, 22);
        board.AddLadder(28, 76);
        board.AddLadder(50, 97);
        board.AddLadder(15, 35);

        var players = new List<Player> { new Player("Rahul"), new Player("Priya") };
        var game = new Game(board, players);
        game.Play();
    }
}
