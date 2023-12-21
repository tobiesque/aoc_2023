namespace Aoc2023Cs;

public class Day16
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "16".ReadLinesArray(test: true);
    }

    public struct Mirror
    {
        public char c;
    }
    
    public class Grid
    {
        public Mirror[,] mirrors;

        public int width => mirrors.GetLength(0);
        public int height => mirrors.GetLength(1);
        
        public Grid(int width, int height)
        {
            mirrors = new Mirror[width, height];
        }

        public Grid(string[] lines) : this(lines[0].Length, lines.Length)
        {
            for (int y = 0; y < lines.Length; y++)
            {
                for (int x = 0; x < lines[y].Length; x++)
                {
                    mirrors[x, y].c = lines[y][x];
                }
            }
        }
    }
}
