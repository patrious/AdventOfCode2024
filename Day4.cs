using System.Runtime.Caching;

public class Day4Part2
{
    List<char> relevantCharacters = new List<char> { 'M', 'A', 'S' };

    char[][] ms =
    [
        ['M', '.', 'S'],
        ['.', 'A', '.'],
        ['M', '.', 'S'],
    ];
    char[][] mm =
    [
        ['M', '.', 'M'],
        ['.', 'A', '.'],
        ['S', '.', 'S'],
    ];
    char[][] sm =
    [
        ['S', '.', 'M'],
        ['.', 'A', '.'],
        ['S', '.', 'M'],
    ];
    char[][] ss =
    [
        ['S', '.', 'S'],
        ['.', 'A', '.'],
        ['M', '.', 'M'],
    ];

    public async Task RunCommand()
    {
        var numberFound = 0;
        using (var filereader = new StreamReader("Advent 2024\\day4.data"))
        {
            var wordSearch = await ReadLines(filereader);

            do
            {
                for (var x = 0; x <= wordSearch[0].Length - ms[0].Length; x++)
                {
                    if (wordSearch[0][x] == 'M' || wordSearch[0][x] == 'S')
                    {
                        if (StartSearch(wordSearch, x, 0))
                        {
                            numberFound++;
                        }
                    }
                }
                wordSearch = await UpdateArray(wordSearch, filereader);
            } while (wordSearch.Length == 3);
        }

        Console.WriteLine("Number of XMAS found: " + numberFound);

        // Run the search
        // if we find a character, we need to search for the next set of characters
        // How to prevent double counting the same x? (count the top left corner of the space?)
    }

    private async Task<char[][]> UpdateArray(char[][] wordSearch, StreamReader filereader)
    {
        var shrunkList = wordSearch.ToList();
        shrunkList.RemoveAt(0);
        if (filereader.Peek() != -1)
        {
            shrunkList.Add((await filereader.ReadLineAsync())?.ToCharArray());
        }

        return shrunkList.ToArray();
    }

    private async Task<char[][]> ReadLines(StreamReader filereader, int numberOfLinesToRead = 3)
    {
        var returnList = new List<char[]>();
        for (int i = 0; i < numberOfLinesToRead; i++)
        {
            if (filereader.Peek() == -1)
            {
                break;
            }
            var readLine = await filereader.ReadLineAsync();
            returnList.Add(readLine.ToCharArray());
        }
        return returnList.ToArray();
    }

    private bool StartSearch(char[][] wordSearch, int x, int y)
    {
        var startCharacter = wordSearch[y][x];
        if (x + 2 >= wordSearch[y].Length || y + 2 >= wordSearch.Length)
        {
            return false;
        }
        if (wordSearch[y][x + 2] == 'M')
        {
            switch (startCharacter)
            {
                case 'M':
                    return VerifyMatch(mm, wordSearch, x, y);

                case 'S':
                    return VerifyMatch(sm, wordSearch, x, y);
            }
        }
        if (wordSearch[y][x + 2] == 'S')
        {
            switch (startCharacter)
            {
                case 'M':
                    return VerifyMatch(ms, wordSearch, x, y);

                case 'S':
                    return VerifyMatch(ss, wordSearch, x, y);
            }
        }
        return false;
    }

