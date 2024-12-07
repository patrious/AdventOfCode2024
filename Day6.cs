using System.Formats.Asn1;
using System.Runtime.InteropServices.Marshalling;

public class Day6
{
    char gaurd = '^';
    char wall = '#';

    public async Task RunCommand()
    {
        var now = DateTime.Now;
        var (Map, gaurdLocation) = ParseData();
        var loopsDetected = 0;
        var tasks = new List<Task>();

        var semaphore = new SemaphoreSlim(1000);
        for (int y = 0; y < Map.Length; y++)
        {
            for (int x = 0; x < Map[y].Length; x++)
            {
                var nuY = y;
                var nuX = x;
                var newTask = Task.Run(async () =>
                {
                    try
                    {
                        await semaphore.WaitAsync();
                        // Make a deep copy of MAP
                        var nuMap = new char[Map.Length][];
                        for (int i = 0; i < Map.Length; i++)
                        {
                            nuMap[i] = new char[Map[i].Length];
                            Array.Copy(Map[i], nuMap[i], Map[i].Length);
                        }

                        if (nuMap[nuY][nuX] == gaurd)
                        {
                            return;
                        }
                        if (nuMap[nuY][nuX] == wall)
                        {
                            return;
                        }
                        nuMap[nuY][nuX] = wall;
                        if (RunSimulation(nuMap, gaurdLocation))
                        {
                            Interlocked.Increment(ref loopsDetected);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                tasks.Add(newTask);
            }
        }
        Console.WriteLine("Done submitting work: " + (DateTime.Now - now));
        Task.WaitAll(tasks.ToArray());
        Console.WriteLine(DateTime.Now - now);
        Console.WriteLine(loopsDetected);
        // Walk the graph,
        // As I am walking, detect the cycle.
    }

    private bool RunSimulation(char[][] map, (int, int) gaurdLocation)
    {
        var vistedNodes = new Dictionary<(int, int), int>();
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        var direction = 0;
        do
        {
            // Add gaurd location to node list
            if (vistedNodes.ContainsKey(gaurdLocation))
            {
                vistedNodes[gaurdLocation]++;
                if (vistedNodes[gaurdLocation] > 4)
                {
                    return true;
                }
            }
            else
            {
                vistedNodes.Add(gaurdLocation, 1);
            }
            // PrintMap(map, vistedNodes, gaurdLocation, direction);

            var startingDirection = direction;
            while (true)
            {
                bool endOfScenario = false;
                var nextSpot = GuardPeek(map, gaurdLocation, direction, out endOfScenario);
                if (endOfScenario)
                {
                    return false;
                }
                if (nextSpot == wall)
                {
                    // Turn right
                    direction = (direction + 1) % 4;
                }
                else
                {
                    break;
                }
                if (direction == startingDirection)
                {
                    return false;
                }
            }

            // Not stuck, take a step
            switch (direction)
            {
                case 0:
                    gaurdLocation = (gaurdLocation.Item1 - 1, gaurdLocation.Item2);
                    break;
                case 1:
                    gaurdLocation = (gaurdLocation.Item1, gaurdLocation.Item2 + 1);
                    break;
                case 2:
                    gaurdLocation = (gaurdLocation.Item1 + 1, gaurdLocation.Item2);
                    break;
                case 3:
                    gaurdLocation = (gaurdLocation.Item1, gaurdLocation.Item2 - 1);
                    break;
            }
        } while (true);
    }

    private void PrintMap(
        char[][] map,
        Dictionary<(int, int), int> vistedNodes,
        (int, int) guardLocation,
        int direction
    )
    {
        Console.WriteLine();
        Console.WriteLine("---");
        Console.WriteLine();
        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                if (i == guardLocation.Item1 && j == guardLocation.Item2)
                {
                    Console.Write('G');
                }
                else if (vistedNodes.ContainsKey((i, j)))
                {
                    Console.Write('o');
                }
                else
                {
                    Console.Write(map[i][j]);
                }
            }
            Console.WriteLine();
        }
    }

    private char GuardPeek(
        char[][] map,
        (int, int) gaurdLocation,
        int direction,
        out bool endOfScenario
    )
    {
        endOfScenario = false;
        switch (direction)
        {
            case 0:
                if (gaurdLocation.Item1 - 1 < 0)
                { // return wall;
                    endOfScenario = true;
                    return 'a';
                }
                return map[gaurdLocation.Item1 - 1][gaurdLocation.Item2];
            case 1:
                if (gaurdLocation.Item2 + 1 >= map[0].Length)
                { // return wall;
                    endOfScenario = true;
                    return 'a';
                }
                return map[gaurdLocation.Item1][gaurdLocation.Item2 + 1];
            case 2:
                if (gaurdLocation.Item1 + 1 >= map.Length)
                { // return wall;
                    endOfScenario = true;
                    return 'a';
                }
                return map[gaurdLocation.Item1 + 1][gaurdLocation.Item2];
            case 3:
                if (gaurdLocation.Item2 - 1 < 0)
                { // return wall;
                    endOfScenario = true;
                    return 'a';
                }
                return map[gaurdLocation.Item1][gaurdLocation.Item2 - 1];
        }
        throw new Exception("Invalid direction");
    }

    private (char[][], (int, int)) ParseData()
    {
        var gaurdLocation = (0, 0);
        var lines = File.ReadAllLines("Advent 2024\\day6.data");
        var map = new char[lines.Length][];
        for (int i = 0; i < lines.Length; i++)
        {
            map[i] = lines[i].ToCharArray();
            if (map[i].ToList().Contains(gaurd))
            {
                gaurdLocation = (i, Array.IndexOf(map[i], gaurd));
            }
        }

        return (map, gaurdLocation);
    }
}

[Serializable]
internal class InfiniteLoopException : Exception
{
    public InfiniteLoopException() { }

    public InfiniteLoopException(string message)
        : base(message) { }

    public InfiniteLoopException(string message, Exception innerException)
        : base(message, innerException) { }
}

[Serializable]
internal class EndOfScenarioException : Exception
{
    public EndOfScenarioException() { }

    public EndOfScenarioException(string message)
        : base(message) { }

    public EndOfScenarioException(string message, Exception innerException)
        : base(message, innerException) { }
}
