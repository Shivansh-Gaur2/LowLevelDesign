using System;
using System.Collections.Generic;
using System.Linq;

// === Elevator System — Strategy Pattern ===

public enum Direction { UP, DOWN, IDLE }
public enum DoorState { OPEN, CLOSED }

public class Request
{
    public int Floor;
    public Direction Direction;
    public bool IsExternal;
}

public class Elevator
{
    public int Id;
    public int CurrentFloor;
    public Direction Direction;
    public DoorState Door;
    public SortedSet<int> UpStops;
    public SortedSet<int> DownStops;

    public Elevator(int id)
    {
        Id = id;
        CurrentFloor = 0;
        UpStops = new();
        DownStops = new();
        Direction = Direction.IDLE;
        Door = DoorState.CLOSED;
    }

    public void AddStop(int floor)
    {
        if (floor == CurrentFloor) return;

        if (Direction == Direction.UP)
        {
            if (floor >= CurrentFloor) UpStops.Add(floor);
            else DownStops.Add(floor);
        }
        else if (Direction == Direction.DOWN)
        {
            if (floor <= CurrentFloor) DownStops.Add(floor);
            else UpStops.Add(floor);
        }
        else
        {
            if (floor > CurrentFloor)
            {
                UpStops.Add(floor);
                Direction = Direction.UP;
            }
            else
            {
                DownStops.Add(floor);
                Direction = Direction.DOWN;
            }
        }
    }

    public void Step()
    {
        if (Direction == Direction.IDLE) return;

        if (Direction == Direction.UP && UpStops.Contains(CurrentFloor))
        {
            UpStops.Remove(CurrentFloor);
            OpenDoor();
            Console.WriteLine($"  Elevator {Id}: STOPPED at floor {CurrentFloor}");
            CloseDoor();
        }
        else if (Direction == Direction.DOWN && DownStops.Contains(CurrentFloor))
        {
            DownStops.Remove(CurrentFloor);
            OpenDoor();
            Console.WriteLine($"  Elevator {Id}: STOPPED at floor {CurrentFloor}");
            CloseDoor();
        }

        if (Direction == Direction.UP)
        {
            if (UpStops.Count > 0) CurrentFloor++;
            else if (DownStops.Count > 0) { Direction = Direction.DOWN; CurrentFloor--; }
            else Direction = Direction.IDLE;
        }
        else if (Direction == Direction.DOWN)
        {
            if (DownStops.Count > 0) CurrentFloor--;
            else if (UpStops.Count > 0) { Direction = Direction.UP; CurrentFloor++; }
            else Direction = Direction.IDLE;
        }
    }

    public void OpenDoor() => Door = DoorState.OPEN;
    public void CloseDoor() => Door = DoorState.CLOSED;
    public bool IsIdle() => Direction == Direction.IDLE;
}

public interface IElevatorSelectionStrategy
{
    Elevator SelectElevator(List<Elevator> elevators, Request request);
}

public class NearestElevatorStrategy : IElevatorSelectionStrategy
{
    public Elevator SelectElevator(List<Elevator> elevators, Request request)
    {
        Elevator? best = null;
        int minDistance = int.MaxValue;

        foreach (var elevator in elevators)
        {
            bool isIdleOrSameDirection =
                elevator.Direction == Direction.IDLE ||
                (elevator.Direction == request.Direction &&
                 (request.Direction == Direction.UP ? elevator.CurrentFloor <= request.Floor
                                                    : elevator.CurrentFloor >= request.Floor));

            if (isIdleOrSameDirection)
            {
                int distance = Math.Abs(elevator.CurrentFloor - request.Floor);
                if (distance < minDistance) { best = elevator; minDistance = distance; }
            }
        }

        if (best == null)
        {
            foreach (var elevator in elevators)
            {
                int distance = Math.Abs(elevator.CurrentFloor - request.Floor);
                if (distance < minDistance) { best = elevator; minDistance = distance; }
            }
        }
        return best!;
    }
}

public class ElevatorController
{
    public List<Elevator> Elevators;
    public IElevatorSelectionStrategy Strategy;
    public int NumFloors;

    public ElevatorController(int numElevators, int numFloors, IElevatorSelectionStrategy strategy)
    {
        NumFloors = numFloors;
        Strategy = strategy;
        Elevators = new List<Elevator>();
        for (int i = 0; i < numElevators; i++)
            Elevators.Add(new Elevator(i));
    }

    public void HandleExternalRequest(int floor, Direction dir)
    {
        var req = new Request { Floor = floor, Direction = dir, IsExternal = true };
        var best = Strategy.SelectElevator(Elevators, req);
        Console.WriteLine($"External request: floor {floor} {dir} -> Elevator {best.Id}");
        best.AddStop(floor);
    }

    public void HandleInternalRequest(int elevatorId, int floor)
    {
        Console.WriteLine($"Internal request: Elevator {elevatorId} -> floor {floor}");
        Elevators[elevatorId].AddStop(floor);
    }

    public void Step()
    {
        foreach (var elevator in Elevators)
            elevator.Step();
    }

    public void Display()
    {
        foreach (var elevator in Elevators)
        {
            string up = elevator.UpStops.Count > 0 ? string.Join(",", elevator.UpStops) : "none";
            string down = elevator.DownStops.Count > 0 ? string.Join(",", elevator.DownStops) : "none";
            Console.WriteLine($"Elevator {elevator.Id}: floor {elevator.CurrentFloor}, {elevator.Direction}, up=[{up}], down=[{down}]");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Elevator System ===\n");
        var controller = new ElevatorController(2, 10, new NearestElevatorStrategy());

        controller.HandleExternalRequest(1, Direction.UP);
        controller.HandleInternalRequest(0, 7);
        controller.HandleExternalRequest(4, Direction.UP);

        Console.WriteLine();
        for (int i = 0; i < 8; i++)
        {
            controller.Step();
            controller.Display();
            Console.WriteLine();
        }
    }
}
