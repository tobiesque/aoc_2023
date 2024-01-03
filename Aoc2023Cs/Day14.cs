using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day14
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "14".ReadLinesArray(test: false);

        Platform platform = new Platform(lines);

        if (partOne)
        {
            Console.WriteLine($"{platform}");
            platform.Move(Vec2.Up);
            Console.WriteLine($"{platform}");

            Console.WriteLine($"Part One: {platform.Load()}");
        }
        else
        {
            Dictionary<long, long> hashes = new();
            long hashCode = platform.HashCode();
            hashes.Add(hashCode, -1);
            Console.WriteLine($"Cycle: {-1} = {hashCode}");

            long previousHashI = -1;
            long cycleBegin = -1;
            long cycleEnd = -1;
            Console.WriteLine($"{platform}");
            long iterations = 1000000000;
            for (long i = 0; i < iterations; ++i)
            {
                platform.Move(Vec2.Up);
                platform.Move(Vec2.Left);
                platform.Move(Vec2.Down);
                platform.Move(Vec2.Right);

                hashCode = platform.HashCode();
                if (!hashes.TryAdd(hashCode, i))
                {
                    long hashI = hashes[hashCode];
                    if (cycleBegin < 0)
                    {
                        cycleBegin = hashI;
                    }
                    else
                    {
                        if (previousHashI > hashI)
                        {
                            cycleEnd = previousHashI;
                            Console.WriteLine($"Cycle = {cycleBegin} - {cycleEnd}");
                            break;
                        }
                    }
                    previousHashI = hashI;
                }
            }

            long modulo = cycleEnd - cycleBegin + 1;
            long offset = cycleBegin + 1;
            long newIterations = (iterations - offset) % modulo;
            for (long i = 0; i < newIterations; ++i)
            {
                platform.Move(Vec2.Up);
                platform.Move(Vec2.Left);
                platform.Move(Vec2.Down);
                platform.Move(Vec2.Right);
            }
            
            Console.WriteLine($"{platform}");
            Console.WriteLine($"Part Two: {platform.Load()}");
        }
    }
        
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

            public override int GetHashCode() => type.GetHashCode();

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
        public Vec2 dimension => new Vec2(width, height);

        public Platform(string[] lines)
        {
            rocks = new Rock [lines[0].Length, lines.Length];
            rocks.Iterate2D((Vec2 pos, ref Rock rock) => rock = new Rock(lines[pos.y][pos.x], pos.x, pos.y));
        }

        public long HashCode()
        {
            const long prime = 31;
            long result = 1L;

            for (int i = 0; i < rocks.GetLength(0); i++)
            {
                for (int j = 0; j < rocks.GetLength(1); j++)
                {
                    result = (prime * result) + Rock.AsChar(ref rocks[i, j]);
                }
            }
            return result;        
        }
        
        public int Weight(Vec2 pos) => (this[pos].type == Rock.Type.Rolling) ? (height - pos.y) : 0;
        
        public bool Move(Vec2 direction)
        {
            bool didMove = false;

            foreach (int y in Util.Loop(0, height, direction.y >= 0))
            {
                foreach (int x in Util.Loop(0, width, direction.x >= 0))
                {
                    Vec2 pos = new Vec2(x, y);
                    if (this[x, y].type != Rock.Type.Rolling) continue;

                    while (true)
                    {
                        if (!pos.InBounds(dimension)) break;
                        
                        Vec2 newPos = pos + direction;
                        if (!newPos.InBounds(dimension)) break;
                        if (this[newPos].type != Rock.Type.Empty) break;

                        this[pos].Swap(ref this[newPos]);
                        pos = newPos;
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
}
