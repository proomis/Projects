using System.Collections.Frozen;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlTypes;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;




namespace day_09;

class Program
{
    static void Main(string[] args)
    {
        const string inputFile = "input.txt";
        InputLines = File.ReadAllLines(inputFile);
        Solution1();
        Solution2();
    }

    static void PrintDisk(List<int?> disk)
    {
        foreach (int? block in disk)
        {
            if (block == null)
            {
                Console.Write(FreeSpaceIndicator);
                continue;
            }
            Console.Write(block);
        }
        Console.WriteLine();
    }

    static void PrintDisk(List<(int?, int)> disk)
    {
        foreach (var block in disk)
        {
            if (block.Item1 == null)
            {
                Console.Write(string.Concat(Enumerable.Repeat(FreeSpaceIndicator, block.Item2)));
                continue;
            }
            Console.Write(string.Concat(Enumerable.Repeat(block.Item1.ToString(), block.Item2)));
        }
        Console.WriteLine();
    }
    
    private static void Solution1()
    {
        int fileID = 0;
        bool indicatesFileLength = false; // false indicates length of free space
        List<int?> disk = [];
        int blockLength = 0;

        foreach (var line in InputLines)
            foreach (char c in line)
            {
                blockLength = int.Parse(c.ToString());
                indicatesFileLength = !indicatesFileLength;
                if (indicatesFileLength)
                {
                    //unformatedDisk.Append(string.Concat(Enumerable.Repeat(fileID, blockLength)));
                    for (int i = 0; i < blockLength; i++)
                        disk.Add(fileID);
                    
                    fileID++;
                    continue;
                }
                for (int i = 0; i < blockLength; i++)
                    disk.Add(null);
            }
        
        int leftmostFreeSpaceIndex = 0;
        int rightmostFileBlockIndex = disk.Count-1;
    //    move file blocks from the end to the leftmost free space block
        while (true)
        {
            while (disk[leftmostFreeSpaceIndex] != null)
                leftmostFreeSpaceIndex++;
            while (disk[rightmostFileBlockIndex] == null)
                rightmostFileBlockIndex--;
            
            if (leftmostFreeSpaceIndex > rightmostFileBlockIndex)
                break;

            disk[leftmostFreeSpaceIndex] = disk[rightmostFileBlockIndex];
            disk[rightmostFileBlockIndex] = null;
        }

        Int128 checkSum = 0;
        foreach (var it in disk.Select((fileID, index) => new {index, fileID}))
        {
            if (it.fileID == null)
                continue;
            
            checkSum += (Int128)it.index * (Int128)it.fileID;
        }
        Console.WriteLine(checkSum);
    }

    private static void Solution2()
    {
         int fileID = 0;
        bool indicatesFileLength = false; // false indicates length of free space
        List<(int?, int)> disk = []; // fileID, block length. fileID=null means free space
        int blockLength = 0;

        foreach (var line in InputLines)
            foreach (char c in line)
            {
                blockLength = int.Parse(c.ToString());
                indicatesFileLength = !indicatesFileLength;
                if (indicatesFileLength)
                {
                    disk.Add((fileID, blockLength));   
                    fileID++;
                    continue;
                }
                disk.Add((null, blockLength));
            }
        

        for (int right = disk.Count-1; right >= 0; right--)
        {
            if (disk[right].Item1 == null)
                continue;
            
            int? rightFileID = disk[right].Item1;
            int rightFileSize = disk[right].Item2;
            
            for (int left = 0; left < right; left++)
            {
                if (disk[left].Item1 != null)
                    continue;
                
                int freeSpaceSize = disk[left].Item2;
                if (freeSpaceSize >= rightFileSize)
                {
                    disk[left] = (null, freeSpaceSize-rightFileSize);
                    disk.Insert(left, disk[right]);
                    disk[right+1] = (null, rightFileSize);
                    break;
                }
            }
        }

        Int128 checkSum = 0;
        Int128 fileIndex = 0;
        foreach (var block in disk)
        {
            if (block.Item1 == null)
            {
                fileIndex += block.Item2;
                continue;
            }
            
            for (int i = 0; i < block.Item2; i++)
            {
                checkSum += (Int128)block.Item1 * fileIndex;
                fileIndex++;
            }
        }

        Console.WriteLine(checkSum);
    }

    
    private static string[] InputLines = [];
    private static readonly string FreeSpaceIndicator = ".";
}
