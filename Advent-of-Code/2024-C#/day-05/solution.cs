using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic;


namespace day_05;

class Program
{
    static void Main(string[] args)
    {
        const string InputFile = "input.txt";
        ParseInput(InputFile);
        Solution1();
        Solution2();
    }

    private  static Dictionary<int, List<int>>? AllRules;
    private static List<List<int>>? AllUpdates;

    private static void ParseInput(string inputFile)
    {
        string[] inputLines = File.ReadAllLines(inputFile);
        string[] allRulesAndUpdatesLines = String.Join('\n', inputLines).Split("\n\n");
        string[] allRuleLines = allRulesAndUpdatesLines[0].Split('\n');
        string[] allUpdateLines = allRulesAndUpdatesLines[1].Split('\n');
        AllRules = ParseAllRules(allRuleLines);
        AllUpdates = ParseAllUpdates(allUpdateLines); 
    }

    private static Dictionary<int, List<int>> ParseAllRules(string[] allRuleLines)
    {
        Dictionary<int, List<int>> allRules = [];
        foreach (string rule in allRuleLines)
        {
            string[] bothPages = rule.Split('|');
            int page1 = int.Parse(bothPages[0]);
            int page2 = int.Parse(bothPages[1]);
            if (!allRules.ContainsKey(page1))
                allRules[page1] = [];
            allRules[page1].Add(page2);         //  eg.  23|43  and 23|78   =>  {23:[43, 78]}
        }
        return allRules;
    }

    private static List<List<int>> ParseAllUpdates(string[] allUpdateLines)
    {
        List<List<int>> allUpdates = []; 
        foreach (var update in allUpdateLines)
        {
            var allPageNumbersInUpdate = update.Split(',').Select(int.Parse).ToList();
            allUpdates.Add(allPageNumbersInUpdate);
        }
        return allUpdates;
    }

    private static int SortAccordingToAllRules(int page1, int page2)
    {
        if (!AllRules!.ContainsKey(page2))
            return -1;
        
        #pragma warning disable CA1854 // weird warning, this is what I want to do
        if (!AllRules!.ContainsKey(page1))
            return 1;
        
        if (AllRules![page1].Contains(page2))
            return -1;
        
        return 1;
    }

    private static void Solution1()
    {
        int sumMiddlePagesOfOrderedUpdates = 0;
        foreach (var update in AllUpdates!)
        {
            if (!IsUpdateInOrder(update, AllRules!))
                continue;        

            int middlePageIndex = update.Count/2;
            int middlePageValue = update[middlePageIndex];
            sumMiddlePagesOfOrderedUpdates += middlePageValue;
        }

        Console.WriteLine($"Sum of middle page numbers of correctly-ordered updates = {sumMiddlePagesOfOrderedUpdates}");
    }

    private static void Solution2()
    {
        int sumMiddlePagesOfUnorderedUpdates = 0;
        foreach (var update in AllUpdates!)
        {
            if (IsUpdateInOrder(update, AllRules!))
                continue;        
        
            update.Sort(SortAccordingToAllRules);
            int middlePageIndex = update.Count/2;
            int middlePageValue = update[middlePageIndex];
            sumMiddlePagesOfUnorderedUpdates += middlePageValue;
        }

        Console.WriteLine($"Sum of middle page numbers of INcorrectly-ordered updates, after ordering them = {sumMiddlePagesOfUnorderedUpdates}");
    }

    static bool IsUpdateInOrder(List<int> update, Dictionary<int, List<int>> allRules)
    {
        foreach (var pageNumber in update)
        {
            if (!allRules.ContainsKey(pageNumber))
                continue;       // beforePage has only ever appeared as the "after page" of any rule
            
            var allAfterPageNumbers = allRules[pageNumber];
            foreach (var afterPageNumber in allAfterPageNumbers)
            {
                if (!update.Contains(afterPageNumber))
                    continue;   // ignore rules that are irrelevant to the current update

                int afterPageIndex = update.IndexOf(afterPageNumber);
                int beforePageIndex = update.IndexOf(pageNumber);
                if (afterPageIndex < beforePageIndex)
                    return false;
            }
        }

        return true;
    }
}