    private bool VerifyMatch(char[][] targetMatrix, char[][] wordSearch, int x, int y)
    {
        int smallRows = targetMatrix.Length;
        int smallCols = targetMatrix[0].Length;

        for (int i = 0; i < smallRows; i++)
        {
            for (int j = 0; j < smallCols; j++)
            {
                if (targetMatrix[i][j] == '.')
                {
                    continue;
                }
                if (wordSearch[y + i][x + j] != targetMatrix[i][j])
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public class Day4
{
    private Dictionary<char, char> searchLookup = new Dictionary<char, char>
    {
        { 'X', 'M' },
        { 'M', 'A' },
        { 'A', 'S' },
        { 'S', 'q' },
    };

    public void RunCommand()
    {
        // Parse puzzle input in 2D Array
        var input = File.ReadAllText("Advent 2024\\day4.data").Split("\n");
        var wordSearch = new char[input.Length][];
        for (int i = 0; i < input.Length; i++)
        {
            wordSearch[i] = input[i].ToCharArray();
        }

        var numberFound = 0;
        for (int y = 0; y < wordSearch.Length; y++)
        {
            for (int x = 0; x < wordSearch[y].Length; x++)
            {
                // Find the X
                if (wordSearch[y][x] == 'X')
                {
                    // Search for the word "XMAS"
                    var searchCharacter = searchLookup['X'];
                    var returnValue = SearchXmas(wordSearch, x, y, searchCharacter);
                    if (returnValue > 0)
                    {
                        numberFound += returnValue;
                    }
                }
            }
        }
        Console.WriteLine("Number of XMAS found: " + numberFound);
    }

    private int SearchXmas(
        char[][] wordSearch,
        int startX,
        int startY,
        char targetCharacter,
        int searchDirection = 0
    )
    {
        var numberFound = 0;
        if (targetCharacter == 'q')
        {
            return 1;
        }
        if (searchDirection == 0)
        {
            for (int y = startY - 1; y <= startY + 1; y++)
            {
                if (y < 0 || y >= wordSearch.Length)
                {
                    continue;
                }

                for (int x = startX - 1; x <= startX + 1; x++)
                {
                    if (x < 0 || x >= wordSearch[y].Length)
                    {
                        // Skip because outof bounds
                        continue;
                    }
                    if (wordSearch[y][x] == targetCharacter)
                    {
                        var newSearchDirection = DetermineSearchDirection(startX, startY, x, y);
                        var searchCharacter = searchLookup[targetCharacter];
                        if (SearchXmas(wordSearch, x, y, searchCharacter, newSearchDirection) > 0)
                        {
                            numberFound++;
                        }
                    }
                }
            }
        }
        else
        {
            // 1 2 3
            // 4 X 5
            // 6 7 8
            // Where X is where you start from.
            var xDirection = 0;
            var yDirection = 0;
            switch (searchDirection)
            {
                case <= 3:
                    yDirection = -1;
                    break;
                case 4:
                case 5:
                    yDirection = 0;
                    break;
                case >= 6:
                    yDirection = +1;
                    break;
            }
            switch (searchDirection)
            {
                case 1:
                case 4:
                case 6:
                    xDirection = -1;
                    break;
                case 2:
                case 7:
                    xDirection = 0;
                    break;
                case 3:
                case 5:
                case 8:
                    xDirection = +1;
                    break;
            }
            var x = startX + xDirection;
            var y = startY + yDirection;
            if (x < 0 || y < 0 || y >= wordSearch.Length || x >= wordSearch[y].Length)
            {
                // Skip because outof bounds
                return -1;
            }
            if (wordSearch[y][x] == targetCharacter)
            {
                var searchCharacter = searchLookup[targetCharacter];
                if (SearchXmas(wordSearch, x, y, searchCharacter, searchDirection) > 0)
                {
                    numberFound++;
                }
            }
            else
            {
                return -1;
            }
        }

        return numberFound;
    }

    private int DetermineSearchDirection(int startX, int startY, int x, int y)
    {
        // 1 2 3
        // 4 X 5
        // 6 7 8
        // Where X is where you start from.
        if (x < startX)
        {
            if (y < startY)
            {
                return 1;
            }
            if (y == startY)
            {
                return 4;
            }
            if (y > startY)
            {
                return 6;
            }
        }
        if (x == startX)
        {
            if (y < startY)
            {
                return 2;
            }
            if (y > startY)
            {
                return 7;
            }
        }
        if (x > startX)
        {
            if (y < startY)
            {
                return 3;
            }
            if (y == startY)
            {
                return 5;
            }
            if (y > startY)
            {
                return 8;
            }
        }
        throw new Exception("Invalid search direction");
    }
}
