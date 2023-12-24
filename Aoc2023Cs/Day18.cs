using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day18
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "18".ReadLinesArray(test: false);

        Dig dig = new(lines);
        Console.WriteLine(dig);
        File.WriteAllText("after.txt", dig.ToString());

        int result = dig.CountDug();
        string partName = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partName}: {result}");
        
    }

    public class Dig
    {
        public char[,] fields;

        public int width => fields.GetLength(0);
        public int height => fields.GetLength(1);

        public Dig(string[] lines)
        {
            Vec2 pos = Vec2.zero;
            Vec2 max = new (int.MinValue, int.MinValue);
            Vec2 min = new (int.MaxValue, int.MaxValue);
            foreach (string line in lines)
            {
                var parts = line.Split(' ');
                Vec2 dir = FromDir(parts[0][0]) * int.Parse(parts[1]);
                pos += dir;
                min = Vec2.Min(min, pos);
                max = Vec2.Max(max, pos);
            }

            int margin = 0;
            min -= new Vec2(margin, margin);
            max += new Vec2(margin, margin);
            
            Vec2 dim = max - min + new Vec2(1, 1);
            
            fields = new char[dim.x, dim.y];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    fields[x, y] = '.';
            
            pos = Vec2.zero;
            foreach (string line in lines)
            {
                var parts = line.Split(' ');
                Vec2 dir = FromDir(parts[0][0]) * int.Parse(parts[1]);
                if (dir.IsVertical)
                {
                    DrawVLine(pos - min, pos - min + dir);
                }
                pos += dir;
            }

            Fill();
            
            pos = Vec2.zero;
            foreach (string line in lines)
            {
                var parts = line.Split(' ');
                Vec2 dir = FromDir(parts[0][0]) * int.Parse(parts[1]);
                if (dir.IsHorizontal)
                {
                    DrawHLine(pos - min, pos - min + dir);
                }
                pos += dir;
            }
        }

        public void DrawVLine(Vec2 from, Vec2 to)
        {
            if (from.y > to.y) (from, to) = (to, from);
            for (int y = from.y; y < to.y; ++y)
            {
                fields[from.x, y] = '#';
            }
        }
        
        public void DrawHLine(Vec2 from, Vec2 to)
        {
            if (from.x > to.x) (from, to) = (to, from);
            for (int x = from.x; x <= to.x; ++x)
            {
                fields[x, from.y] = '#';
            }
        }

        public void Xor(Vec2 pos) => fields[pos.x, pos.y] = (fields[pos.x, pos.y] == '#') ? '.' : '#';

        public ref char this[Vec2 pos] => ref fields[pos.x, pos.y];
            
        public bool InBounds(Vec2 pos)
        {
            return (pos.x >= 0) && (pos.y >= 0) && (pos.x < width) && (pos.y < height);
        }

        public int CountDug()
        {
            int result = 0;
            foreach (char field in fields)
            {
                if (field == '#') ++result;
            }
            return result;
        }

        public void Fill()
        {
            for (int y = 0; y < height; ++y)
            {
                bool fill = false;
                int x = 0;
                while (x < width)
                {
                    while (fields[x, y] == '.')
                    {
                        if (fill) { fields[x, y] = '#'; }
                        if (++x >= width) break;
                    }
                    if (x >= width) break;

                    int hashes = 0;
                    while (fields[x, y] == '#')
                    {
                        ++hashes;
                        if (++x >= width) break;
                    }

                    if (hashes == 1)
                    {
                        fill = !fill;
                    }
                }
            }
        }
        
        public override string ToString()
        {
            StringBuilder sb = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append(fields[x, y]);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        public static Vec2 FromDir(char dir)
        {
            if (dir == 'R') return Vec2.right;
            if (dir == 'D') return Vec2.down;
            if (dir == 'L') return Vec2.left;
            if (dir == 'U') return Vec2.up;
            return Vec2.zero;
        }
    }
}
