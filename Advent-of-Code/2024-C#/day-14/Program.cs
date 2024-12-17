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
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;





namespace day_14;

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
        const int SimulationTime_s = 100;
        const int SpaceWidth = 101;
        const int SpaceHeight = 103;
        int NWQuadCount = 0;
        int NEQuadCount = 0;
        int SEQuadCount = 0;
        int SWQuadCount = 0;

        foreach (var robot in AllRobots)
        {
            robot.xPos += robot.xVel*SimulationTime_s;
            robot.xPos %= SpaceWidth;
            robot.yPos += robot.yVel*SimulationTime_s;
            robot.yPos %= SpaceHeight;
            
            if (robot.xPos < 0)
                robot.xPos += SpaceWidth;
            if (robot.yPos < 0)
                robot.yPos += SpaceHeight;
            
            if ((robot.xPos < (SpaceWidth/2)) && (robot.yPos < (SpaceHeight/2)))
            {
                NWQuadCount++;
                continue;
            }
            if ((robot.xPos > (SpaceWidth/2)) && (robot.yPos < (SpaceHeight/2)))
            {
                NEQuadCount++;
                continue;
            }
            if ((robot.xPos > (SpaceWidth/2)) && (robot.yPos > (SpaceHeight/2)))
            {
                SEQuadCount++;
                continue;
            }
            if ((robot.xPos < (SpaceWidth/2)) && (robot.yPos > (SpaceHeight/2)))
            {
                SWQuadCount++;
                continue;
            }
        }
        
        int safetyFactor = NWQuadCount * NEQuadCount * SEQuadCount * SWQuadCount;
        Console.WriteLine($"the safety factor is {safetyFactor}");
    }


    private static void Solution2()
    {
        const int SpaceWidth = 101;
        const int SpaceHeight = 103;
        const int MaxSimulationTime_s = SpaceHeight*SpaceWidth;

        for (int step = 1; step < MaxSimulationTime_s; step++)
        {
            Dictionary<(int, int), int> positionCounter = [];
            foreach (var robot in AllRobots)
            {
                robot.xPos += robot.xVel;
                robot.xPos %= SpaceWidth;
                robot.yPos += robot.yVel;
                robot.yPos %= SpaceHeight;
                
                if (robot.xPos < 0)
                    robot.xPos += SpaceWidth;
                if (robot.yPos < 0)
                    robot.yPos += SpaceHeight;
            
                if (!positionCounter.ContainsKey((robot.xPos, robot.yPos)))
                    positionCounter[(robot.xPos, robot.yPos)] = 0;
                positionCounter[(robot.xPos, robot.yPos)]++;
            }

            if (positionCounter.Values.Any(c => c!=1))
                continue;
            
            PrintSpace(SpaceWidth, SpaceHeight);
            Console.Write("\n\n");
            Console.WriteLine($"all robots are at unique locations efter {step} seconds");
            break;
        }
    }



    private static void PrintSpace(int spaceWidth, int spaceHeight)
    {
        for (int y = 0; y < spaceHeight; y++)
        {
            for (int x = 0; x < spaceWidth; x++)
            {
                int c = AllRobots.Count(r => r.xPos == x && r.yPos == y);
                string s = c == 0 ? "." : c.ToString();
                Console.Write(s);
            }
            Console.WriteLine();
        }
    }



    private class Robot
    {
        public int xPos;
        public int yPos;
        public int xVel;
        public int yVel;

    }

    
    private static void Parse()
    {
        Regex pattern = new Regex("[pv=]");
        foreach (var line in InputLines)
        {
            string[][] values = pattern.Replace(line, String.Empty)
                                       .Split(' ')
                                       .Select(s => s.Split(','))
                                       .ToArray();
            var robot = new Robot
            {
                xPos = int.Parse(values[0][0]),
                yPos = int.Parse(values[0][1]),
                xVel = int.Parse(values[1][0]),
                yVel = int.Parse(values[1][1])
            };
            // Console.WriteLine($"{r.xPos} {r.yPos} {r.xVel} {r.yVel}");
            AllRobots.Add(robot);
        }
    }

    
    private static string[] InputLines = [];
    
    private static readonly List<Robot> AllRobots = [];
}
