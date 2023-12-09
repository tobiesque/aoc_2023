namespace Aoc2023Cs;

public partial class Day2
{
    public static void Run(int part)
    {
        if (part == 1) RunOne(); else RunTwo();
    }
    
    public enum Color { red = 0, green = 1, blue = 2 };
    
    public static void RunOne()
    {
        var lines = "2".ReadLinesEnumerable(test: false);
        
        int[] maxCubes = { 12, 13, 14 };
        int game = 0;
        int validGamesResult = 0;
        foreach (string lineStr in lines)
        {
            Span<char> line = lineStr.AsSpan();
            
            ++game;
            Console.WriteLine("Game {0}", game);
            
            line.After(':').SkipWhiteRef();
            Console.WriteLine(line.ToString());

            bool isValid = true;
            while (line.Length > 0)
            {
                line.ExtractIntRef(out int amount);
                var colorString = line.Until(',', ';');
                line.SkipRef(colorString.Length);
                Color color = colorString.ToString().ToEnum<Color>();
                Console.WriteLine("    {0} {1}", amount, color);
                isValid &= (amount <= maxCubes[Convert.ToInt32(color)]);
                
                if (line.Length < 2) break;
                line = line[2..];
            }

            validGamesResult += isValid ? game : 0;
        }
        
        Console.WriteLine(validGamesResult);
    }
}