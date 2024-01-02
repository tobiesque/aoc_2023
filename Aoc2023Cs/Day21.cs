using System.Text;

namespace Aoc2023Cs;

public class Day21
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: true);
        Map map = new(lines);
        Console.WriteLine(map);
    }

    public class Map
    {
        public char[,] plots;
        public int Width => plots.GetLength(0);
        public int Height => plots.GetLength(1);

        public Map(string[] lines)
        {
            plots = new char[lines[0].Length, lines.Length];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    plots[x, y] = lines[y][x];
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    sb.Append(plots[x, y]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}