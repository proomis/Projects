namespace day_02;

class Program
{
    static void Main(string[] args)
    {
        const string InputFile = "input.txt";
        string[] inputLines = File.ReadAllLines(InputFile);
        
        Solution1(inputLines);
        Solution2(inputLines);
    }

    static bool IsSafe(List<int> report)
    {
        for (int i = 0; i < report.Count-1; i++)
        {
            int? prev_level   = i > 0 ? report[i-1] : null;
            int current_level = report[i];
            int next_level    = report[i+1];
            int difference    = Math.Abs(current_level-next_level);
            
            if (difference == 0 || difference > 3)
                return false;
            if (prev_level == null)
                continue;
            if (prev_level < current_level && current_level > next_level)
                return false;
            if (prev_level > current_level && current_level < next_level)
                return false;
        }
        return true;
    }

    static bool CanBeMadeSafe(List<int> report)
    {
        for (int i = 0; i < report.Count; i++)
        {
            List<int> dampenedReport = [.. report];
            dampenedReport.RemoveAt(i);
            if (IsSafe(dampenedReport))
                return true;
        }

        return false;
    }

    static void Solution1(string[] inputLines)
    {
        int safeReportsCount = 0;
        foreach (var line in inputLines)
        {
            List<int> report = line.Split(' ').Select(x => int.Parse(x)).ToList();
            if (IsSafe(report))
                safeReportsCount++;
        }
        Console.WriteLine(safeReportsCount + " reports are safe");
    }

    static void Solution2(string[] inputLines)
    {
        int safeReportsCount = 0;
        foreach (var line in inputLines)
        {
            List<int> report = line.Split(' ').Select(x => int.Parse(x)).ToList();
            if (!IsSafe(report) && !CanBeMadeSafe(report))
                continue;
            safeReportsCount++;
        }
        Console.WriteLine(safeReportsCount + " reports are safe");
    }
}
