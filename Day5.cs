public class Day5
{
    public void RunCommand()
    {
        var (rules, printOrders) = ParseData();

        var middleTotal = 0;
        // Solve Print Puzzle
        foreach (var printOrder in printOrders)
        {
            var successfulPrint = false;
            var visitedNodes = new HashSet<int>();
            var printList = printOrder.Item1.ToList();
            for (int i = 0; i < printList.Count; i++)
            {
                var page = printList[i];
                // If this node has a prerequiste AND that page is in the print
                if (rules.ContainsKey(page) && rules[page].Any(x => visitedNodes.Contains(x)))
                {
                    // Which rule failed, fix that issue.
                    var failedRules = rules[page].Where(visitedNodes.Contains).ToList();

                    // All these numbers must be after this number.
                    // Find the index that is closest to the front of the list
                    var closestIndex = failedRules.Select(x => printList.IndexOf(x)).Min();

                    printList.Remove(page);
                    printList.Insert(closestIndex, page);

                    i = -1;
                    visitedNodes.Clear();

                    successfulPrint = true;
                }

                visitedNodes.Add(page);
            }

            if (successfulPrint)
            {
                middleTotal += CalculatePrintNumber(printList.ToArray());
            }
        }

        Console.WriteLine($"Middle Total: {middleTotal}");
    }

    private int CalculatePrintNumber(int[] printOrder)
    {
        Console.WriteLine(string.Join(',', printOrder));
        var index = (int)Math.Floor(printOrder.Length / 2.0);
        // Find the middle value
        return printOrder[index];
    }

    private (Dictionary<int, HashSet<int>>, List<(int[], Dictionary<int, int>)>) ParseData()
    {
        var rules = new Dictionary<int, HashSet<int>>();
        var printOrders = new List<(int[], Dictionary<int, int>)>();

        using (var reader = new StreamReader("Advent 2024\\day5.data"))
        {
            string line;
            // 0 = Rules, 1 = Print Orders
            var mode = 0;
            while ((line = reader.ReadLine()) != null)
            {
                if (line == "")
                {
                    mode++;
                    continue;
                }

                switch (mode)
                {
                    case 0:
                        // Parse Rules
                        var split = line.Split("|");
                        if (rules.ContainsKey(int.Parse(split[0])))
                        {
                            rules[int.Parse(split[0])].Add(int.Parse(split[1]));
                        }
                        else
                        {
                            rules.Add(
                                int.Parse(split[0]),
                                new HashSet<int> { int.Parse(split[1]) }
                            );
                        }

                        break;
                    case 1:
                        // Parse Print Orders
                        var printList = line.Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToArray();
                        var printIndex = new Dictionary<int, int>();
                        for (int i = 0; i < printList.Length; i++)
                        {
                            printIndex.Add(printList[i], i);
                        }
                        printOrders.Add((printList, printIndex));

                        break;
                }
            }
        }

        return (rules, printOrders);
    }
}
