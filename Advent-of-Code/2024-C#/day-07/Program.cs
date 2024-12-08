using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;


using Equation = System.Tuple<long, long[]>;


namespace day_07;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        ParseEquations();
        Solution1();
        Solution2();
    }

    
    private static void Solution1()
    {   
        Operator[] additionAndMultiplication = [Operator.Addition, Operator.Multiplication];
        long totalCalibrationResult = AllEquations.Where(eq => CanEquationBeTrue(eq, additionAndMultiplication))
                                                  .Sum(equation => equation.Item1);
        Console.WriteLine($"Total calibration result = {totalCalibrationResult}");
    }
    private static void Solution2()
    {                                                                  
        Operator[] everyOperator = [Operator.Addition, Operator.Multiplication, Operator.Concatenation];               
        long totalCalibrationResult = AllEquations.Where(eq => CanEquationBeTrue(eq, everyOperator))
                                                  .Sum(equation => equation.Item1);
        Console.WriteLine($"Total calibration result = {totalCalibrationResult}");
    }

    private static bool CanEquationBeTrue(Equation equation, Operator[] possibleOperators)
    {
        long lhs = equation.Item1;
        long[] rhs  = equation.Item2;
        List<long> possibleValuesRHS = [];
        foreach (long number in rhs)
        {
            if (possibleValuesRHS.Count == 0)
            {
                possibleValuesRHS.Add(number);
                continue;
            }
            List<long> nextPartialAnswers = [];
            foreach (long partialAnswer in possibleValuesRHS)
            {
                if (possibleOperators.Contains(Operator.Addition))
                    nextPartialAnswers.Add(partialAnswer+number);
                if (possibleOperators.Contains(Operator.Multiplication))
                    nextPartialAnswers.Add(partialAnswer*number);
                if (possibleOperators.Contains(Operator.Concatenation))
                    nextPartialAnswers.Add(long.Parse(partialAnswer.ToString() + number.ToString()));
            }
            possibleValuesRHS = nextPartialAnswers;
        }
        
        return possibleValuesRHS.Contains(lhs);
    }

    private static void ParseEquations()
    {
        AllEquations = [];
        foreach (var line in InputLines)
        {
            string[] bothSides = line.Split(":");
            long lhs = long.Parse(bothSides[0]);
            long[] rhs = bothSides[1].Trim()
                                    .Split(" ")
                                    .Select(n=>long.Parse(n))
                                    .ToArray();
            
            AllEquations.Add(Tuple.Create(lhs, rhs));
        }
    }
    
    private static string[] InputLines = [];
    private static List<Equation> AllEquations = [];

    private enum Operator
    {
        Addition,
        Multiplication,
        Concatenation,
    }

}
