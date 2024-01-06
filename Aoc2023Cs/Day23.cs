using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day23
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: false);
        Map map = new(lines);
        Console.WriteLine(map);
        // Map.Context2 context = new();
        // map.Walk(context, map.startPos, 0);
        var path = map.Walk();
        Console.WriteLine(map.ToString(path));
        Console.WriteLine($"Day {Day.PartStr}: {path.Count-1}");
    }

    public class Map
    {
        public class Tile
        {
            public Tile(char c) { this.c = c; }
            public char c;
            public HashSet<Vec2> longestPath = new();

            public bool Walkable => c != '#';

            public bool IsSlope => c is '>' or 'v';
            public Vec2 Slope => (c == '>') ? Vec2.Right : Vec2.Down;
        }

        public Tile[,] tiles;
        public Vec2 startPos;
        public Vec2 endPos;
        
        public int Width => tiles.GetLength(0);
        public int Height => tiles.GetLength(1);
        public Tile this[Vec2 pos] => tiles[pos.x, pos.y];
        public bool InBounds(Vec2 pos) => (pos.x >= 0) && (pos.x < Width) && (pos.y >= 0) && (pos.y < Height);

        public class Context2
        {
            public HashSet<Vec2> path = new();
            
            public int longest = 0;
            public HashSet<Vec2> longestPath = new();
        }

        public class Context
        {
            public Vec2 pos;
            public int steps = 0;
            public HashSet<Vec2> path = new();
        }
        
        public HashSet<Vec2> Walk()
        {
            HashSet<Vec2> longestPath = new();
            Vec2 maxPos = Vec2.Zero;

            Stack<Context> stack = new(1000000);
            stack.Push(new () { pos = startPos });

            long i = 0;
            
            while (stack.TryPop(out var context))
            {
                ++i;
                maxPos = maxPos.Max(context.pos);

                if ((i % 10000) == 0)
                {
                    Console.Write($"{stack.Count}, {maxPos}/({Width}, {Height}), {i}\r");
                }
                
                ++context.steps;
                Tile tile = this[context.pos];
                context.path.Add(context.pos);
                
                if (context.pos == endPos)
                {
                    if (context.path.Count > longestPath.Count)
                    {
                        longestPath = new(context.path);
                    }
                    context.path.Remove(context.pos);
                }
                else
                {
                    if (Day.PartOne && tile.IsSlope)
                    {
                        Vec2 newPos = context.pos + tile.Slope;
                        if (InBounds(newPos))
                        {
                            Tile newTile = this[newPos];
                            if (InBounds(newPos) && newTile.Walkable && !context.path.Contains(newPos))
                            {
                                stack.Push(new() { pos = newPos, path = new(context.path), steps = context.steps });
                            }
                        }
                        context.path.Remove(context.pos);
                    }

                    foreach (var newPos in Vec2.Directions.Select(v => v + context.pos))
                    {
                        if (InBounds(newPos))
                        {
                            Tile newTile = this[newPos];
                            if (newTile.Walkable && !context.path.Contains(newPos))
                            {
                                stack.Push(new() { pos = newPos, path = new(context.path), steps = context.steps });
                            }
                        }
                    }
                }
            }

            return longestPath;
        }

        public void Walk(Context2 context, Vec2 pos, int steps)
        {
            Tile tile = this[pos];

            ++steps;
            context.path.Add(pos);
            
            if (pos == endPos)
            {
                Console.WriteLine($"    {steps}");
                if (steps > context.longest)
                {
                    context.longest = steps;
                    context.longestPath = new(context.path);
                }
                context.path.Remove(pos);
                return;
            }

            if (Day.PartOne && tile.IsSlope)
            {
                Vec2 newPos = pos + tile.Slope;
                Tile newTile = this[newPos];
                if (InBounds(newPos) && newTile.Walkable && !context.path.Contains(newPos))
                {
                    Walk(context, newPos, steps);
                }
                context.path.Remove(pos);
                return;
            }
            
            foreach (var newPos in Vec2.Directions.Select(v => v + pos))
            {
                if (InBounds(newPos))
                {
                    Tile newTile = this[newPos];
                    if (InBounds(newPos) && newTile.Walkable && !context.path.Contains(newPos))
                    {
                        Walk(context, newPos, steps);
                    }
                }
            }
            context.path.Remove(pos);
        }

        public Map(string[] lines)
        {
            int width = lines[0].Length;
            int height = lines.Length;
            tiles = new Tile[width, height];
            
            int y = 0;
            foreach (var line in lines)
            {
                int x = 0;
                foreach (var c in line)
                {
                    if ((y == 0) && (c == '.'))
                    {
                        startPos = new(x, y);
                    } 
                    if ((y == (Height-1)) && (c == '.'))
                    {
                        endPos = new(x, y);
                    } 
                    tiles[x++, y] = new (c);
                }
                ++y;
            }
        }

        public override string ToString()
        {
            return ToString([]);
        }
        
        public string ToString(ICollection<Vec2> path)
        {
            StringBuilder sb = new();
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (path.Contains(new (x, y)))
                    {
                        sb.Append('O');
                    }
                    else
                    {
                        sb.Append(tiles[x, y].c);
                    }
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}