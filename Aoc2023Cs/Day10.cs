using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Dynamic;
using Aoc2023Cs.Util2d;
using Microsoft.VisualBasic.FileIO;

namespace Aoc2023Cs;

public class Day10

{
    public static bool partOne = true;

    public struct Pipe
    {
        public enum Connection
        {
            North,
            South,
            East,
            West,
            None
        };

        public static Connection Invert(Connection connection)
        {
            switch (connection)
            {
                case Connection.North: return Connection.South;
                case Connection.South: return Connection.North;
                case Connection.West: return Connection.East;
                case Connection.East: return Connection.West;
            }

            return Connection.None;
        }

        public Vec2 position;
        public char type;
        public Connection one = Connection.North;
        public Connection two = Connection.North;
        public int distance = 0;

        public static Pipe invalid = new(Vec2.invalid, ' ');

        public Connection GetConnection(int i) => (i == 0) ? two : one;

        public Pipe() : this(Vec2.invalid, '.')
        {
        }

        public Pipe(Vec2 position, char type)
        {
            this.position = position;
            this.type = type;

            switch (type)
            {
                case '|':
                    Connect(Connection.North, Connection.South);
                    break;
                case '-':
                    Connect(Connection.East, Connection.West);
                    break;
                case 'L':
                    Connect(Connection.North, Connection.East);
                    break;
                case 'J':
                    Connect(Connection.North, Connection.West);
                    break;
                case '7':
                    Connect(Connection.West, Connection.South);
                    break;
                case 'F':
                    Connect(Connection.East, Connection.South);
                    break;
                case '.':
                    Connect(Connection.None, Connection.None);
                    break;
            }
        }

        public static Connection CategorizeIncoming(Vec2 incoming)
        {
            switch (incoming.y)
            {
                case > 0: return Connection.North;
                case < 0: return Connection.South;
            }

            switch (incoming.x)
            {
                case > 0: return Connection.East;
                case < 0: return Connection.West;
                default: return Connection.None;
            }
        }

        public static Vec2 ToVec2(Connection connection)
        {
            switch (connection)
            {
                case Connection.North: return new(0, -1);
                case Connection.South: return new(0, 1);
                case Connection.West: return new(-1, 0);
                case Connection.East: return new(1, 0);
            }

            Debug.Fail("Invalid connection");
            return Vec2.invalid;
        }

        public void Connect(Connection one, Connection two)
        {
            bool oneIsGreaterThanTwo = ((int)one) > ((int)two);
            this.one = oneIsGreaterThanTwo ? one : two;
            this.two = oneIsGreaterThanTwo ? two : one;
        }

        public Connection Enter(Vec2 from) => CategorizeIncoming(from - position);

        public Connection Enter(Connection from)
        {
            Debug.Assert((one == from) || (two == from));
            return (one == from) ? two : one;
        }

        public override string ToString()
        {
            return $"({position.x}, {position.y}) - exits through {one} and {two}";
        }
    }

    public class Map
    {
        public readonly Pipe[,] map;
        public readonly Vec2 start = Vec2.invalid;

        public Map(string[] mapLines)
        {
            int height = mapLines.Length;
            int width = mapLines[0].Length;
            map = new Pipe[width, height];
            int x;
            int y;
            for (y = 0; y < mapLines.Length; y++)
            {
                string line = mapLines[y];
                for (x = 0; x < line.Length; x++)
                {
                    Vec2 pos = new(x, y);
                    map[x, y] = new Pipe(pos, line[x]);
                    if (line[x] == 'S')
                    {
                        Debug.Assert((start.x == -1) && (start.y == -1));
                        start = pos;
                    }
                }
            }

            x = start.x;
            y = start.y;
            if (this[x + 1, y].one == Pipe.Connection.West) map[x, y].one = Pipe.Connection.East;
            if (this[x, y - 1].one == Pipe.Connection.South) map[x, y].one = Pipe.Connection.North;
            if (this[x, y + 1].one == Pipe.Connection.North) map[x, y].one = Pipe.Connection.South;
            if (this[x - 1, y].two == Pipe.Connection.East) map[x, y].two = Pipe.Connection.West;
            if (this[x + 1, y].two == Pipe.Connection.West) map[x, y].two = Pipe.Connection.East;
            if (this[x, y - 1].two == Pipe.Connection.South) map[x, y].two = Pipe.Connection.North;
            if (this[x, y + 1].two == Pipe.Connection.North) map[x, y].two = Pipe.Connection.South;
            if (this[x - 1, y].one == Pipe.Connection.East) map[x, y].one = Pipe.Connection.West;

        }

        public Pipe this[Vec2 pos] => map[pos.x, pos.y];

        public Pipe this[int x, int y] =>
            (x < 0) || (y < 0) || (x >= map.GetLength(0)) || (y >= map.GetLength(1))
                ? Pipe.invalid
                : map[x, y];
    }

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] mapLines = "10".ReadLinesArray(test: false);

        Map map = new(mapLines);

        if (partOne) PartOne(map); else PartTwo(map);
    }
    
    public static void PartTwo(Map map)
    {
        Pipe.Connection enter = map[map.start].one;
        Vec2 pos = map.start;

        while (true)
        {
            Pipe pipe = map[pos];
            Console.WriteLine(pipe);

            Pipe.Connection exit = pipe.Enter(enter);
            Console.WriteLine($"Enter from {enter}, leave through {exit}");
            Vec2 move = Pipe.ToVec2(exit);
            enter = Pipe.Invert(exit);
            pos += move;

            if (pos == map.start)
            {
                Console.WriteLine("STOPPED");
                break;
            }
        }
        
        Console.WriteLine($"Part Two: ");
    }

    public static void PartOne(Map map)
    {
        Pipe.Connection[] enters = new[] { map[map.start].one, map[map.start].two };
        Vec2[] poses = new[] { map.start, map.start };
        string[] names = new[] { "A", "B" };

        int steps = 0;
        while (true)
        {
            ++steps;
            for (var i = 0; i < enters.Length; i++)
            {
                ref Pipe.Connection enter = ref enters[i];
                ref Vec2 pos = ref poses[i];

                Pipe pipe = map[pos];
                Console.WriteLine(pipe);

                Pipe.Connection exit = pipe.Enter(enter);
                Console.WriteLine($"[{names[i]}] Enter from {enter}, leave through {exit}, distance = {steps}");
                Vec2 move = Pipe.ToVec2(exit);
                enter = Pipe.Invert(exit);
                pos += move;
            }

            if (poses[0] == poses[1])
            {
                Console.WriteLine($"MET");
                break;
            }
        }

        Console.WriteLine($"Part One: {steps}");
    }
}