namespace Aoc2023Cs;

public class Day2one
{
    public enum Color { red = 0, green = 1, blue = 2 };
    
    public static void Run()
    {
        var lines = Util.ReadLines("2", false);
        
        int[] maxCubes = { 12, 13, 14 };
        int game = 0;
        int validGamesResult = 0;
        foreach (string constLine in lines)
        {
            ++game;
            Console.WriteLine("Game {0}", game);
            
            string line = constLine.After(':');
            line = Util.SkipSpaces(line);
            Console.WriteLine(line);

            bool isValid = true;
            while (line.Length > 0)
            {
                int amount;
                line = Util.ExtractInt(line, out amount);
                string colorString = line.Until(',', ';');
                line = line.Substring(colorString.Length);
                Color color = colorString.ToEnum<Color>();
                Console.WriteLine("    {0} {1}", amount, color);
                isValid &= (amount <= maxCubes[Convert.ToInt32(color)]);
                
                if (line.Length < 2) break;
                line = line.Substring(2);
            }

            validGamesResult += isValid ? game : 0;
        }
        
        Console.WriteLine(validGamesResult);
    }
}