using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

using Coordinate2d = System.Tuple<int, int>;


namespace day_08;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        ParseAntennaLocations();
        Solution1();
        Solution2();
    }

    
    private static void Solution1()
    {
        HashSet<Coordinate2d> allAntinodeLocations = [];
        foreach (var antennaFrequency in AllAntennaLocations.Keys)
        {
            var locationsForFrequency = AllAntennaLocations[antennaFrequency];
            foreach (var loc1 in locationsForFrequency)
                foreach (var loc2 in locationsForFrequency)
                {
                    if (loc1 == loc2)
                        continue;
                    
                    int dx = loc2.Item1 - loc1.Item1;
                    int dy = loc2.Item2 - loc1.Item2;
                    Coordinate2d antinodeLoc = new(loc1.Item1-dx, loc1.Item2-dy);
                    if (antinodeLoc.Item1 < 0 || antinodeLoc.Item1 >= InputLines.First().Length || 
                        antinodeLoc.Item2 < 0 || antinodeLoc.Item2 >= InputLines.Length)
                        continue;
                    
                    allAntinodeLocations.Add(antinodeLoc);
                }
        }
        Console.WriteLine($"{allAntinodeLocations.Count} unique locations contain an antinode");
    }


    private static void Solution2()
    {                     
        HashSet<Coordinate2d> allAntinodeLocations = [];
        foreach (var antennaFrequency in AllAntennaLocations.Keys)
        {
            var locationsForFrequency = AllAntennaLocations[antennaFrequency];
            foreach (var loc1 in locationsForFrequency)
                foreach (var loc2 in locationsForFrequency)
                {
                    if (loc1 == loc2)
                        continue;
                    
                    allAntinodeLocations.Add(loc1);

                    int dx = loc2.Item1 - loc1.Item1;
                    int dy = loc2.Item2 - loc1.Item2;
                    Coordinate2d antinodeLoc = new(loc1.Item1-dx, loc1.Item2-dy);
                    while (!(antinodeLoc.Item1 < 0 || antinodeLoc.Item1 >= InputLines.First().Length || 
                             antinodeLoc.Item2 < 0 || antinodeLoc.Item2 >= InputLines.Length))
                    {
                        allAntinodeLocations.Add(antinodeLoc);
                        antinodeLoc = new(antinodeLoc.Item1-dx, antinodeLoc.Item2-dy);
                    }
                }
        }
        Console.WriteLine($"{allAntinodeLocations.Count} unique locations contain an antinode, with resonant harmonics taken into account");                                    
    }

    
    private static void ParseAntennaLocations()
    {
        AllAntennaLocations = [];
        foreach (var verticalIt in InputLines.Select((row, y) => new {row, y}))
            foreach (var horizontalIt in verticalIt.row.Select((character, x) => new {character, x}))
            {
                if (horizontalIt.character == EmptyIndicator)
                    continue;
                
                var antennaLocation = new Coordinate2d(horizontalIt.x, verticalIt.y); 
                var antennaFrequency = horizontalIt.character;

                if (!AllAntennaLocations.ContainsKey(antennaFrequency))
                    AllAntennaLocations[antennaFrequency] = [];
                AllAntennaLocations[antennaFrequency].Add(antennaLocation);
            }
    }
    
    private static string[] InputLines = [];
    private static Dictionary<char, List<Coordinate2d>> AllAntennaLocations = [];
    private static readonly char EmptyIndicator = '.';
}
