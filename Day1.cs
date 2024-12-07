using System;

public class ShortestDistance()
{
    public void DetermineShortestDistance(int[] arrayOne, int[] arrayTwo)
    {
        Console.WriteLine("Calculating shortest distance...");

        Array.Sort(arrayOne);
        Array.Sort(arrayTwo);

        var distance = 0;
        for (int i = 0; i < arrayOne.Length; i++)
        {
            distance += Math.Abs(arrayOne[i] - arrayTwo[i]);
        }

        Console.WriteLine("Shortest distance is: " + distance);
    }

    public void determineSimilarity(int[] arrayOne, int[] arrayTwo)
    {
        var dict1 = new Dictionary<int, int>();
        var dict2 = new Dictionary<int, int>();
        for (int i = 0; i < arrayOne.Length; i++)
        {
            if (dict1.ContainsKey(arrayOne[i]))
            {
                dict1[arrayOne[i]]++;
            }
            else
            {
                dict1[arrayOne[i]] = 1;
            }

            if (dict2.ContainsKey(arrayTwo[i]))
            {
                dict2[arrayTwo[i]]++;
            }
            else
            {
                dict2[arrayTwo[i]] = 1;
            }
        }

        var similarirtyScore = 0;
        foreach (var kvp in dict1)
        {
            if (dict2.ContainsKey(kvp.Key))
            {
                similarirtyScore += kvp.Key * kvp.Value * dict2[kvp.Key];
            }
        }

        Console.WriteLine("Similarity score is: " + similarirtyScore);
    }

    public void RunCommand()
    {
        Console.WriteLine("Reading file...");
        string[] lines = File.ReadAllLines("day1.csv");
        var arrayOne = new int[lines.Length];
        var arrayTwo = new int[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            var parsedData = Array.ConvertAll(
                lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries),
                int.Parse
            );
            arrayOne[i] = parsedData[0];
            arrayTwo[i] = parsedData[1];
        }

        determineSimilarity(arrayOne, arrayTwo);
    }
}
