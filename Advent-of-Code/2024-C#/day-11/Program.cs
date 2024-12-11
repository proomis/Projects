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
        int blinkCount = 25;
        BigInteger stoneCount = CountStonesAfterNBlinks(blinkCount);
        Console.WriteLine($"number of stones after {blinkCount} blinks = {stoneCount}");
    }

    private static void Solution2()
    {
        int blinkCount = 75;
        BigInteger stoneCount = CountStonesAfterNBlinks(blinkCount);
        Console.WriteLine($"number of stones after {blinkCount} blinks = {stoneCount}");
    }

    private static BigInteger CountStonesAfterNBlinks(int blinkCount)
    {
        Dictionary<BigInteger, BigInteger> stoneCounter = [];
        foreach (BigInteger stone in stoneLine)
        {
            if (!stoneCounter.ContainsKey(stone)) { stoneCounter[stone] = 0; }
            stoneCounter[stone]++;
        }

        foreach (int blink in Enumerable.Range(1, blinkCount))
        {
            Dictionary<BigInteger, BigInteger> nextStoneCounter = [];
            foreach (BigInteger stone in stoneCounter.Keys)
            {
                if (stone == 0)
                {
                    if (!nextStoneCounter.ContainsKey(1)) { nextStoneCounter[1] = 0; }
                    nextStoneCounter[1] += stoneCounter[0];
                    continue;
                }
                
                string stoneString = stone.ToString();
                int stoneDigitCount = stoneString.Length;
                if (BigInteger.IsEvenInteger(stoneDigitCount))
                {
                    BigInteger newStone1 = BigInteger.Parse(stoneString[..(stoneDigitCount / 2)]);
                    BigInteger newStone2 = BigInteger.Parse(stoneString[(stoneDigitCount / 2)..]);
                    if (!nextStoneCounter.ContainsKey(newStone1)) { nextStoneCounter[newStone1] = 0; }
                    if (!nextStoneCounter.ContainsKey(newStone2)) { nextStoneCounter[newStone2] = 0; }
                    nextStoneCounter[newStone1] += stoneCounter[stone];
                    nextStoneCounter[newStone2] += stoneCounter[stone];
                    continue;
                }

                BigInteger newStone = stone*2024;
                if (!nextStoneCounter.ContainsKey(newStone)) { nextStoneCounter[newStone] = 0; }
                nextStoneCounter[newStone] += stoneCounter[stone];
            }
            stoneCounter = nextStoneCounter;    
        }

        BigInteger stoneCount = stoneCounter.Values.Aggregate( (a, b) => a + b);
        return stoneCount;
    }


    private static void Parse()
    {
        stoneLine = InputLines.First().Split(" ").Select(s=>BigInteger.Parse(s)).ToList();
    }

    
    private static string[] InputLines = [];
    private static List<BigInteger> stoneLine = [];
}
