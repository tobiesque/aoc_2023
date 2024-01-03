using System.Text;

namespace Aoc2023Cs;

public class Day21
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: true);
        Map map = new(lines);
        Console.WriteLine(map);

        int maxSteps = Day.PartOne ? 64 : 26501365;
        HashSet<Vec2> positions = new(100000000) { map.startPos };
        HashSet<Vec2> positions2 = new(100000000);
        
        for (int i = 1; i <= maxSteps; ++i)
        {
            if (i % 1000 == 0)
            {
                Console.Write($"{i}\r");
            }

            (positions, positions2) = (positions2, positions);
            positions.Clear();
            bool onTheEdge = false;
            foreach (var pos in positions2)
            {
                foreach (var dir in Vec2.Directions)
                {
                    Vec2 newPos = pos + dir;
                    onTheEdge |= !map.InBounds(newPos); 
                    if (map.InBounds(newPos) && (map[newPos] != '#'))
                    {
                        positions.Add(newPos);
                    }
                }
            }

            if (onTheEdge)
            {
                Console.WriteLine(map.ToString(positions));
                Console.WriteLine($"{i}");
                return;
            }
            // Console.WriteLine(map.ToString(positions));
            // Console.WriteLine();
        }
        
        Console.WriteLine($"Part One: {positions.Count}");
    }

    public class Map
    {
        public char[,] plots;
        public Vec2 startPos;
        public int distanceFromPos = -1;
        
        public bool InBounds(Vec2 pos) => (pos.x >= 0) && (pos.x < Width) && (pos.y >= 0) && (pos.y < Height);
        public int Width => plots.GetLength(0);
        public int Height => plots.GetLength(1);

        public ref char this[Vec2 pos] => ref plots[pos.x, pos.y];

        public Map(string[] lines)
        {
            plots = new char[lines[0].Length, lines.Length];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    plots[x, y] = lines[y][x];
                    if (plots[x, y] == 'S')
                    {
                        startPos = new(x, y);
                    }
                }
            }
        }

        public Map(Map other)
        {
            plots = (char[,])other.plots.Clone();
            startPos = other.startPos;
            distanceFromPos = other.distanceFromPos;
        }

        public void Iterate(Action<Vec2> action)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    action(new(x, y));
                }
            }
        }

        public IEnumerable<Vec2> GetPositions()
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Vec2 pos = new(x, y);
                    if (this[pos] == 'O')
                    {
                        yield return pos;
                    }
                }
            }
        }

        public void Step(Vec2 pos)
        {
            foreach (var dir in Vec2.Directions)
            {
                Vec2 newPos = pos + dir;
                if (InBounds(newPos) && (this[newPos] != '#'))
                {
                    this[newPos] = 'O';
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
        
        public string ToString(HashSet<Vec2> positions)
        {
            StringBuilder sb = new();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (positions.Contains(new(x, y)))
                    {
                        sb.Append('O');
                    }
                    else
                    {
                        sb.Append(plots[x, y]);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}