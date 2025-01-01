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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;

namespace day_18;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = RealInputFileName;
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        (BytesToSimulateCount, GridWidth, GridHeight) =
            inputFile == ExampleInputFileName
                ? (MaxExampleBytesToSimulateCount, ExampleGridWidth, ExampleGridHeight)
                : (MaxRealBytesToSimulateCount, RealGridWidth, RealGridHeight);
        Solution1();
        Solution2();
    }

    private static void Solution1()
    {
        List<List<char>> memorySpace = [];
        foreach (var y in Enumerable.Range(0, GridHeight + 1))
        {
            memorySpace.Add([]);
            foreach (var x in Enumerable.Range(0, GridWidth + 1))
                memorySpace.Last().Add(Symbol.Safe);
        }

        foreach (var (x, y) in AllCoordinates.Take(BytesToSimulateCount))
            memorySpace[y][x] = Symbol.Corrupted;

        (int, int) start = (0, 0);
        (int, int) exit = (GridWidth, GridHeight);
        int? minStepCount = CalcMinStepCount(memorySpace, start, exit);
        if (minStepCount == null)
        {
            Console.WriteLine("ERROR: No answer found");
            return;
        }
        Console.WriteLine(
            $"The minimum number of steps needed to reach the exit is {minStepCount}"
        );
    }

    private static void Solution2()
    {
        List<List<char>> memorySpace = [];
        foreach (var y in Enumerable.Range(0, GridHeight + 1))
        {
            memorySpace.Add([]);
            foreach (var x in Enumerable.Range(0, GridWidth + 1))
                memorySpace.Last().Add(Symbol.Safe);
        }

        (int, int) start = (0, 0);
        (int, int) exit = (GridWidth, GridHeight);

        foreach (var (x, y) in AllCoordinates)
        {
            memorySpace[y][x] = Symbol.Corrupted;
            int? stepCount = CalcMinStepCount(memorySpace, start, exit);
            if (stepCount != null)
                continue;

            Console.WriteLine(
                $"The coordinates of the first byte that will prevent the exit from being reachable from the starting position is {x},{y}"
            );
            return;
        }
        Console.WriteLine("ERROR: No answer found");
    }

    private static int? CalcMinStepCount(
        List<List<char>> memorySpace,
        (int, int) start,
        (int, int) exit
    )
    {
        var queue = new Queue<((int, int), int)>();
        queue.Enqueue((start, 0));
        var visited = new HashSet<(int, int)> { start };

        while (queue.Count > 0)
        {
            var ((x, y), dist) = queue.Dequeue();

            foreach (
                var (nX, nY) in new (int, int)[] { (x, y + 1), (x + 1, y), (x, y - 1), (x - 1, y) }
            )
            {
                if (nX < 0 || nY < 0 || nX > GridWidth || nY > GridHeight)
                    continue;
                if (memorySpace[nY][nX] == Symbol.Corrupted)
                    continue;
                if (visited.Contains((nX, nY)))
                    continue;

                if ((nX, nY) == exit)
                    return (dist + 1);

                visited.Add((nX, nY));
                queue.Enqueue(((nX, nY), dist + 1));
            }
        }
        return null;
    }

    private static void Parse()
    {
        AllCoordinates = InputLines
            .Select(line => line.Split(","))
            .Select(coord => (int.Parse(coord.First()), int.Parse(coord.Last())))
            .ToList();
    }

    private static void PrintMemorySpace(List<List<char>> memorySpace)
    {
        foreach (var row in memorySpace)
        {
            foreach (var symbol in row)
                Console.Write(symbol);
            Console.WriteLine();
        }
    }

    private static string[] InputLines = [];
    private static List<(int, int)> AllCoordinates = [];
    private static int BytesToSimulateCount;
    private const int MaxExampleBytesToSimulateCount = 12;
    private const int MaxRealBytesToSimulateCount = 1024;
    private static int GridWidth;
    private static int GridHeight;
    private const int ExampleGridWidth = 6;
    private const int ExampleGridHeight = 6;
    private const int RealGridWidth = 70;
    private const int RealGridHeight = 70;
    private const string ExampleInputFileName = "ex_input.txt";
    private const string RealInputFileName = "input.txt";

    private static class Symbol
    {
        public static readonly char Safe = '.';
        public static readonly char Corrupted = '#';
    }
}
