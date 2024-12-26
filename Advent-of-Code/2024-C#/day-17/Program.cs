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

        InstructionPointer = 0;
        while (InstructionPointer < AllInstructions.Count)
        {
            Instruction instruction = AllInstructions[InstructionPointer++];
            int operand = (int)AllInstructions[InstructionPointer++];

            Execute(instruction, operand);            
        }
        Console.WriteLine("hej");
        Console.WriteLine(String.Join(",", Output));
    }


    private static void Solution2()
    {
        const long MaxA = 100_000_000_000_000;
        long B = Register.B;
        long C = Register.C; 

        for (long a = 0; a < MaxA; a++)
        {
            Register.A = a;
            Register.B = B;
            Register.C = C;
            Output = [];
            InstructionPointer = 0;
            while (InstructionPointer < AllInstructions.Count)
            {
                Instruction instruction = AllInstructions[InstructionPointer++];
                int operand = (int)AllInstructions[InstructionPointer++];
                Execute(instruction, operand);
            }
            // Console.WriteLine("A = " + a);
            string outputString = String.Join(",", Output);
            // Console.WriteLine(output);
            string allInstructionsString = String.Join(",", AllInstructions.Select(i => (int)i));
            if (a % 100_000 == 0)
                Console.WriteLine(a);
            if (outputString != allInstructionsString)
                continue;
            
            Console.WriteLine("Answer = " + a);
            break;
        }
    }


    private static void Execute(Instruction instruction, int operand)
    {
        switch (instruction)
        {
            case Instruction.adv:
                long numerator = Register.A;
                int denominator = (int)Math.Pow(2, ComboOperandToValue(operand));
                Register.A = numerator / denominator;
                break;
            case Instruction.bxl:
                Register.B ^= operand;
                break;
            case Instruction.bst:
                Register.B = ComboOperandToValue(operand) % 8;
                break;
            case Instruction.jnz:
                if (Register.A == 0)
                    break;
                InstructionPointer = operand;
                break;
            case Instruction.bxc:
                Register.B ^= Register.C;
                break;
            case Instruction.out_:
                long value = ComboOperandToValue(operand) % 8;
                Output.Add(value);
                break;
            case Instruction.bdv:
                numerator = Register.A;
                denominator = (int)Math.Pow(2, ComboOperandToValue(operand));
                Register.B = numerator / denominator;
                break;
            case Instruction.cdv:
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
        Register.A = int.Parse(InputLines[0].Split(" ").Last());
        Register.B = int.Parse(InputLines[1].Split(" ").Last());
        Register.C = int.Parse(InputLines[2].Split(" ").Last());
        AllInstructions = InputLines.Last()
                                    .Split(" ")
                                    .Last()
                                    .Split(",")
                                    .Select(c => (Instruction)int.Parse(c))
                                    .ToList();
    }

    
    private static string[] InputLines = [];
    private static List<Instruction> AllInstructions = [];

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
        adv,
        bxl,
        bst,
        jnz,
        bxc,
        out_,
        bdv,
        cdv
    }



    
    

}

