using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day16
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "16".ReadLinesArray(test: false);

        Grid grid = new(lines);

        if (partOne)
        {
            grid.LightWalk(Vec2.zero, Vec2.right );
            Console.WriteLine(grid);
            Console.WriteLine();
            Console.WriteLine(grid.ToStringDirection());
            Console.WriteLine(grid.ToStringEnergized());
        
            Console.WriteLine($"Part One: {grid.NumEnergized}");
            return;
        }

        long result = 0;
        for (int y = 0; y < grid.height; ++y)
        {
            grid.LightWalk(new (0, y), Vec2.right);
            result = long.Max(result, grid.NumEnergized);
        }
        
        for (int y = 0; y < grid.height; ++y)
        {
            grid.LightWalk(new (grid.width - 1, y), Vec2.left);
            result = long.Max(result, grid.NumEnergized);
        }
        
        for (int x = 0; x < grid.width; ++x)
        {
            grid.LightWalk(new (x, 0), Vec2.down);
            result = long.Max(result, grid.NumEnergized);
        }
        
        for (int x = 0; x < grid.width; ++x)
        {
            grid.LightWalk(new (x, grid.height - 1), Vec2.up);
            result = long.Max(result, grid.NumEnergized);
        }
        Console.WriteLine($"Part Two: {result}");
    }

    public struct Mirror(char c)
    {
        public char c = c;
        public int energized = 0;
        public char v ='.';
        public int beams = 0;

        public Vec2 Pass(Vec2 inVector, out Vec2? splitVector)
        {
            splitVector = null;
            ++energized;
            
            switch (c)
            {
                case '-':
                    {
                        if (inVector.IsHorizontal) return inVector;
                        splitVector = Vec2.left;
                        return Vec2.right;
                    }
                case '|':
                    {
                        if (inVector.IsVertical) return inVector;
                        splitVector = Vec2.up;
                        return Vec2.down;
                    }
                case '\\':
                    {
                        if (inVector == Vec2.up) return Vec2.left;
                        if (inVector == Vec2.down) return Vec2.right;
                        if (inVector == Vec2.left) return Vec2.up;
                        if (inVector == Vec2.right) return Vec2.down;
                        Debug.Fail("Invalid direction");
                        return Vec2.invalid;
                    }
                case '/':
                    {
                        if (inVector == Vec2.up) return Vec2.right;
                        if (inVector == Vec2.down) return Vec2.left;
                        if (inVector == Vec2.left) return Vec2.down;
                        if (inVector == Vec2.right) return Vec2.up;
                        Debug.Fail("Invalid direction");
                        return Vec2.invalid;
                    }
                case '.': return inVector;
            }

            Debug.Fail("Invalid mirror type");
            return Vec2.invalid;
        }
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
                    mirrors[x, y] = new Mirror(lines[y][x]);
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
                    sb.Append(mirrors[x, y].c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string ToStringEnergized()
        {
            StringBuilder sb = new();
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append(mirrors[x, y].energized == 0 ? '.' : '#');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        public string ToStringDirection()
        {
            StringBuilder sb = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append(mirrors[x, y].v);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        public IEnumerable<Mirror> All()
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    yield return mirrors[x, y];
        }
        
        public bool InBounds(Vec2 pos)
        {
            return (pos.x >= 0) && (pos.y >= 0) && (pos.x < width) && (pos.y < height);
        }

        public void Clear()
        {
            visited = new();
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    mirrors[x, y].beams = 0;
                    mirrors[x, y].energized = 0;
                    mirrors[x, y].v = '.';
                }
            }
        }

        public long NumEnergized => All().Count(m => (m.energized > 0));

        public void LightWalk(Vec2 pos, Vec2 v)
        {
            Clear();
            LightWalkRecursive(pos, v);
        }

        private static HashSet<KeyValuePair<Vec2, Vec2>> visited;
        
        private void LightWalkRecursive(Vec2 pos, Vec2 v)
        {
            if (!InBounds(pos))
            {
                return;
            }

            if (!visited.Add(new(pos, v)))
            {
                return;
            }
            
            ++mirrors[pos.x, pos.y].beams;
            mirrors[pos.x, pos.y].v = v.DirectionChar;
            
            v = mirrors[pos.x, pos.y].Pass(v, out Vec2? splitVector);
            if (splitVector != null)
            {
                LightWalkRecursive(pos + splitVector.Value, splitVector.Value);
            }
            LightWalkRecursive(pos + v, v);
        }
    }
}
