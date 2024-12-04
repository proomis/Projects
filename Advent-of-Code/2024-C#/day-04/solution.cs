

using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;

namespace day_04;

class Program
{
    static void Main(string[] args)
    {
        const string InputFile = "input.txt";
        string[] inputLines = File.ReadAllLines(InputFile);
        Solution1.Solve(inputLines);
        Solution2.Solve(inputLines);
    }
}


static class Solution1
{
    public static void Solve(string[] inputLines)
    {
        int yDim = inputLines.Length;
        int xDim = inputLines.First().Length;
        int XMASCount = 0;
        for (int y = 0; y < yDim; y++)
            for (int x = 0; x < xDim; x++)
                if (inputLines[y][x] == 'X')
                    XMASCount += FindInstancesOfXMAS(inputLines, y, x);
        Console.WriteLine("\"XMAS\" appears " + XMASCount + " times");
    }

     private static int FindInstancesOfXMAS(string[] grid, int startY, int startX)
    {
        int foundXMASCount = 0;   
        for (int dir2dIndex = 0; dir2dIndex < AllDirections2D.GetLength(0); dir2dIndex++)
        {
            var yDir = AllDirections2D[dir2dIndex,0];
            var xDir = AllDirections2D[dir2dIndex,1];
            if (IsXMASInDir(grid, startY, startX, yDir, xDir))
                foundXMASCount++;
        }

        return foundXMASCount;
    }

    private static bool IsXMASInDir(string[] grid, int startY, int startX, int yDir, int xDir)
    {
        const string XMASTail = "MAS";
        int tailLength = XMASTail.Length;
        
        int YDimSize = grid.Length;
        int XDimSize = grid.First().Length;

        for (int stepLength = 1; stepLength <= tailLength; stepLength++)
        {
            int y = stepLength * yDir + startY;
            int x = stepLength * xDir + startX;
            if (y < 0 || y >= YDimSize || x < 0 || x >= XDimSize)
                return false; 
            
            char currLetter   = grid[y][x];
            char targetLetter = XMASTail[stepLength-1];
            if (currLetter != targetLetter)
                return false;
        }

        return true;
    }

    private const int Directions2DCount = 8;
    private static  readonly int[] AllDirections1D = {-1, 0, 1};
    
    private static int[,] AllDirections2D
    {
        get 
        { 
            int[,] directions2D = new int[Directions2DCount,2];
            int i = 0;
            foreach (int yDir in AllDirections1D)
                foreach (int xDir in AllDirections1D)
                {
                    if (yDir == 0 && xDir == 0)
                        continue;
                    directions2D[i,0] = yDir;
                    directions2D[i,1] = xDir;
                    i++;
                }
            return directions2D; 
        }
    }
}


static class Solution2
{
    public static void Solve(string[] inputLines)
    {
        int yDim = inputLines.Length;
        int xDim = inputLines.First().Length;
        int _MAS_AsXcount = 0;
        for (int y = 0; y < yDim; y++)
            for (int x = 0; x < xDim; x++)
                if (Is_MAS_AtCoords(inputLines, x, y))
                    _MAS_AsXcount++;
        Console.WriteLine("\"MAS\" in the shape of an X appears " + _MAS_AsXcount + " times");
    }

    private static bool Is_MAS_AtCoords(string[] grid, int y, int x)
    {
        char letter = grid[y][x];
        if (letter != 'A')
            return false;

        int yMax = grid.Length-1;
        int xMax = grid.First().Length-1;
        if (new int[]{0, yMax}.Contains(y) || new int[]{0, xMax}.Contains(x))
            return false;

        char NW = grid[y-1][x-1];
        char NE = grid[y-1][x+1];
        char SE = grid[y+1][x+1];
        char SW = grid[y+1][x-1];
        var diagonal1 = new HashSet<char> {NW, SE};
        var diagonal2 = new HashSet<char> {NE, SW};

        var startAndEndLetters = new HashSet<char> {'M', 'S'};
        return diagonal1.SetEquals(startAndEndLetters) && diagonal2.SetEquals(startAndEndLetters);
    }
}