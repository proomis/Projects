using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;





namespace day_15;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        RobotPos = FindRobot();
        if (RobotPos == null)
        {
            Console.WriteLine("ERROR: ROBOT NOT FOUND");
            return;
        }
        Solution1();
        Solution2();
    }
    
    private static void Solution1()
    {
        foreach (char move in AllMoves)
        {
            IntVector2D? moveDirection = DirectionSymbolToDirection(move);
            if (moveDirection == null)
            {
                Console.WriteLine("ERROR: BAD MOVE");
                return;
            }
            PerformMove(moveDirection);
        }

        int sum = CalcSumGPS();
        Console.WriteLine("the sum of all boxes' GPS coordinates is " + sum);
    }


    private static void Solution2()
    {
        ResizeMap();
        RobotPos = FindRobot();
        foreach (char move in AllMoves)
        {
            IntVector2D? moveDirection = DirectionSymbolToDirection(move);
            if (moveDirection == null)
            {
                Console.WriteLine("ERROR: BAD MOVE");
                return;
            }
            PerformMoveWide(moveDirection);
        }

        int sum = CalcSumGPS();
        Console.WriteLine("the sum of all boxes' GPS coordinates in the scaled-up warehouse is " + sum);
    }


    private static int CalcSumGPS() 
    {
        int sum = 0;
        for (int y = 1; y < WarehouseMap.Count-1; y++)
            for (int x = 1; x < WarehouseMap.First().Count; x++)
                if (MapGet(new(x, y)) == Symbol.Box || MapGet(new(x, y)) == Symbol.WideBoxLeftHalf)
                    sum += y*100+x;

        return sum;
    }


    private static void PerformMoveWide(IntVector2D moveDirection)
    {
        int xMax = WarehouseMap.First().Count-1;
        int yMax = WarehouseMap.Count-1;
        
        IntVector2D nextRobotPos = new(RobotPos!.X+moveDirection.X, RobotPos.Y+moveDirection.Y);
        IntVector2D peekPos = new(nextRobotPos);
        List<IntVector2D> positionsToCheck = [];
        Stack<IntVector2D> boxesToMove = [];
        
        positionsToCheck.Add(new IntVector2D(nextRobotPos));
        

        if (moveDirection.Y != 0)
        {   
            for (int positionIdx = 0; positionIdx < positionsToCheck.Count; positionIdx++)
            {
                IntVector2D position = positionsToCheck.ElementAt(positionIdx);
                char symbol = MapGet(position);
                switch (symbol)
                {
                    case Symbol.WideBoxLeftHalf:
                        if (!boxesToMove.Contains(position))
                            boxesToMove.Push(new IntVector2D(position));
                        var otherHalf = new IntVector2D(position.X+1, position.Y);
                        if (!boxesToMove.Contains(otherHalf))
                            boxesToMove.Push(otherHalf);
                        var nextBox = new IntVector2D(position + moveDirection);
                        if (!positionsToCheck.Contains(nextBox))
                            positionsToCheck.Add(new IntVector2D(nextBox));
                        var otherHalfNext = new IntVector2D(position.X+1, position.Y+moveDirection.Y);
                        if (!positionsToCheck.Contains(otherHalfNext))
                            positionsToCheck.Add(new IntVector2D(otherHalfNext));
                        break;
                    case Symbol.WideBoxRightHalf:
                        if (!boxesToMove.Contains(position))
                            boxesToMove.Push(new IntVector2D(position));
                        otherHalf = new IntVector2D(position.X-1, position.Y);
                        if (!boxesToMove.Contains(otherHalf))
                            boxesToMove.Push(otherHalf);
                        nextBox = new IntVector2D(position + moveDirection);
                        if (!positionsToCheck.Contains(nextBox))
                            positionsToCheck.Add(new IntVector2D(nextBox));
                        otherHalfNext = new IntVector2D(position.X-1, position.Y+moveDirection.Y);
                        if (!positionsToCheck.Contains(otherHalfNext))
                            positionsToCheck.Add(new IntVector2D(otherHalfNext));
                        break;
                    case Symbol.Wall:
                        return;
                    case Symbol.Empty:
                        break;
                    case Symbol.Robot:
                        Console.WriteLine("ERROR: DUBLICATE ROBOT FOUND");
                        return;
                    default:
                        Console.WriteLine("DEFAULT");
                        return;
                }   
            }

            while (boxesToMove.Count > 0)
            {
                IntVector2D oldBoxPos = new(boxesToMove.Pop());
                IntVector2D newBoxPos = new(oldBoxPos.X, oldBoxPos.Y+moveDirection.Y);
                char boxSymbol = MapGet(oldBoxPos); // could be right or left half of a wide box
                MapSet(newBoxPos, boxSymbol);
                MapSet(oldBoxPos, Symbol.Empty);
            }
            MapSet(nextRobotPos, Symbol.Robot);
            MapSet(    RobotPos, Symbol.Empty);
            RobotPos = nextRobotPos;
            return;
        }
        

        while (true)
        {
            char symbol = MapGet(peekPos);
            switch (symbol)
            {
                case Symbol.WideBoxLeftHalf:
                case Symbol.WideBoxRightHalf:
                    boxesToMove.Push(new IntVector2D(peekPos.X, peekPos.Y));
                    break;
                case Symbol.Robot:
                    Console.WriteLine("ERROR: DUBLICATE ROBOT FOUND");
                    return;
                case Symbol.Wall:
                    return;
                case Symbol.Empty:
                    while (boxesToMove.Count > 0)
                    {
                        IntVector2D temp = boxesToMove.Pop();
                        MapSet(peekPos, MapGet(temp));
                        peekPos = temp;
                    }
                    MapSet(nextRobotPos, Symbol.Robot);
                    MapSet(    RobotPos, Symbol.Empty);
                    RobotPos = nextRobotPos;
                    return;
                default:
                    Console.WriteLine("ERROR: BAD SYMBOL");
                    return;
            }

            peekPos.Y += moveDirection.Y;
            peekPos.X += moveDirection.X;
        }
    }


    private static void PerformMove(IntVector2D moveDirection)
    {
        
        int xMax = WarehouseMap.First().Count-1;
        int yMax = WarehouseMap.Count-1;
        IntVector2D nextRobotPos = new(RobotPos!.X+moveDirection.X, RobotPos.Y+moveDirection.Y);
        IntVector2D peekPos = new(nextRobotPos.X, nextRobotPos.Y);
        
        while (true)
        {
            char symbol = MapGet(peekPos);
            switch (symbol)
            {
                case Symbol.Box:
                    break;
                case Symbol.Robot:
                    Console.WriteLine("ERROR: DUBLICATE ROBOT FOUND");
                    return;
                case Symbol.Wall:
                    return;
                case Symbol.Empty:
                    MapSet(     peekPos, Symbol.Box);
                    MapSet(nextRobotPos, Symbol.Robot);
                    MapSet(    RobotPos, Symbol.Empty);
                    RobotPos = nextRobotPos;
                    return;
                default:
                    Console.WriteLine("ERROR: BAD SYMBOL");
                    return;
            }

            peekPos.Y += moveDirection.Y;
            peekPos.X += moveDirection.X;
        }
    }

    
    private static char MapGet(IntVector2D coord)
    {
        return WarehouseMap[coord.Y][coord.X];
    }

    private static void MapSet(IntVector2D coord, char newSymbol)
    {
        WarehouseMap[coord.Y][coord.X] = newSymbol;
    }


    private static void ResizeMap()
    {
        List<List<char>> newMap = [];
        foreach (var row in WarehouseMap)
        {
            List<char> newRow = [];
            foreach (char c in row)
            {
                switch (c)
                {
                    case Symbol.Wall:
                        newRow.AddRange(Symbol.Wall, Symbol.Wall);
                        break;
                    case Symbol.Empty:
                        newRow.AddRange(Symbol.Empty, Symbol.Empty);
                        break;
                    case Symbol.Box:
                        newRow.AddRange(Symbol.WideBoxLeftHalf, Symbol.WideBoxRightHalf);
                        break;
                    case Symbol.Robot:
                        newRow.AddRange(Symbol.Robot, Symbol.Empty);
                        break;
                    default:
                        break;
                }
            }
            newMap.Add(newRow);
        }
        WarehouseMap = newMap;
    }


    private static void PrintMap()
    {
        WarehouseMap.ForEach(row => Console.WriteLine(String.Join("", row)));
    }

    


    private static IntVector2D? FindRobot()
    {
        foreach (int y in Enumerable.Range(0, WarehouseMap.Count))
            foreach (int x in Enumerable.Range(0, WarehouseMap.First().Count))
                if (WarehouseMap[y][x] == Symbol.Robot)
                    return new IntVector2D(x, y);

        return null;
    }


    
    private static void Parse()
    {
        var mapAndMoves = String.Join("\n", InputLines).Split("\n\n").Select(x => x.Split("\n"));
        WarehouseMap = mapAndMoves.First().Select(r => r.ToList()).ToList();
        AllMoves = String.Join("", mapAndMoves.Last()); 
    }

    
    private static string[] InputLines = [];
    
    private static List<List<char>> WarehouseMap = [];
    private static string AllMoves = "";

    private static IntVector2D? RobotPos = null;

    

    private class IntVector2D
    {
        public int X;
        public int Y;
        public IntVector2D(int x, int y)
        {
            X = x;
            Y = y;
        }

        public IntVector2D(IntVector2D other)
        {
            X = other.X;
            Y = other.Y;
        }

        public static IntVector2D operator+ (IntVector2D a, IntVector2D b) => new(a.X+b.X, a.Y+b.Y);
        public static IntVector2D operator- (IntVector2D a, IntVector2D b) => new(a.X-b.X, a.Y-b.Y);
        public static bool operator == (IntVector2D? a, IntVector2D? b)
        {
            if (a is null)
                return b is null;
            return a.Equals(b);
        } 
            

        public static bool operator != (IntVector2D? a, IntVector2D? b)
        {
            if (a is null)
                return b is not null;
            return !a.Equals(b);
        } 
         // override object.Equals
        public override bool Equals(object? obj)
        {   
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            IntVector2D other = (IntVector2D)obj;
            return X == other.X && Y == other.Y;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

    }

    private class DirectionSymbol
    {
        public const char Up = '^';
        public const char Right = '>';
        public const char Down = 'v';
        public const char Left = '<';
    }

    


    private static IntVector2D? DirectionSymbolToDirection(char directionSymbol)
    {
        return directionSymbol switch
        {
            DirectionSymbol.Up      => new IntVector2D(0, -1),
            DirectionSymbol.Right   => new IntVector2D(1, 0),
            DirectionSymbol.Down    => new IntVector2D(0, 1),
            DirectionSymbol.Left    => new IntVector2D(-1, 0),
            _ => null,
        };
    }


    private static class Symbol 
    {
        public const char Robot = '@';
        public const char Wall = '#';
        public const char Empty = '.';
        public const char Box = 'O';

        public const char WideBoxLeftHalf = '[';
        public const char WideBoxRightHalf = ']';

    }




}
