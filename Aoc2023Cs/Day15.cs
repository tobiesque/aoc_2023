namespace Aoc2023Cs;

public class Day15
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] entries = File.ReadAllText("15".GetInputFile(test: false)).Split(',');

        if (partOne)
        {
            PartOne(entries);
            return;
        }

        foreach (var entry in entries)
        {
            if (entry.Contains('-'))
            {
                long label = Hash(entry[..^1]);
            } else if (entry.Contains('='))
            {
            }
        }
    }
    
    public static void PartOne(string[] entries)
    {
        long result = 0;
        foreach (string entry in entries)
        {
            result += Hash(entry);
        }
        Console.WriteLine($"Part One: {result}");
    }
    
    public static long Hash(string s)
    {
        int value = 0;
        foreach (var c in s)
        {
            value += c;
            value *= 17;
            value %= 256;
        }
        return value;
    }
}