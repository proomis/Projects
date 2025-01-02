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

namespace day_10;

class Program
{
    static void Main()
    {
        const string inputFile = RealInputFileName;
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        Solution1();
        Solution2();
    }

    private static void Solution1()
    {
        int possibleCount = AllDesiredDesigns.Count(IsDesignPossible);
        Console.WriteLine($"{possibleCount} designs are possible");
    }

    private static void Solution2()
    {
        long totalWaysCount = AllDesiredDesigns.Sum(CountWaysToMakeDesign);
        Console.WriteLine(
            $"The sum of the different ways to make each design is {totalWaysCount} "
        );
    }

    private static readonly Dictionary<string, bool> PossibleDesignsCache = [];

    private static bool IsDesignPossible(string design)
    {
        if (PossibleDesignsCache.TryGetValue(design, out bool isDesignPossible))
            return isDesignPossible;

        foreach (string pattern in AllAvailableTowelPatterns)
        {
            if (!design.StartsWith(pattern))
                continue;

            if (pattern == design)
                return PossibleDesignsCache[design] = true;

            string designTail = string.Join("", design.Skip(pattern.Length));
            if (IsDesignPossible(designTail))
                return PossibleDesignsCache[design] = true;
        }
        return PossibleDesignsCache[design] = false;
    }

    private static readonly Dictionary<string, long> WaysToMakeDesignsCache = [];

    private static long CountWaysToMakeDesign(string design)
    {
        if (WaysToMakeDesignsCache.TryGetValue(design, out long waysCount))
            return waysCount;

        WaysToMakeDesignsCache[design] = 0;

        foreach (string pattern in AllAvailableTowelPatterns)
        {
            if (!design.StartsWith(pattern))
                continue;

            if (pattern == design)
            {
                WaysToMakeDesignsCache[design]++;
                continue;
            }

            string designTail = string.Join("", design.Skip(pattern.Length));
            WaysToMakeDesignsCache[design] += CountWaysToMakeDesign(designTail);
        }
        return WaysToMakeDesignsCache[design];
    }

    private static void Parse()
    {
        var allTowelPatternsAndDesiredDesgins = string.Join("\n", InputLines).Split("\n\n");
        AllAvailableTowelPatterns = allTowelPatternsAndDesiredDesgins.First().Split(", ");
        AllDesiredDesigns = allTowelPatternsAndDesiredDesgins.Last().Split("\n");
    }

    private static string[] InputLines = [];
    private static string[] AllAvailableTowelPatterns = [];
    private static string[] AllDesiredDesigns = [];

    private const string ExampleInputFileName = "ex_input.txt";
    private const string RealInputFileName = "input.txt";
}
