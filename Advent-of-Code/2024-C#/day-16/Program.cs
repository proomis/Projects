using System.Collections;
using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
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




namespace day_16;

class Program
{
    
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        StartTile    = FindSymbol(Symbol.Start)!;
        EndTile      = FindSymbol(Symbol.End)!;
        Solution1();
        Solution2(); 
    }
    
    private static readonly int MaxCost = int.MaxValue/2;
    private static readonly int MoveForwardCost = 1;
    private static readonly int Rotate90DegCost = 1000;
    private static readonly int Rotate180DegCost = Rotate90DegCost * 2;
    private static void Solution1()
    {
        Path path = FindBestPath(StartTile!, Direction.Right, EndTile!);
        int pathCost = CalcPathCost(path);
        Console.WriteLine($"The lowest cost a reindeer could possibly get is {pathCost}");
    }


    private static void Solution2()
    {
        var allBestPaths = GetAllBestPaths();
        HashSet<IntVector2D> allTilesOnOneOfTheBestPaths = [];
        foreach (var path in allBestPaths)
            foreach (var tile in path)
                allTilesOnOneOfTheBestPaths.Add(tile);
        
        Console.WriteLine($"{allTilesOnOneOfTheBestPaths.Count} tiles are part of at least one of the best paths");
    }

    private class Path : List<IntVector2D>{}


    private static int CalcPathCost(Path path)
    {
        int totalPathCost = 0;
        IntVector2D prevDir = StartDir;
        IntVector2D prevtile = path.First();
        foreach (var tile in path.Skip(1))
        {
            int tileCost = CalcStepCost(prevDir, prevtile, tile);
            totalPathCost += tileCost;
            prevDir = tile-prevtile;
            prevtile = tile;
            // MazeSet(step, GetCorrespondingValue<char>(prevDir));
        }

        return totalPathCost;
    }


    private static Path FindBestPath(IntVector2D startTile, IntVector2D startDir, IntVector2D endTile)
    {
        return FindBestPath(startTile, startDir, endTile, out var _);
    }


    private static Path FindBestPath(IntVector2D startTile, IntVector2D startDir, IntVector2D endTile, out IntVector2D endDir)
    {
        int mazeHeight = Maze.Count;
        int mazeWidth = Maze.First().Count;
        Dictionary<IntVector2D, int> distanceTable = [];
        Dictionary<IntVector2D, IntVector2D?> prev = [];
        PriorityQueue<IntVector2D, int> queue = new();
        
        // init
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                IntVector2D tile = new(x, y);
                prev[tile] = null;
                distanceTable[tile] = MaxCost;
            }
        }

        distanceTable[startTile] = 0;
        queue.Enqueue(startTile, 0);
        
        while (queue.Count > 0)
        {
            var tile = queue.Dequeue();
            if (tile == endTile)
            {
                endDir = tile - prev[tile]!;
                return GetFullPath(prev, endTile);
            }

            foreach (var adj in GetAdjacentTiles(tile))
            {

                IntVector2D dir = tile == startTile ? startDir : tile - prev[tile]!;
                int alt = distanceTable[tile] + CalcStepCost(dir, tile, adj);
                if (alt >= distanceTable[adj])
                    continue;
                
                prev[adj] = tile;
                distanceTable[adj] = alt;
                queue.Enqueue(adj, alt);
            }
        }

        throw new Exception($"ERROR: No path between ({startTile.X}, {startTile.Y}) and ({endTile.X}, {endTile.Y}) exists");
    }


    private static int CalcStepCost(IntVector2D currDirection, IntVector2D tile, IntVector2D adj)
    {
        char adjSymbol = MazeGet(adj);
        if (adjSymbol == Symbol.Wall)
            return MaxCost;
        if (tile + currDirection == adj)
            return 1;
        if (tile - currDirection == adj)
            return Rotate180DegCost + MoveForwardCost;

        return Rotate90DegCost + MoveForwardCost;
    }



    


    private static List<Path> GetAllBestPaths()
    {
        List<Path> allPaths = [];
        int mazeHeight = Maze.Count;
        int mazeWidth = Maze.First().Count;
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                // Console.WriteLine($"{100*(y*mazeWidth+x)/C} %");
                var tile = new IntVector2D(x, y);
                if (MazeGet(tile) == Symbol.Wall)
                    continue;
                if (tile == StartTile || tile == EndTile)
                {
                    Path path = FindBestPath(StartTile!, StartDir, EndTile!);
                    allPaths.Add(path);
                    continue;
                }
                Path pathHead = FindBestPath(StartTile!, StartDir, tile, out IntVector2D headEndDir);
                Path pathTail = FindBestPath(tile, headEndDir, EndTile!);
                pathHead.AddRange(pathTail.Skip(1));
                allPaths.Add(pathHead);
            }
        }

        int lowestPathCost = allPaths.Min(p => CalcPathCost(p));
        List<Path> allBestPaths = allPaths.FindAll(p => CalcPathCost(p) == lowestPathCost);
        return allBestPaths;
    }


    private static Path GetFullPath(Dictionary<IntVector2D, IntVector2D?> prev, IntVector2D? endTile)
    {
        Path result = [];
        
        IntVector2D? curr = endTile;
        while (curr != null)
        {
            result.Add(curr);
            curr = prev[curr];
        }

        result.Reverse();
        return result;
    }
    

    private static List<IntVector2D> GetAdjacentTiles(IntVector2D tile)
    {
        List<IntVector2D> adjacentCoords = [];
        for (int x = -1; x <= 1; x++)
            for (int y = -1; y <= 1; y++)
            {
                if ((x != 0 && y != 0) || (x == 0 && y == 0))
                    continue;

                IntVector2D adjDirection = new(x, y);
                IntVector2D adjCoord = tile+adjDirection;
                if (MazeGet(adjCoord) != Symbol.Wall)
                    adjacentCoords.Add(adjCoord);
            }

        return adjacentCoords;
    }

    
    private static char MazeGet(IntVector2D coord)
    {
        return Maze[coord.Y][coord.X];
    }

    private static void MazeSet(IntVector2D coord, char newSymbol)
    {
        Maze[coord.Y][coord.X] = newSymbol;
    }

    private static void PrintMaze()
    {
        foreach (var row in Maze)
        {
            foreach (var col in row)
            {
                Console.Write(col);
            }
            Console.WriteLine();
        }
    }



    private static IntVector2D? FindSymbol(char symbol)
    {
        foreach (int y in Enumerable.Range(0, Maze.Count))
            foreach (int x in Enumerable.Range(0, Maze.First().Count))
                if (MazeGet(new IntVector2D(x, y)) == symbol)
                    return new IntVector2D(x, y);

        return null;
    }


    
    private static void Parse()
    {
        Maze = InputLines.Select(r => r.ToList()).ToList();
    }

    
    private static string[] InputLines = [];
    
    private static List<List<char>> Maze = [];

    private static IntVector2D? StartTile = null;
    private static IntVector2D? EndTile = null;

    private static readonly IntVector2D StartDir = Direction.Right;


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


    private class Direction
    {
        public static readonly IntVector2D Up = new(0,-1);
        public static readonly IntVector2D Right = new(1,0);
        public static readonly IntVector2D Down = new(0,1);
        public static readonly IntVector2D Left = new(-1,0);
    }

    

    

    private static T GetCorrespondingValue<T>(object key)
    {
        Dictionary<object, object> DirectionMapping = new()
        {
            { DirectionSymbol.Up, Direction.Up },
            { DirectionSymbol.Right, Direction.Right },
            { DirectionSymbol.Down, Direction.Down },
            { DirectionSymbol.Left, Direction.Left },
            { Direction.Up, DirectionSymbol.Up },
            { Direction.Right, DirectionSymbol.Right },
            { Direction.Down, DirectionSymbol.Down },
            { Direction.Left, DirectionSymbol.Left }
        };
        return DirectionMapping.TryGetValue(key, out var value) ? (T)value : throw new KeyNotFoundException();
    }


    private static class Symbol 
    {
        public const char Start = 'S';
        public const char Wall = '#';
        public const char Empty = '.';
        public const char End = 'E';
    }

}
