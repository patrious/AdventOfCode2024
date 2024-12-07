using System.Text.RegularExpressions;

public class Day3
{
    public void RunCommand()
    {
        var corruptMemory = File.ReadAllText("Advent 2024\\day3.data");

        // Regex for 1 to 3 digits
        var runningTotal = 0;
        var regex = new Regex(@"mul\((\d{1,3}),(\d{1,3})\)|(do\(\))|(don\'t\(\))");
        var matches = regex.Matches(corruptMemory);

        var enabled = true;
        foreach (Match match in matches)
        {
            if (match.Groups[0].ToString() == "do()")
            {
                enabled = true;
                continue;
            }
            if (match.Groups[0].ToString() == "don't()")
            {
                enabled = false;
                continue;
            }
            if (!enabled)
            {
                continue;
            }
            var first = int.Parse(match.Groups[1].Value);
            var second = int.Parse(match.Groups[2].Value);
            runningTotal += first * second;
            Console.WriteLine(match + " = " + first * second);
            Console.WriteLine("Running total: " + runningTotal);
        }
        Console.WriteLine("Running total: " + runningTotal);
    }
}
