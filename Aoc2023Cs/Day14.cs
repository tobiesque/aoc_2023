using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day14
{
    public class Platform
    {
        public struct Rock
        {
            public enum Type { Empty, Rolling, Static };
            public Type type = Type.Empty;

            public static char AsChar(ref Rock rock)
            {
                if (rock.type == Type.Empty) return '.';
                if (rock.type == Type.Rolling) return 'O';
                if (rock.type == Type.Static) return '#';
                Debug.Fail("Unknown rock type");
                return ' ';
            }
            
            public static Type FromChar(char c)
            {
                if (c == '.') return Type.Empty;
                if (c == '#') return Type.Static;
                if (c == 'O') return Type.Rolling;
                Debug.Fail("Unknown rock type");
                return Type.Empty;
            }
            
            public Rock(char c, int x, int y)
            {
                type = FromChar(c);
            }
        }
        
        public Rock[,] rocks;
        
        public ref Rock this[Vec2 pos] => ref rocks[pos.x, pos.y];
        public ref Rock this[int x, int y] => ref rocks[x, y];
        
        public int width => rocks.GetLength(0);
        public int height => rocks.GetLength(1);

        public Platform(string[] lines)
        {
            rocks = new Rock [lines[0].Length, lines.Length];
            rocks.Iterate2D((Vec2 pos, ref Rock rock) => rock = new Rock(lines[pos.y][pos.x], pos.x, pos.y));
        }

        public int Weight(Vec2 pos) => (this[pos].type == Rock.Type.Rolling) ? (height - pos.y) : 0;
        
        public bool MoveNorth()
        {
            bool didMove = false;
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < height; x++)
                {
                    if (rocks[x, y].type != Rock.Type.Rolling) continue;

                    Vec2 pos = new Vec2(x, y);
                    while (true)
                    {
                        if (pos.y == 0) break;
                        Vec2 newPos = new Vec2(pos.x, pos.y - 1);
                        if (this[newPos].type != Rock.Type.Empty) break;

                        this[pos].Swap(ref this[newPos]);
                        --pos.y;
                        didMove = true;
                    }
                }
            }
            return didMove;
        }

        public int Load()
        {
            int result = 0;
            rocks.Iterate2D((Vec2 pos, ref Rock rock) => result += Weight(pos));
            return result;

        }

        public override string ToString()
        {
            StringBuilder sb = new((width * height) + height + 2);
            rocks.Iterate2D((Vec2 _, ref Rock rock) => sb.Append(Rock.AsChar(ref rock)), (_) => sb.Append('\n'));
            return sb.ToString();
        } 
    }
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "14".ReadLinesArray(test: false);

        Platform platform = new Platform(lines);
        Console.WriteLine($"{platform}");
        platform.MoveNorth();
        Console.WriteLine($"{platform}");
        
        Console.WriteLine($"Part One: {platform.Load()}");
    }
}
