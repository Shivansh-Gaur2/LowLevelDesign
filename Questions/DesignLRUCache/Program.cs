using System;
using System.Collections.Generic;

// === LRU Cache — Dictionary + Doubly Linked List ===

public class Node
{
    public string Key;
    public int Value;
    public Node? Prev;
    public Node? Next;

    public Node(string key, int value)
    {
        Key = key;
        Value = value;
    }
}

public class LRUCache
{
    private int _capacity;
    private Dictionary<string, Node> _cache;
    private Node _head;
    private Node? _tail;

    public LRUCache(int capacity)
    {
        _cache = new();
        _capacity = capacity;
        _head = new Node("dummy", -1);
        _tail = _head;
    }

    public void AddToFront(Node node)
    {
        _tail!.Next = node;
        node.Prev = _tail;
        _tail = node;
    }

    public void RemoveNode(Node node)
    {
        node.Prev!.Next = node.Next;
        if (node.Next != null)
            node.Next.Prev = node.Prev;
        if (node == _tail)
            _tail = node.Prev;
    }

    public void MoveToFront(Node node)
    {
        RemoveNode(node);
        AddToFront(node);
    }

    public Node RemoveLast()
    {
        Node evicted = _head.Next!;
        _head.Next = evicted.Next;
        if (evicted.Next != null)
            evicted.Next.Prev = _head;
        if (evicted == _tail)
            _tail = _head;
        return evicted;
    }

    public int Get(string key)
    {
        if (!_cache.ContainsKey(key))
            return -1;
        MoveToFront(_cache[key]);
        return _cache[key].Value;
    }

    public void Put(string key, int value)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key].Value = value;
            MoveToFront(_cache[key]);
            return;
        }
        if (_cache.Count == _capacity)
        {
            Node evicted = RemoveLast();
            _cache.Remove(evicted.Key);
        }
        Node n = new Node(key, value);
        AddToFront(n);
        _cache[key] = n;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== LRU Cache ===");
        var cache = new LRUCache(3);

        cache.Put("rahul", 100);
        cache.Put("priya", 200);
        cache.Put("amit", 300);

        Console.WriteLine($"Get rahul: {cache.Get("rahul")}");   // 100
        cache.Put("neha", 400);  // evicts priya
        Console.WriteLine($"Get priya: {cache.Get("priya")}");   // -1 (evicted)
        Console.WriteLine($"Get amit: {cache.Get("amit")}");     // 300
        Console.WriteLine($"Get neha: {cache.Get("neha")}");     // 400
    }
}
