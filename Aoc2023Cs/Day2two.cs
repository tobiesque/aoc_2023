namespace Aoc2023Cs;

public partial class Day2
{
    public static void RunTwo()
    {
        var lines = "2".ReadLinesSpan(test: false);
        
        int game = 0;
        int powerResult = 0;
        foreach (ref string lineStr in lines)
        {
            Span<char> line = lineStr.AsSpan();

            ++game;
            Console.WriteLine("Game {0}", game);
            
            line.After(':').SkipWhiteRef();
            Console.WriteLine(line.ToString());

            int[] minCubes = { 0, 0, 0 };
            
            while (line.Length > 0)
            {
                line.ExtractIntRef(out int amount);
                var colorString = line.Until(',', ';');
                line = line[colorString.Length..];
                Color color = colorString.ToString().ToEnum<Color>();
                Console.WriteLine("    {0} {1}", amount, color);

                int colorInt = Convert.ToInt32(color);
                if (minCubes[colorInt] < amount)
                {
                    minCubes[colorInt] = amount;
                }

                if (line.Length < 2) break;
                line = line[2..];
            }
            
            int power = minCubes[0] * minCubes[1] * minCubes[2];
            Console.WriteLine("    {0} = {1}*{2}*{3}", power, minCubes[0], minCubes[1], minCubes[2] );
            powerResult += power;
        }
        
        Console.WriteLine(powerResult);
    }
}