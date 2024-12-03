using System.Text.RegularExpressions;

namespace day_03;

class Program
{
    static void Main(string[] args)
    {
        const string InputFile = "input.txt";
        string[] inputLines = File.ReadAllLines(InputFile);
        Solution1(inputLines);
        Solution2(inputLines);
    }

    private const string MulInstructionPattern = @"mul\(\d{1,999},\d{1,999}\)";
    private const string NumberPattern = @"\d+";
    private const string EnableInstructionPattern = @"do\(\)";
    private const string DisableInstructionPattern = @"don't\(\)";

    private static int ExecMulInstruction(string mulInstruction)
    {
        var bothNumbers = Regex.Matches(mulInstruction, NumberPattern);
        int number1 = int.Parse(bothNumbers.First().ToString());
        int number2 = int.Parse(bothNumbers.Last().ToString());
        int product = number1 * number2;
        return product;
    }

    private static void Solution1(string[] inputLines)
    {
        int result = 0;
        foreach (var line in inputLines)
        {
            var validInstructionMatches = Regex.Matches(line, MulInstructionPattern);
            foreach (var match in validInstructionMatches)
            {
                string instruction = match.ToString()!;
                int product = ExecMulInstruction(instruction);
                result += product;
            }
        }
        Console.WriteLine(result);

    }

    private enum InstructionType
    {
        Mul,
        Enable,
        Disable,
    }

    private static PriorityQueue<Tuple<string, InstructionType>, int> stack = new();
    private static void EnqueueInstruction(string instruction, InstructionType type, int index)
    {
        var entry = new Tuple<string, InstructionType>(instruction, type);
        stack.Enqueue(entry, index);
    }

    private static void Solution2(string[] inputLines)
    {
        int result = 0;
        bool isMulInstructionEnabled = true;
        foreach (var line in inputLines)
        {
            var mulInstructionMatches = Regex.Matches(line, MulInstructionPattern);
            var enableInstructionMatches = Regex.Matches(line, EnableInstructionPattern);
            var disableInstructionMatches = Regex.Matches(line, DisableInstructionPattern);
            foreach (Match match in mulInstructionMatches)
                EnqueueInstruction(match.Value, InstructionType.Mul, match.Index);

            foreach (Match match in enableInstructionMatches) 
                EnqueueInstruction(match.Value, InstructionType.Enable, match.Index);

            foreach (Match match in disableInstructionMatches)
                EnqueueInstruction(match.Value, InstructionType.Disable, match.Index);

            while (stack.Count > 0)
            {
                Tuple<string, InstructionType> entry;
                stack.TryDequeue(out entry!, out int instructionIndex);
                string thisInstruction = entry.Item1;
                InstructionType thisInstructionType = entry.Item2;

                switch (thisInstructionType)
                {
                    case InstructionType.Mul:
                        if (!isMulInstructionEnabled)
                            break;
                        int product = ExecMulInstruction(thisInstruction);
                        result += product;
                        break;
                    case InstructionType.Enable:
                        isMulInstructionEnabled = true;
                        break;
                    case InstructionType.Disable:
                        isMulInstructionEnabled = false;
                        break;
                }
            }
        }
        Console.WriteLine(result);
    }
}