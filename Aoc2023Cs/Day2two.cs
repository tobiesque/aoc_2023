namespace Aoc2023Cs;

public class Day2two
{
    public enum Color { red = 0, green = 1, blue = 2 };
    
    public static void Run()
    {
        var lines = Util.ReadLines("2", false);
        
        int game = 0;
        int powerResult = 0;
        foreach (string constLine in lines)
        {
            ++game;
            Console.WriteLine("Game {0}", game);
            
            string line = constLine.After(':');
            line = Util.SkipSpaces(line);
            Console.WriteLine(line);

            int[] minCubes = { 0, 0, 0 };
            
            while (line.Length > 0)
            {
                int amount;
                line = Util.ExtractInt(line, out amount);
                string colorString = line.Until(',', ';');
                line = line.Substring(colorString.Length);
                Color color = colorString.ToEnum<Color>();
                Console.WriteLine("    {0} {1}", amount, color);

                int colorInt = Convert.ToInt32(color);
                if (minCubes[colorInt] < amount)
                {
                    minCubes[colorInt] = amount;
                }

                if (line.Length < 2) break;
                line = line.Substring(2);
            }
            
            int power = minCubes[0] * minCubes[1] * minCubes[2];
            Console.WriteLine("    {0} = {1}*{2}*{3}", power, minCubes[0], minCubes[1], minCubes[2] );
            powerResult += power;
        }
        
        Console.WriteLine(powerResult);
    }
}