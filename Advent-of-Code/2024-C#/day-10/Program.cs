using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;




namespace day_10;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        Solution1();
        Solution2();
    }
    
    private static void Solution1()
    {
        int totalScore = 0;
        foreach (var head in GetTrailheads())
        {
            int score = DFS(head.Item1, head.Item2, true);
            totalScore += score;
        }
        Console.WriteLine($"The sum of the scores of all trailheads is {totalScore}");
    }

    private static void Solution2()
    {
        int totalRating = 0;
        foreach (var head in GetTrailheads())
        {
            int rating = DFS(head.Item1, head.Item2, false);
            totalRating += rating;
        }
        Console.WriteLine($"The sum of the ratings of all trailheads is {totalRating}");
    }

    private static int DFS(int trailheadX, int trailheadY, bool returnScore) // returnScore == false means return Rating instead
    {
        HashSet<(int, int)> visited = [];
        int result = 0;
        int yMax = Map.Count-1;
        int xMax = Map.First().Count-1;
        DFSRecursive(trailheadX, trailheadY);
        return result;

        void DFSRecursive(int x, int y)
        {
            if (returnScore)
                visited.Add((x, y));
            int currentCell = Map[y][x];
            if (currentCell == 9)
            {
                result++;
                return;
            }

            List<(int, int)> neighbors = [];
            if (y-1 >= 0 && Map[y-1][x] == currentCell+1)
                neighbors.Add((x, y-1));
            if (y+1 <= yMax && Map[y+1][x] == currentCell+1)
                neighbors.Add((x, y+1));
            if (x-1 >= 0 && Map[y][x-1] == currentCell+1)
                neighbors.Add((x-1, y));
            if (x+1 <= xMax && Map[y][x+1] == currentCell+1)
                neighbors.Add((x+1, y));
            
            foreach (var n in neighbors)
                if (!visited.Contains(n))
                    DFSRecursive(n.Item1, n.Item2);
        }
    }

    private static List<(int, int)> GetTrailheads()
    {
        List<(int, int)> trailheads = [];
        int yMax = Map.Count-1;
        int xMax = Map.First().Count-1;
        for (int y = 0; y <= yMax; y++)
            for (int x = 0; x <= xMax; x++)  
                if (Map[y][x] == 0)
                    trailheads.Add((x, y));
        
        return trailheads;
    }

    private static void Parse()
    {
        foreach (string inputLine in InputLines)
        {
            List<int> row = [];
            foreach (char height in inputLine)
                row.Add(int.Parse(height.ToString()));
            Map.Add(row);
        }
    }

    
    private static string[] InputLines = [];
    private static readonly List<List<int>> Map = [];
}
