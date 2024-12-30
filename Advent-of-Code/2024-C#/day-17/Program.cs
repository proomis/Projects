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

namespace day_17;

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
        Output = [];

        List<Instruction> program = AllInstructions
            .SelectMany(pair => new[] { pair.Item1, (Instruction)pair.Item2 })
            .ToList();
        InstructionPointer = 0;
        while (InstructionPointer < AllInstructions.Count)
        {
            Instruction instruction = AllInstructions[InstructionPointer].Item1;
            int operand = AllInstructions[InstructionPointer].Item2;
            Execute(instruction, operand);
            InstructionPointer++;
        }

        string outputString = String.Join(",", Output);
        Console.WriteLine(
            "If you use commmas to join the values the program output into a single string you get "
                + outputString
        );
    }

    private static void Solution2()
    {
        // hard-coded to my given input
        static long? findLowestInitialA(List<int> program, long answer)
        {
            if (program.Count == 0)
                return answer;

            long a = 0;
            long b = 0;
            long c = 0;
            for (long i = 0; i < 8; i++)
            {
                a = (answer << 3) | i;
                b = a % 8;
                b ^= 1;
                c = a >> (int)b;
                b ^= 5;
                b ^= c;
                long last = b % 8;
                if (last != (long)program.Last())
                    continue;

                var subAnswer = findLowestInitialA(program.SkipLast(1).ToList(), a);
                if (subAnswer == null)
                    continue;

                return subAnswer;
            }

            return null;
        }

        List<int> program = AllInstructions
            .SelectMany(pair => new[] { (int)pair.Item1, pair.Item2 })
            .ToList();
        long? lowestA = findLowestInitialA(program, 0);
        Console.WriteLine(
            "The lowest initial value for register A that causes the program to output a copy of itself is "
                + lowestA
        );
    }

    private static void Execute(Instruction instruction, int operand)
    {
        switch (instruction)
        {
            case Instruction.ADV:
                long numerator = Register.A;
                long denominator = (long)Math.Pow(2, ComboOperandToValue(operand));
                Register.A = numerator / denominator;
                break;
            case Instruction.BXL:
                Register.B ^= operand;
                break;
            case Instruction.BST:
                Register.B = ComboOperandToValue(operand) % 8;
                break;
            case Instruction.JNZ:
                if (Register.A == 0)
                    break;
                InstructionPointer = operand;
                break;
            case Instruction.BXC:
                Register.B ^= Register.C;
                break;
            case Instruction.OUT:
                long value = ComboOperandToValue(operand) % 8;
                Output.Add(value);
                break;
            case Instruction.BDV:
                numerator = Register.A;
                denominator = (long)Math.Pow(2, ComboOperandToValue(operand));
                Register.B = numerator / denominator;
                break;
            case Instruction.CDV:
                numerator = Register.A;
                denominator = (int)Math.Pow(2, ComboOperandToValue(operand));
                Register.C = numerator / denominator;
                break;

            default:
                throw new Exception("ERROR: BAD INSTRUCTION");
        }
    }

    private static void Parse()
    {
        Register.A = long.Parse(InputLines[0].Split(" ").Last());
        Register.B = long.Parse(InputLines[1].Split(" ").Last());
        Register.C = long.Parse(InputLines[2].Split(" ").Last());
        AllInstructions = InputLines
            .Last()
            .Split(" ")
            .Last()
            .Split(",")
            .Chunk(2)
            .Select(pair => ((Instruction)int.Parse(pair.First()), int.Parse(pair.Last())))
            .ToList();
    }

    private static string[] InputLines = [];
    private static List<(Instruction, int)> AllInstructions = [];

    private static int InstructionPointer;

    private static long ComboOperandToValue(int comboOperand)
    {
        return comboOperand switch
        {
            0 or 1 or 2 or 3 => comboOperand,
            4 => Register.A,
            5 => Register.B,
            6 => Register.C,
            7 => throw new Exception("ERROR: COMBO OPERAND 7 IS RESERVED"),
            _ => throw new Exception("ERROR: BAD COMBO OPERAND"),
        };
    }

    private static List<long> Output = [];

    private static class Register
    {
        public static long A;
        public static long B;
        public static long C;
    }

    private enum Instruction
    {
        ADV,
        BXL,
        BST,
        JNZ,
        BXC,
        OUT,
        BDV,
        CDV,
    }
}
