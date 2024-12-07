using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace day_06;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Solution1();
        Solution2();
    }
    private static void Solution2()
    {
        var guardPos = FindGuard();
        if (guardPos == null)
        {
            Console.Error.WriteLine("NO GUARD ON MAP");
            return;
        }
        guardDirectionIndex = 0;
        HashSet<Tuple<int, int>> positionsThatCauseLoop = [];
        HashSet<Tuple<int, int>> visitedPositions = [];

        while (true)
        {
            visitedPositions.Add(guardPos);
            var guardDirection = GuardDirections[guardDirectionIndex];
            var nextGuardPos = new Tuple<int, int>(guardPos.Item1 + guardDirection.Item1, guardPos.Item2 + guardDirection.Item2);
            
            string? nextGuardRow = InputLines!.ElementAtOrDefault(nextGuardPos.Item1);
            if (nextGuardRow == default(string))
                break;
            
            char? nextGuardCell = nextGuardRow.ElementAtOrDefault(nextGuardPos.Item2);
            if (nextGuardCell == default(char))
                break;

            if (nextGuardCell == obstacleIndicator)
            {
                guardDirectionIndex = (guardDirectionIndex+1) % GuardDirections.Length;
                continue;    
            }

            if (nextGuardCell == guardIndicator)
            {
                // Console.WriteLine("can't place obstacle at guard's starting position");
                guardPos = nextGuardPos;
                continue;  // can't place obstacle at guard's starting position anyway.
            }

            if (visitedPositions.Contains(nextGuardPos))
            {
                guardPos = nextGuardPos;
                continue;
            }
            
            int rayDirectionIndex = (guardDirectionIndex+1)%GuardDirections.Length;
            var rayPos = guardPos;
            var tempObstaclePos = nextGuardPos;
            
            List<char> origRow = InputLines![tempObstaclePos.Item1].ToList();
            origRow[tempObstaclePos.Item2] = obstacleIndicator;
            InputLines[tempObstaclePos.Item1] = String.Join("", origRow);
            
            HashSet<Tuple<int, int, int, int>> visitedStates = [];

            while (true)
            {
                var rayDirection = GuardDirections[rayDirectionIndex];
                var state = new Tuple<int, int, int, int>(rayPos.Item1, rayPos.Item2, rayDirection.Item1, rayDirection.Item2);
                
                if (visitedStates.Contains(state))
                {   
                    positionsThatCauseLoop.Add(tempObstaclePos);
                    break;
                }

                visitedStates.Add(state);
                var nextRayPos = new Tuple<int, int>(rayPos.Item1 + rayDirection.Item1, rayPos.Item2 + rayDirection.Item2);
                
                string? nextRayRow = InputLines.ElementAtOrDefault(nextRayPos.Item1);
                if (nextRayRow == default(string))
                    break;
                
                char? nextRayCell = nextRayRow.ElementAtOrDefault(nextRayPos.Item2);
                if (nextRayCell == default(char))
                    break;
                
                if (nextRayCell == obstacleIndicator)
                {
                    rayDirectionIndex = (rayDirectionIndex+1) % GuardDirections.Length;
                    continue;
                } 
                    
                rayPos = nextRayPos;
            }
            
            List<char> newRow = InputLines[tempObstaclePos.Item1].ToList();
            newRow[tempObstaclePos.Item2] = floorIndicator;
            InputLines[tempObstaclePos.Item1] = String.Join("", newRow);
        
            guardPos = nextGuardPos;
        }
        Console.WriteLine($"An obstacle can be placed in {positionsThatCauseLoop.Count} distinct positions to cause a loop");
    }

    private static void Solution1()
    {        
        var guardPos = FindGuard();
        if (guardPos == null)
        {
            Console.Error.WriteLine("NO GUARD ON MAP");
            return;
        }

        guardDirectionIndex = 0;
        HashSet<Tuple<int, int>> visitedPositions = [];
        while (true)
        {
            visitedPositions.Add(guardPos);
            var guardDirection = GuardDirections[guardDirectionIndex];
            var nextGuardPos = new Tuple<int, int>(guardPos.Item1 + guardDirection.Item1, guardPos.Item2 + guardDirection.Item2);
            
            string? nextGuardRow = InputLines!.ElementAtOrDefault(nextGuardPos.Item1);
            if (nextGuardRow == default(string))
                break;
            
            char? nextGuardCell = nextGuardRow.ElementAtOrDefault(nextGuardPos.Item2);
            if (nextGuardCell == default(char))
                break;

            // turn, maybe corner of obstacles
            while (nextGuardCell == obstacleIndicator)
            {
                guardDirectionIndex = (guardDirectionIndex+1) % GuardDirections.Length;
                guardDirection = GuardDirections[guardDirectionIndex];
                nextGuardPos = new Tuple<int, int>(guardPos.Item1 + guardDirection.Item1, guardPos.Item2 + guardDirection.Item2);
                nextGuardCell = InputLines![nextGuardPos.Item1][nextGuardPos.Item2];
            }

            guardPos = nextGuardPos;
        }
        Console.WriteLine($"The guard will visit {visitedPositions.Count} distinct positions");

    }

    private static Tuple<int, int>? FindGuard()
    {
        for (int y = 0; y < InputLines!.Length; y++)
            for (int x = 0; x < InputLines.First().Length; x++)
                    if (InputLines[y][x] == guardIndicator)
                        return new Tuple<int, int>(y, x);
        return null;
    }
    
    private static string[]? InputLines;
    private static int guardDirectionIndex = 0;

    private static readonly Tuple<int, int>[] GuardDirections = {
        new(-1, 0), // North
        new(0, 1), // E
        new(1, 0), // S
        new(0, -1) // W
    };

    private static readonly char guardIndicator = '^';
    private static readonly char obstacleIndicator = '#';
    private static readonly char floorIndicator = '.';

}
