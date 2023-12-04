using System.Text;

namespace Aoc2023Cs;

public static class Day1 {
    public static void Run()
    {
        bool partTwo = false;
        bool test = false;
        string inputFile = partTwo ? "1two" : "1one";
        inputFile += test ? ".tst" : ".txt";
        var lines = File.ReadLines(inputFile);
        
        int result = 0;

        string[] wordNumbers =
        {
            "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"
        };
        
        foreach (string line in lines)
        {
            string prettyLine = line;
            if (partTwo)
            {
                prettyLine = line;
            }
            else
            {
                // convert text to numbers for Day 2 
                StringBuilder resultSb = new();
                string workLine = line;
                while (workLine.Length > 0)
                {
                    for (var i = 0; i < wordNumbers.Length; i++)
                    {
                        if (workLine.StartsWith(wordNumbers[i]))
                        {
                            resultSb.Append(i % 10);
                            break;
                        }
                    }
                    workLine = workLine.Remove(0, 1);
                }
                prettyLine = resultSb.ToString();
                Console.WriteLine("\t{0} -> {1}", line, prettyLine);
            }

            // add up first and last digits (if they exist)
            int numDigits = prettyLine.Count(Char.IsDigit);
            if (numDigits == 0) continue;
            
            result += ( prettyLine.First(Char.IsDigit) - '0' ) * 10;
            result += prettyLine.Last(Char.IsDigit) - '0';

        }
        

        Console.WriteLine(result);
    }
}