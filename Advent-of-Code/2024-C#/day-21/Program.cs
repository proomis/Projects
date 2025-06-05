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

namespace day_21;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = ExampleInputFileName;
        InputLines = File.ReadAllLines(inputFile);
        Parse();
        Solution1();
        Solution2();
    }


    //wrong answer = 163872   -    too hiigh
    private static void Solution1()
    {
        char[,] numericKeypad =
        {
            { '7', '8', '9' },
            { '4', '5', '6' },
            { '1', '2', '3' },
            { Button.Gap, '0', Button.Activate },
        };

        char[,] directionalKeypad =
        {
            { Button.Gap, Button.Up, Button.Activate },
            { Button.Left, Button.Down, Button.Right },
        };

        // var buttonSequence = GetButtonSequence(numericKeypad, AllCodes.First());
        // Console.WriteLine(string.Join("", buttonSequence));

        int totalCodeComplexity = 0;

        foreach (List<char> code in AllCodes)
        {
            
            var buttonSequence1 = GetButtonSequence(numericKeypad, code);
            var buttonSequence2 = GetButtonSequence(directionalKeypad, buttonSequence1);
            var buttonSequence3 = GetButtonSequence(directionalKeypad, buttonSequence2);

            int numericPartOfCode = int.Parse(string.Join("", code.SkipLast(1)));
            int lengthOfShortestSequence = buttonSequence3.Count;
            int codeComplexity = numericPartOfCode * lengthOfShortestSequence;
            totalCodeComplexity += codeComplexity;

            Console.WriteLine(
                $"{lengthOfShortestSequence} * {numericPartOfCode} = {codeComplexity}"
            );
        }

        Console.WriteLine($"total code complexity = {totalCodeComplexity}");
    }

    private static void Solution2() { }

    private static List<char> GetButtonSequence(char[,] keypad, List<char> targetSequence)
    {
        List<char> buttonSequence = [];
        (int gapX, int gapY) = FindButton(keypad, Button.Gap);
        (int currX, int currY) = FindButton(keypad, Button.Activate);
        foreach (var button in targetSequence)
        {
            (int nextX, int nextY) = FindButton(keypad, button);
            int xDist = nextX - currX;
            char horizontalButton = xDist < 0 ? Button.Left : Button.Right;

            int yDist = nextY - currY;
            char verticalButton = yDist < 0 ? Button.Up : Button.Down;

            // if (currX == gapX && nextY == gapY)
            // {
            //     buttonSequence.AddRange(Enumerable.Repeat(horizontalButton, Math.Abs(xDist)));
            //     buttonSequence.AddRange(Enumerable.Repeat(verticalButton, Math.Abs(yDist)));
            // }
            // else if (currY == gapY && nextX == gapX)
            // {
            //     buttonSequence.AddRange(Enumerable.Repeat(verticalButton, Math.Abs(yDist)));
            //     buttonSequence.AddRange(Enumerable.Repeat(horizontalButton, Math.Abs(xDist)));
            // }
            // else
            // {
            //     buttonSequence.AddRange(Enumerable.Repeat(horizontalButton, Math.Abs(xDist)));
            //     buttonSequence.AddRange(Enumerable.Repeat(verticalButton, Math.Abs(yDist)));
            // }

            buttonSequence.Add(Button.Activate);
            (currX, currY) = (nextX, nextY);
        }

        return buttonSequence;
    }

    private static (int, int) FindButton(char[,] keypad, char button)
    {
        for (int row = 0; row < keypad.GetLength(0); row++)
        for (int col = 0; col < keypad.GetLength(1); col++)
            if (keypad[row, col] == button)
                return (col, row);
        throw new Exception($"could not find button \"{button}\"");
    }

    private static void Parse()
    {
        AllCodes = InputLines.Select(s => s.ToList()).ToList();
    }

    private static string[] InputLines = [];
    private static List<List<char>> AllCodes = [];
    private const string ExampleInputFileName = "ex_input.txt";
    private const string RealInputFileName = "input.txt";

    private static class Button
    {
        public static readonly char Activate = 'A';
        public static readonly char Gap = ' ';
        public static readonly char One = '1';
        public static readonly char Two = '2';
        public static readonly char Three = '3';
        public static readonly char Four = '4';
        public static readonly char Five = '5';
        public static readonly char Six = '6';
        public static readonly char Seven = '7';
        public static readonly char Eight = '8';
        public static readonly char Nine = '9';

        public static readonly char Up = '^';
        public static readonly char Right = '>';
        public static readonly char Down = 'v';
        public static readonly char Left = '<';
    }
}
