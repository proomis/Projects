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




namespace day_11;

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
        long stoneCount = CountStonesAfterNBlinks(25);
        Console.WriteLine(stoneCount);
    }

    private static void Solution2()
    {
        long stoneCount = CountStonesAfterNBlinks(75);
        Console.WriteLine(stoneCount);
    }

    private static long CountStonesAfterNBlinks(int blinkCount)
    {
        Dictionary<long, long> stoneCounter = [];
        foreach (long stone in stoneLine)
        {
            if (!stoneCounter.ContainsKey(stone)) { stoneCounter[stone] = 0; }
            stoneCounter[stone]++;
        }

        foreach (int blink in Enumerable.Range(1, blinkCount))
        {
            Dictionary<long, long> nextStoneCounter = [];
            foreach (long stone in stoneCounter.Keys)
            {
                if (stone == 0)
                {
                    if (!nextStoneCounter.ContainsKey(1)) { nextStoneCounter[1] = 0; }
                    nextStoneCounter[1] += stoneCounter[0];
                    continue;
                }
                
                string stoneString = stone.ToString();
                int stoneDigitCount = stoneString.Length;
                if (long.IsEvenInteger(stoneDigitCount))
                {
                    long newStone1 = long.Parse(stoneString[..(stoneDigitCount / 2)]);
                    long newStone2 = long.Parse(stoneString[(stoneDigitCount / 2)..]);
                    if (!nextStoneCounter.ContainsKey(newStone1)) { nextStoneCounter[newStone1] = 0; }
                    if (!nextStoneCounter.ContainsKey(newStone2)) { nextStoneCounter[newStone2] = 0; }
                    nextStoneCounter[newStone1] += stoneCounter[stone];
                    nextStoneCounter[newStone2] += stoneCounter[stone];
                    continue;
                }

                long newStone = stone*2024;
                if (!nextStoneCounter.ContainsKey(newStone)) { nextStoneCounter[newStone] = 0; }
                nextStoneCounter[newStone] += stoneCounter[stone];
            }
            stoneCounter = nextStoneCounter;    
        }

        long stoneCount = stoneCounter.Sum(x => x.Value);
        return stoneCount;
    }


    private static void Parse()
    {
        stoneLine = InputLines.First().Split(" ").Select(s=>long.Parse(s)).ToList();
    }

    
    private static string[] InputLines = [];
    private static List<long> stoneLine = [];
}
