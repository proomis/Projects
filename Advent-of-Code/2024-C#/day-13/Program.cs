using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;





namespace day_13;

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
    private const int CostA = 3;
    private const int CostB = 1;
    
    private static void Solution1()
    {
        BigInteger totalMinCost = CalcMinCost(0);
        Console.WriteLine($"The fewest tokens you would have to spend is {totalMinCost}");
    }


    private static void Solution2()
    {
        BigInteger UnitConversionErrorTerm = 10000000000000;
        BigInteger totalMinCost = CalcMinCost(UnitConversionErrorTerm);
        Console.WriteLine($"The fewest tokens you would have to spend is {totalMinCost}");
    }


    private static BigInteger CalcMinCost(BigInteger unitConversionErrorTerm)
    {
        BigInteger totalMinCost = 0;

        foreach (var machine in AllClawMachines)
        {
            BigInteger targetX = machine.prizeX+unitConversionErrorTerm;
            BigInteger targetY = machine.prizeY+unitConversionErrorTerm;        
            (BigInteger a, var aRem) = BigInteger.DivRem(targetX*machine.bY - targetY*machine.bX, machine.aX*machine.bY - machine.aY*machine.bX);
            (BigInteger b, var bRem) = BigInteger.DivRem(targetX-machine.aX*a, machine.bX);

            if (aRem != 0 || bRem != 0)
                continue;

            BigInteger minCost = CostA * a + CostB * b;
            totalMinCost += minCost;
        }
        return totalMinCost;
    }


    private class ClawMachine
    {
        public int aX;
        public int aY;
        public int bX;
        public int bY;
        public int prizeX;
        public int prizeY;

    }

    
    private static void Parse()
    {
        static (int, int) parseLine(string line)
        {
            if (line == "")
                return (0, 0);
            int[] bothNumbers = line.Split(": ")
                          .Last()
                          .Replace(",", string.Empty)
                          .Split(" ")
                          .Select(s => int.Parse(s.Replace("+", " ")
                                                  .Replace("=", " ")
                                                  .Split(" ")
                                                  .Last()))
                          .ToArray()[..2];

            (int, int) xy = (bothNumbers.First(), bothNumbers.Last());
            return xy;
        }

        var currClawMachine = new ClawMachine();
        foreach (int lineIndex in Enumerable.Range(0, InputLines.Length))
        {
            string line = InputLines[lineIndex];
            (int, int) xy = parseLine(line);
            
            switch (lineIndex%4)
            {
                case 0:
                    currClawMachine.aX = xy.Item1;
                    currClawMachine.aY = xy.Item2;
                    break;
                case 1:
                    currClawMachine.bX = xy.Item1;
                    currClawMachine.bY = xy.Item2;
                    break;
                case 2:
                    currClawMachine.prizeX = xy.Item1;
                    currClawMachine.prizeY = xy.Item2;
                    break;
                case 3:
                    AllClawMachines.Add(currClawMachine);
                    currClawMachine = new ClawMachine();
                    break;
            }

        }
        AllClawMachines.Add(currClawMachine);
    }

    
    private static string[] InputLines = [];
    
    private static readonly List<ClawMachine> AllClawMachines = [];
}
