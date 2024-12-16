using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;



using Coordinate = (int, int);



namespace day_12;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        AllRegions = FindAllRegions();
        Solution1();
        Solution2();
        
    }
    
    
    private static void Solution1()
    {
        int totalPrice = AllRegions.Sum(region => region.Count * CalcPerimeter(region));
        Console.WriteLine($"The total price is {totalPrice}");
    }

    private static void Solution2()
    {
        int totalDiscountedPrice = AllRegions.Sum(region => region.Count * CountSides(region));
        Console.WriteLine($"The total price (with a bulk discount) is {totalDiscountedPrice}");
    }

    private static int CountSides(HashSet<Coordinate> region)
    {
        int sideCount = region.Sum(plot => GetDiagonalCoords(plot).Count(diag => IsCorner(region, diag, plot)));
        return sideCount;
    }


    private static bool IsCorner(HashSet<Coordinate> region, Coordinate diag, Coordinate plot)
    {
        return IsConvexCorner(region, diag, plot) || IsConcaveCorner(region, diag, plot);
    }


    private static bool IsConvexCorner(HashSet<Coordinate> region, Coordinate diag, Coordinate plot)
    {
        int plotX = plot.Item1;
        int plotY = plot.Item2;
        int diagX = diag.Item1;
        int diagY = diag.Item2;
        return !region.Contains((diagX, plotY)) && !region.Contains((plotX, diagY));
    }


    private static bool IsConcaveCorner(HashSet<Coordinate> region, Coordinate diag, Coordinate plot)
    {
        int plotX = plot.Item1;
        int plotY = plot.Item2;
        int diagX = diag.Item1;
        int diagY = diag.Item2;
        return region.Contains((diagX, plotY)) && region.Contains((plotX, diagY)) && !region.Contains((diagX, diagY));
    }


    private static int CalcPerimeter(HashSet<Coordinate> region)
    {
        int perimeter = region.Sum(plot => GetAdjacentCoords(plot).Count(adj => !region.Contains(adj)));
        return perimeter;
    }


    private static Coordinate[] GetAdjacentCoords(Coordinate coord)
    {
        int coordX = coord.Item1;
        int coordY = coord.Item2;
        Coordinate[] allAdjacentPlots = [
            (coordX,    coordY-1),  // N
            (coordX+1,  coordY),    // E
            (coordX,    coordY+1),  // S
            (coordX-1,  coordY),    // W
        ];
        return allAdjacentPlots;
    }


    private static Coordinate[] GetDiagonalCoords(Coordinate coord)
    {
        int coordX = coord.Item1;
        int coordY = coord.Item2;
        Coordinate[] allDiagonalPlots = [
            (coordX-1,  coordY-1),  // NW
            (coordX+1,  coordY-1),  // NE
            (coordX+1,  coordY+1),  // SE
            (coordX-1,  coordY+1),  // SW
        ];
        return allDiagonalPlots;
    }


    private static List<HashSet<Coordinate>> FindAllRegions()
    {
        List<HashSet<Coordinate>> allRegions = [];
        int yMax = gardenPlots.Count;
        int xMax = gardenPlots.First().Count;
        
        foreach (int y in Enumerable.Range(0, yMax))
            foreach (int x in Enumerable.Range(0, xMax))
            {
                Coordinate coord = (x, y);
                if (allRegions.Any(region => region.Contains(coord)))
                    continue;
                
                var foundRegion = FindRegion(gardenPlots, coord);
                allRegions.Add(foundRegion);
            }
        return allRegions;
    }


    // BFS
    private static HashSet<Coordinate> FindRegion(List<List<char>> gardenPlots, Coordinate startPlot)
    {
        int maxY = gardenPlots.Count-1;
        int maxX = gardenPlots.First().Count-1;
        char plantType = gardenPlots[startPlot.Item2][startPlot.Item1];

        HashSet<Coordinate> region = [];
        region.Add(startPlot);

        Queue<Coordinate> queue = [];
        queue.Enqueue(startPlot);
    
        while (queue.Count > 0)
        {
            Coordinate curr = queue.Dequeue();
            Coordinate[] allAdjacentPlots = GetAdjacentCoords(curr);
            foreach (Coordinate adj in allAdjacentPlots)
            {
                if (region.Contains(adj))
                    continue;
                
                var adjX = adj.Item1;
                var adjY = adj.Item2;
                if (adjX < 0 || adjX > maxX || adjY < 0 || adjY > maxY)
                    continue;
                
                char adjPlantType = gardenPlots[adjY][adjX];
                if (plantType != adjPlantType)
                    continue;
                
                region.Add(adj);
                queue.Enqueue(adj);
            }
        }

        return region;
    }


    private static void Parse()
    {
        gardenPlots = InputLines.Select(line => line.ToCharArray()
                                                    .ToList())
                                .ToList();
    }

    
    private static string[] InputLines = [];
    private static List<List<char>> gardenPlots = [];
    private static List<HashSet<Coordinate>> AllRegions = [];
}
