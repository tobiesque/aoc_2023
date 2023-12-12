namespace Aoc2023Cs;

using System.Diagnostics;

using Vec2 = Util2d.Vec2<int>;

public class Day10
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] mapLines = "10".ReadLinesArray(test: false);

        Map map = new(mapLines);
        if (partOne) PartOne(map); else PartTwo(map);
    }
    
    public static void PartTwo(Map map)
    {
        // mark path
        map.Walk(map[map.start].one, (pipe, newPos) =>
        {
            pipe.isOnPath = true;
            return (newPos == map.start);
        });

        // scan lines and count going in and out of the path
        int insideCount = 0;
        for (int y = 0; y < map.map.GetLength(1); ++y)
        {
            bool isInside = false;
            bool isOnHorizontalPath = false;
            Pipe.Connection horizontalBegin = Pipe.Connection.None; 
            for (int x = 0; x < map.map.GetLength(0); ++x)
            {
                Vec2 pos = new(x, y);
                Pipe pipe = map[pos];
                if (pipe.isOnPath)
                {
                    Console.Write('+');
                    
                    // is straight?
                    if (pipe.one == Pipe.Invert(pipe.two))
                    {
                        // straight vertical is crossing path boundaries
                        if (Pipe.IsVertical(pipe.one))
                        {
                            isInside = !isInside;
                        }
                        // straight horizontal is ignored
                    }
                    else
                    {
                        isOnHorizontalPath = !isOnHorizontalPath;
                        if (isOnHorizontalPath)
                        {
                            horizontalBegin = Pipe.IsVertical(pipe.one) ? pipe.one : pipe.two;
                        }
                        else
                        {
                            Pipe.Connection horizontalEnd = Pipe.IsVertical(pipe.one) ? pipe.one : pipe.two;
                            if (horizontalBegin != horizontalEnd) isInside = !isInside;
                        }
                    }
                }
                else
                {
                    if (isInside && !isOnHorizontalPath)
                    {
                        Console.Write('I');
                        ++insideCount;
                    }
                    else
                    {
                        Console.Write('O');
                    }
                }
            }
            Console.WriteLine();
        }
        
        Console.WriteLine($"Part Two: {insideCount}");
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
    
    public class Pipe
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
            return connection switch
            {
                Connection.North => Connection.South,
                Connection.South => Connection.North,
                Connection.West => Connection.East,
                Connection.East => Connection.West,
                _ => Connection.None
            };
        }

        public static bool IsVertical(Connection connection) => connection is Connection.North or Connection.South;


        public Vec2 pos = Vec2.invalid;
        public Connection one = Connection.North;
        public Connection two = Connection.North;
        public bool isOnPath = false;

        public static Pipe invalid = new(Vec2.invalid, ' ');

        public Pipe(Vec2 pos, char type)
        {
            this.pos = pos;

            if (type == '|') Connect(Connection.North, Connection.South);
            else if (type == '-') Connect(Connection.East, Connection.West);
            else if (type == 'L') Connect(Connection.North, Connection.East);
            else if (type == 'J') Connect(Connection.North, Connection.West);
            else if (type == '7') Connect(Connection.West, Connection.South);
            else if (type == 'F') Connect(Connection.East, Connection.South);
            else if (type == '.') Connect(Connection.None, Connection.None);
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

        public Connection Enter(Connection from) => (one == from) ? two : one;

        public override string ToString() => $"({pos.x}, {pos.y}) - exits through {one} and {two}";
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

        public void Walk(Pipe.Connection enter, Func<Pipe, Vec2, bool> callback)
        {
            Vec2 pos = start;
            int steps = 0;
            while (true)
            {
                ++steps;
                Pipe pipe = this[pos];
                Pipe.Connection exit = pipe.Enter(enter);
                enter = Pipe.Invert(exit);
                pos += Pipe.ToVec2(exit);
                if (callback(pipe, pos)) break;
            }
        }
    }
}
