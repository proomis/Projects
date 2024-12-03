namespace day_01;

class Program
{
    static void Main(string[] args)
    {
        const string InputFile = "input.txt";
        string[] inputLines = File.ReadAllLines(InputFile);
        Solution1(inputLines);
        Solution2(inputLines);
    }

    static void Solution1(string[] inputLines)
    {
        List<int> list1 = new List<int>();
        List<int> list2 = new List<int>();
        foreach (string line in inputLines)
        {
            string[] bothLocationIDs = line.Split("   ");
            int locationID1 = int.Parse(bothLocationIDs.First());
            int locationID2 = int.Parse(bothLocationIDs.Last());

            list1.Add(locationID1);
            list2.Add(locationID2);
        }
        list1.Sort();
        list2.Sort();
        var allDistances = list1.Zip(list2, (first, second) => Math.Abs(first - second));
        int totalDistance = allDistances.Sum();
        Console.WriteLine("Total Distance = " + totalDistance);
    }

    static void Solution2(string[] inputLines)
    {
        List<int> list1 = new List<int>();
        List<int> list2 = new List<int>();
        foreach (string line in inputLines)
        {
            string[] bothLocationIDs = line.Split("   ");
            int locationID1 = int.Parse(bothLocationIDs.First());
            int locationID2 = int.Parse(bothLocationIDs.Last());

            list1.Add(locationID1);
            list2.Add(locationID2);
        }
        
        int totalSimilarityScore = 0;
        Dictionary<int, int> cache = new Dictionary<int, int>();
        foreach (var locationID in list1)
        {
            int count = cache.ContainsKey(locationID) ? cache[locationID] : list2.Where(x => x.Equals(locationID)).Count();
            cache[locationID] = count;
            int similarityScore = locationID * count;
            totalSimilarityScore += similarityScore;

        }
        Console.WriteLine("Total Similarity Score = " + totalSimilarityScore);
    }
}