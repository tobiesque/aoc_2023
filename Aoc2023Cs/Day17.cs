using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day17
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "17".ReadLinesArray(test: true);

        Grid grid = new(lines);
        Console.WriteLine(grid);
        var path = grid.Walk(Vec2.zero, new Vec2(grid.width -1, grid.height - 1));
        Console.WriteLine(grid.ToStringPath(path));
        Console.WriteLine($"Part One: {path.Sum(p => p.heat)}");
    }

    public class Grid
    {
        public PathNode[,] nodes;

        public int width => nodes.GetLength(0);
        public int height => nodes.GetLength(1);

        public PathNode endNode;
        
        public Grid(int width, int height)
        {
            nodes = new PathNode[width, height];
        }

        public Grid(string[] lines) : this(lines[0].Length, lines.Length)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    long heat = long.Parse(lines[y][x].ToString());
                    nodes[x, y] = new PathNode(new Vec2(x, y), heat);
                }
            }

            endNode = nodes[width - 1, height - 1];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    nodes[x, y].h = nodes[x, y].pos.Distance(endNode.pos);
                }
            }
        }

        public PathNode this[Vec2 pos] => nodes[pos.x, pos.y];
        
        public bool InBounds(Vec2 pos)
        {
            return (pos.x >= 0) && (pos.y >= 0) && (pos.x < width) && (pos.y < height);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append((char)('0' + nodes[x, y].heat));
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string ToStringPath(PathNode[] path)
        {
            HashSet<Vec2> pathSet = path.Select(n => n.pos).ToHashSet();
            StringBuilder sb = new();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vec2 pos = new(x, y);
                    if (pathSet.Contains(pos))
                    {
                        sb.Append('#');
                    }
                    else
                    {
                        sb.Append((char)('0' + nodes[x, y].heat));
                    }
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public class PathNode : IComparable<PathNode>
        {
            public Vec2 pos;
            public long g = 0;
            public long h = 0;
            public long f => g + h;
            public PathNode parent = null;
            public Vec2 direction;
            public int directionMoves = 1;

            public long heat;

            public PathNode(Vec2 pos, long heat)
            {
                this.pos = pos;
                this.heat = heat;
            }

            public int CompareTo(PathNode? other)
            {
                return (f == other!.f) ? h.CompareTo(other!.h) : f.CompareTo(other!.f);
            }
        }

        private PathNode MinF()
        {
            PathNode minF = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if ((open[i].f < minF.f) || ((open[i].f == minF.f) && (open[i].h < minF.h)))
                {
                    minF = open[i];
                }
            }
            return minF;
        }        
        
        public List<PathNode> open = new ();
        public HashSet<PathNode> closed = new ();
        
        public PathNode[] Walk(Vec2 startPos, Vec2 endPos)
        {
            open.Add(this[startPos]);
            
            while (open.Count > 0)
            {
                PathNode node = MinF();
                if (node.pos == endPos)
                {
                    return CreatePath(this[startPos]);
                }
                
                closed.Add(node);                
                open.Remove(node);

                foreach (var direction in Vec2.directions)
                {
                    if (direction == -node.direction) continue;

                    Vec2 newPos = node.pos + direction;
                    if (!InBounds(newPos)) continue;
                    
                    PathNode neighbour = this[newPos];
                    if (closed.Contains(neighbour)) continue;

                    int directionMoves = (direction == node.direction) ? node.directionMoves + 1 : 1;
                    if (directionMoves >= 3) continue;
                    
                    long movementCost = node.g + neighbour.heat + node.pos.Distance(neighbour.pos);
                    if ((movementCost < neighbour.g) || !open.Contains(neighbour))
                    {
                        neighbour.g = movementCost;
                        neighbour.h = neighbour.pos.Distance(endNode.pos);
                        neighbour.parent = node;
                        neighbour.directionMoves = directionMoves;
                        neighbour.direction = direction;
                        if (!open.Contains(neighbour))
                        {
                            open.Add(neighbour);
                        }
                    }
                }
            }

            return [];
        }
        
        private PathNode[] CreatePath(PathNode startNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode node = endNode;
            while (node != startNode)
            {
                path.Add(node);
                node = node.parent;
            }
            path.Reverse();
            return path.ToArray();
        }        
    }
}
