using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day17
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "17".ReadLinesArray(test: false);

        Grid grid = new(lines);
        grid.FillNodes();

        Console.WriteLine(grid);
        grid.Walk(true);
        grid.Walk(false);

        var pathH = grid.CreatePath(true);
        var pathV = grid.CreatePath(false);
        long result = long.Min(pathH.Max(p => p.g), pathV.Max(p => p.g));
        
        Console.WriteLine($"Part One: {result}");
    }

    public class Grid
    {
        
        public Node[,] hNodes;
        public Node[,] vNodes;

        public int width => hNodes.GetLength(0);
        public int height => hNodes.GetLength(1);

        public Grid(int width, int height)
        {
            hNodes = new Node[width, height];
            vNodes = new Node[width, height];
        }

        public Grid(string[] lines) : this(lines[0].Length, lines.Length)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    long heat = long.Parse(lines[y][x].ToString());
                    hNodes[x, y] = new Node(new Vec2(x, y), heat, true);
                    vNodes[x, y] = new Node(new Vec2(x, y), heat, false);
                }
            }

            startPos = new(0, 0);
            endPos = new (width - 1, height - 1);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    hNodes[x, y].h = hNodes[x, y].pos.Distance(endPos);
                    vNodes[x, y].h = vNodes[x, y].pos.Distance(endPos);
                }
            }
        }

        public Vec2 startPos;
        public Vec2 endPos;
        
        public Node H(Vec2 pos) => hNodes[pos.x, pos.y];
        public Node V(Vec2 pos) => vNodes[pos.x, pos.y];
        
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
                    sb.Append((char)('0' + hNodes[x, y].heat));
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public string ToStringPath(Node[] path)
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
                        sb.Append((char)('0' + hNodes[x, y].heat));
                    }
                }

                sb.AppendLine();
            }
            return sb.ToString();
        }

        public class Node
        {
            public readonly Vec2 pos;
            public readonly long heat;
            public readonly bool isHorizontal; // for hashcode diff between hNodes and vNodes


            public struct Neighbour(Node node, long heat)
            {
                public Node node = node;
                public long heat = heat;
            }
            
            public List<Neighbour> neighbours = new();
            
            public long g = 0;
            public long h = 0;
            public long f => g + h;
            public Node parent = null;
            
            public Node(Vec2 pos, long heat, bool isHorizontal)
            {
                this.pos = pos;
                this.heat = heat;
                this.isHorizontal = isHorizontal;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(pos.GetHashCode(), isHorizontal.GetHashCode());
            }
        }

        private Node MinF()
        {
            Node minF = open[0];
            for (int i = 1; i < open.Count; i++)
            {
                if ((open[i].f < minF.f) || ((open[i].f == minF.f) && (open[i].h < minF.h)))
                {
                    minF = open[i];
                }
            }
            return minF;
        }        
        
        public List<Node> open = new ();
        public HashSet<Node> closed = new ();
        
        public bool Walk(bool horizontal)
        {
            FillNodes();
            
            open.Add(horizontal ? H(startPos) : V(startPos));
            
            while (open.Count > 0)
            {
                Node node = MinF();
                if (node.pos == endPos)
                {
                    return true;
                }
                
                closed.Add(node);
                open.Remove(node);

                foreach(Node.Neighbour neighbour in node.neighbours) {
                    if (closed.Contains(neighbour.node)) continue;

                    long movementCost = node.g + neighbour.heat;
                    if ((movementCost < neighbour.node.g) || !open.Contains(neighbour.node))
                    {
                        neighbour.node.g = movementCost;
                        neighbour.node.h = neighbour.node.pos.Distance(endPos);
                        neighbour.node.parent = node;
                        if (!open.Contains(neighbour.node))
                        {
                            open.Add(neighbour.node);
                        }
                    }
                }
            }

            return false;
        }
        
        public Node[] CreatePath(bool horizontal)
        {
            List<Node> path = new List<Node>();
            Node node = horizontal ? H(endPos) : V(endPos);
            while (node.pos != startPos)
            {
                path.Add(node);
                node = node.parent;
            }
            path.Reverse();
            return path.ToArray();
        }

        public void FillNodes()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    FillNode(hNodes[x, y]);
                    FillNode(vNodes[x, y]);
                }
            }
        }

        private void FillNode(Node node)
        {
            Vec2[] directions = node.isHorizontal ? [Vec2.down, Vec2.up] : [Vec2.right, Vec2.left];
            
            foreach (Vec2 direction in directions)
            {
                Vec2 newPos = node.pos;
                long newHeat = 0;
                for (int i = 0; i <= 2; ++i)
                {
                    newPos += direction;
                    if (!InBounds(newPos)) continue;

                    Node newNode = node.isHorizontal ? V(newPos) : H(newPos);
                    newHeat += newNode.heat;

                    node.neighbours.Add(new (newNode, newHeat));
                }
            }
        }        
    }
}
