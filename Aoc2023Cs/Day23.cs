using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day23
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: false);
        Map map = new(lines);
        if (Day.PartOne)
        {
            Console.WriteLine(map);
            Map.Context context = new();
            map.Walk(context, map.startPos, 0);
            var path = context.longestPath;
            Console.WriteLine(map);
            Console.WriteLine($"Day {Day.PartStr}: {path.Count-1}");
        }
        else
        {
            map.CreateNetwork();
            map.WriteDot();
            var path = map.WalkNetwork();
            Console.WriteLine(map.ToStringNodes(path));
            var pathStr = path.MakeList("\n");
            Console.WriteLine(pathStr);
            Console.WriteLine($"Day {Day.PartStr}: {path.Sum(p => p.length)}");
        }
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
        public Tile this[Vec2 pos] => tiles[pos.x-1, pos.y-1];
        public bool InBounds(Vec2 pos) => (pos.x >= 1) && (pos.x <= Width) && (pos.y >= 1) && (pos.y <= Height);
        public bool IsValid(Vec2 pos) => (InBounds(pos) && this[pos].Walkable);
        
        public class Node
        {
            public Vec2 pos;
            public readonly Connection?[] connections = new Connection?[4];
            public IEnumerable<Connection> Connections => connections.OfType<Connection>();
        }

        public class Connection()
        {
            public Vec2 from = Vec2.Zero;
            public Vec2 to = Vec2.Zero;
            public int length = 0;

            public override string ToString()
            {
                return $"[{from}->{to}, {length}]";
            }
        }

        public Dictionary<Vec2, Node> network = new();

        public void CreateNetwork()
        {
            for (int y = 1; y <= Height; ++y)
            {
                for (int x = 1; x <= Width; ++x)
                {
                    Vec2 pos = new(x, y);
                    if (!this[pos].Walkable) continue;
                    if ((pos != startPos) && (pos != endPos))
                    {
                        int numConnections = Vec2.Directions.Select(dir => pos + dir).Count(IsValid);
                        if (numConnections is > 0 and < 3) continue;
                    }

                    Node node = new () { pos = pos };
                    network.Add(pos, node);
                    Console.WriteLine($"{pos}");
                }
            }

            foreach (var node in network.Values)
            {
                Vec2 pos = node.pos;
                for (var i = 0; i < Vec2.Directions.Length; i++)
                {
                    if (node.connections[i] != null) continue;
                    Vec2 newPos = pos + Vec2.Directions[i];
                    if (!IsValid(pos + Vec2.Directions[i])) continue;

                    // scan and find next node in that direction
                    int length = 0;
                    Vec2 scanPos = newPos;
                    Vec2 lastPos = pos;
                    while (true)
                    {
                        ++length;
                        int connectionCount = 0;
                        var connections = Vec2.Directions.Select(dir => scanPos + dir)
                                                         .Where(p => (p != lastPos))
                                                         .Where(IsValid)
                                                         .ToArray();
                        connectionCount = connections.Length;
                        if (connectionCount == 1)
                        {
                            lastPos = scanPos;
                            scanPos = connections.First();
                        }

                        if (connectionCount != 1)
                        {
                            Debug.Assert(network.ContainsKey(scanPos));
                            Debug.Assert(pos != scanPos);
                            break;
                        }
                    }

                    Debug.Assert(network.ContainsKey(scanPos));
                    Debug.Assert(pos != scanPos);
                    node.connections[i] = new() { from = pos, to = scanPos, length = length };
                }
            }
        }
        
        public class NodeContext
        {
            public Connection connection;
            public int length = 0;
            public int depth = 0;
            public HashSet<Connection> connectionPath = new ();
            public HashSet<Vec2> path = new ();
        }
        
        public HashSet<Connection> WalkNetwork()
        {
            Stack<NodeContext> stack = new();
            stack.Push(new () {connection = new () {from = startPos, to = startPos, length = 0}});
            int maxLength = 0;
            HashSet<Connection> maxPath = new ();

            int maxDepth = 0;
            long i = 0;
            while (stack.TryPop(out var context))
            {
                ++i;
                if ((i % 10000) == 0)
                {
                    Console.Write($"{maxDepth}, {stack.Count}, {i}\r");
                }

                context.path.Add(context.connection.to);
                context.connectionPath.Add(context.connection);
                context.length += context.connection.length;
                if (context.connection.to == endPos)
                {
                    if (context.length > maxLength)
                    {
                        maxLength = context.length;
                        maxPath = [..context.connectionPath];
                        Debug.Assert(maxLength == maxPath.Sum(c => c.length));
                    }
                    continue;
                }

                maxDepth = Math.Max(maxDepth, context.depth);
                Node node = network[context.connection.to];
                foreach (var connection in node.Connections)
                {
                    if (context.path.Contains(connection.to)) continue; 
                    stack.Push(new() {connection = connection,
                                      connectionPath = [..context.connectionPath],
                                      path = [..context.path],
                                      length = context.length, 
                                      depth = context.depth + 1});
                }
            }

            return maxPath;
        }

        public class Context
        {
            public HashSet<Vec2> path = new();
            
            public int longest = 0;
            public HashSet<Vec2> longestPath = new();
        }

        public void Walk(Context context, Vec2 pos, int steps)
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
                if (!InBounds(newPos)) return; 
                Tile newTile = this[newPos];
                if (newTile.Walkable && !context.path.Contains(newPos))
                {
                    Walk(context, newPos, steps);
                }
                context.path.Remove(pos);
                return;
            }
            
            foreach (var newPos in Vec2.Directions.Select(v => v + pos))
            {
                if (!InBounds(newPos)) continue;
                Tile newTile = this[newPos];
                if (newTile.Walkable && !context.path.Contains(newPos))
                {
                    Walk(context, newPos, steps);
                }
            }
            context.path.Remove(pos);
        }

        public Map(string[] lines)
        {
            int width = lines[0].Length;
            int height = lines.Length;
            tiles = new Tile[width, height];
            
            int y = 1;
            foreach (var line in lines)
            {
                int x = 1;
                foreach (var c in line)
                {
                    if ((y == 1) && (c == '.')) startPos = new(x, y); 
                    if ((y == Height) && (c == '.')) endPos = new(x, y);
                    tiles[x-1, y-1] = new (c);
                    ++x;
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
            for (int y = 1; y <= Height; ++y)
            {
                for (int x = 1; x <= Width; ++x)
                {
                    sb.Append(path.Contains(new(x, y)) ? 'O' : this[new (x, y)].c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string ToStringNodes(ICollection<Connection> path)
        {
            StringBuilder sb = new();
            for (int y = 1; y <= Height; ++y)
            {
                for (int x = 1; x <= Width; ++x)
                {
                    sb.Append(path.Select(c => c.from).Contains(new(x, y)) ? '*' : this[new (x, y)].c);
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        
        public string Vec2ToDot(Vec2 pos)
        {
            if (pos == startPos) return "startPos";
            if (pos == endPos) return "endPos";
            return "P" + (pos + Vec2.One).ToString().RemoveChars('(', ')', ',').Replace(" ", "_");
        }
        
        public void WriteDot()
        {
            StringBuilder sb = new();
            sb.AppendLine("digraph day20 {");
            // sb.AppendLine("\tnode [fontsize=25.0 width=0.5];");
            foreach (Node node in network.Values)
            {
                foreach (var connection in node.Connections)
                {
                    sb.AppendLine($"{Vec2ToDot(connection.from)}->{Vec2ToDot(connection.to)} " +
                                  $"[weight={connection.length} label={connection.length}]");
                }
            }
            sb.AppendLine("}");
            File.WriteAllText(Day.Str + ".dot", sb.ToString());
        }
    }
}