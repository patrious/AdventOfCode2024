public class Day2
{
    public void RunCommand()
    {
        var start = DateTime.Now;
        Console.WriteLine("Reading file...");
        var lines = File.ReadAllLines("Advent 2024\\day2.data");

        var numOfSafeReports = 0;
        foreach (var line in lines)
        {
            var reportLine = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var parsedReport = Array.ConvertAll(reportLine, int.Parse);

            if (!CheckIfSafe(parsedReport))
            {
                for (int i = 0; i < parsedReport.Length; i++)
                {
                    //Remove the current element
                    var temp = parsedReport.ToList();
                    temp.RemoveAt(i);
                    if (CheckIfSafe(temp.ToArray()))
                    {
                        numOfSafeReports++;
                        break;
                    }
                }
            }
            else
            {
                numOfSafeReports++;
            }
        }
        Console.WriteLine("Number of safe reports: " + numOfSafeReports);
        Console.WriteLine("Time taken: " + (DateTime.Now - start));
    }

    /// <summary>
    /// Check if the report is safe
    /// </summary>
    /// <param name="parsedReport">The report to inspect</param>
    /// <returns>Returns if the report is safe</returns>
    private bool CheckIfSafe(int[] parsedReport)
    {
        var isSafe = true;
        var firstReport = parsedReport[0];
        var secondReport = parsedReport[1];
        var ascendingOrder = false;

        if (firstReport < secondReport)
        {
            ascendingOrder = true;
        }
        for (int i = 1; i < parsedReport.Length; i++)
        {
            if (parsedReport[i] == parsedReport[i - 1])
            {
                isSafe = false;
                break;
            }
            var stepsize = Math.Abs(parsedReport[i] - parsedReport[i - 1]);
            if (stepsize > 3 || stepsize < 1)
            {
                isSafe = false;
                break;
            }
            if (ascendingOrder)
            {
                if (parsedReport[i] < parsedReport[i - 1])
                {
                    isSafe = false;
                    break;
                }
            }
            else
            {
                if (parsedReport[i] > parsedReport[i - 1])
                {
                    isSafe = false;
                    break;
                }
            }
        }
        return isSafe;
    }
}
