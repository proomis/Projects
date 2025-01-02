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

namespace day_20;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = RealInputFileName;
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        Solution1();
        Solution2();
    }

    private static void Solution1()
    {
        const int MaxCheatTime_ps = 2;
        Dictionary<int, int> timeSavedTable = BuildTimeSavedTable(MaxCheatTime_ps);
        int cheatCount = timeSavedTable
            .Where(entry => entry.Key >= MinSaveTime)
            .Sum(entry => entry.Value);
        Console.WriteLine(
            $"There are {cheatCount} cheats that save at least {MinSaveTime} picoseconds when cheating for {MaxCheatTime_ps} picoseconds"
        );
    }

    private static void Solution2()
    {
        const int MaxCheatTime_ps = 20;
        Dictionary<int, int> timeSavedTable = BuildTimeSavedTable(MaxCheatTime_ps);
        int cheatCount = timeSavedTable
            .Where(entry => entry.Key >= MinSaveTime)
            .Sum(entry => entry.Value);
        Console.WriteLine(
            $"There are {cheatCount} cheats that save at least {MinSaveTime} picoseconds when cheating for at most {MaxCheatTime_ps} picoseconds"
        );
    }

    private static Dictionary<(int, int), int> BuildDistanceLookupTable()
    {
        Dictionary<(int, int), int> distanceLookupTable = [];
        distanceLookupTable[StartPos] = 0;
        var (x, y) = StartPos;
        while ((x, y) != EndPos)
            foreach (
                var (nX, nY) in new (int, int)[] { (x, y + 1), (x + 1, y), (x, y - 1), (x - 1, y) }
            )
            {
                if (IsRacetrackCoordOOB(nX, nY))
                    continue;
                if (RacetrackGet(nX, nY) == Symbol.Wall)
                    continue;
                if (distanceLookupTable.ContainsKey((nX, nY)))
                    continue;

                distanceLookupTable[(nX, nY)] = distanceLookupTable[(x, y)] + 1;
                (x, y) = (nX, nY);
                break;
            }

        return distanceLookupTable;
    }

    private static Dictionary<int, int> BuildTimeSavedTable(int maxCheatTime_ps)
    {
        Dictionary<int, int> timeSavedTable = [];

        HashSet<((int, int), (int, int))> checkedCheats = [];
        Dictionary<(int, int), int> distanceLookupTable = BuildDistanceLookupTable();

        foreach (var (coord, (x, y)) in distanceLookupTable.Keys.Select(coord => (coord, coord)))
        foreach (var (cheatCoord, (cheatX, cheatY)) in distanceLookupTable.Keys.Select(cheatCoord => (cheatCoord, cheatCoord)))
        {
            int cheatTime_ps = Math.Abs(cheatX - x) + Math.Abs(cheatY - y);
            if (cheatTime_ps > maxCheatTime_ps)
                continue;
            if (cheatTime_ps <= 1)
                continue;
            if (checkedCheats.Contains((coord, cheatCoord)))
                continue;
            checkedCheats.Add((coord, cheatCoord));

            int timeSaved_ps =
                distanceLookupTable[(x, y)] - distanceLookupTable[(cheatX, cheatY)] - cheatTime_ps;
            if (timeSaved_ps <= 0)
                continue;

            if (!timeSavedTable.ContainsKey(timeSaved_ps))
                timeSavedTable[timeSaved_ps] = 0;
            timeSavedTable[timeSaved_ps]++;
        }

        return timeSavedTable;
    }

    private static void Parse()
    {
        Racetrack = InputLines.Select(line => line.ToList()).ToList();
        RacetrackHeight = Racetrack.Count;
        RacetrackWidth = Racetrack.First().Count;
        StartPos = FindSymbol(Symbol.Start);
        EndPos = FindSymbol(Symbol.End);
    }

    private static (int, int) FindSymbol(char targetSymbol)
    {
        for (int y = 0; y < RacetrackHeight; y++)
        for (int x = 0; x < RacetrackWidth; x++)
            if (targetSymbol == RacetrackGet(x, y))
                return (x, y);

        throw new Exception($"Cannot find symbol {targetSymbol}");
    }

    private static void PrintRacetrack()
    {
        foreach (var row in Racetrack)
        {
            foreach (var symbol in row)
                Console.Write(symbol);
            Console.WriteLine();
        }
    }

    private static char RacetrackGet(int x, int y)
    {
        if (IsRacetrackCoordOOB(x, y))
            throw new Exception("out of range");
        return Racetrack[y][x];
    }

    private static void RacetrackSet(int x, int y, char newSymbol)
    {
        if (IsRacetrackCoordOOB(x, y))
            throw new Exception("out of range");
        if (
            !new[]
            {
                Symbol.Track,
                Symbol.Wall,
                Symbol.Start,
                Symbol.End,
                Symbol.Move1,
                Symbol.Move2,
            }.Contains(newSymbol)
        )
            throw new Exception("Invalid symbol");
        Racetrack[y][x] = newSymbol;
    }

    private static bool IsRacetrackCoordOOB(int x, int y)
    {
        return x < 0 || x >= RacetrackWidth || y < 0 || y >= RacetrackHeight;
    }

    private static string[] InputLines = [];
    private const string ExampleInputFileName = "ex_input.txt";
    private const string RealInputFileName = "input.txt";
    private static List<List<char>> Racetrack = [];
    private static int RacetrackWidth;
    private static int RacetrackHeight;
    private static (int, int) StartPos;
    private static (int, int) EndPos;
    private const int MinSaveTime = 100;

    private static class Symbol
    {
        public static readonly char Track = '.';
        public static readonly char Wall = '#';
        public static readonly char Start = 'S';
        public static readonly char End = 'E';
        public static readonly char Move1 = '1';
        public static readonly char Move2 = '2';
    }
}
