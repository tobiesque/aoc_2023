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

        if (partOne)
        {
            grid.minMoves = 1;
            grid.maxMoves = 3;
        }
        else
        {
            grid.minMoves = 4;
            grid.maxMoves = 10;
        }
        
        grid.FillNodes();

        Console.WriteLine(grid);
        
        long result = long.MaxValue;
        {
            grid.Reset();
            grid.Walk(true);
            result = long.Min(grid.CalcPath(true), result);
            result = long.Min(grid.CalcPath(false), result);
        }
        {
            grid.Reset();
            grid.Walk(false);
            result = long.Min(grid.CalcPath(true), result);
            result = long.Min(grid.CalcPath(false), result);
        }

        string partName = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partName}: {result}");
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
                    hNodes[x, y] = new Node(new (x, y), heat, true);
                    vNodes[x, y] = new Node(new (x, y), heat, false);
                }
            }

            startPos = new(0, 0);
            endPos = new(width - 1, height - 1);
        }

        public Vec2 startPos;
        public Vec2 endPos;

        public int minMoves;
        public int maxMoves;
        
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

        public class Node
        {
            public Vec2 pos;
            public readonly long heat;
            public readonly bool isHorizontal; // for hashcode diff between hNodes and vNodes

            public struct Neighbour(Node node, long heat)
            {
                public Node node = node;
                public long heat = heat;
            }
            
            public List<Neighbour> neighbours = new();
            
            public long g;
            public long h;
            public long f => g + h;
            public Node? parent;
            
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

        private Node MinF(IEnumerable<Node> open)
        {
            Node minF = open.First();
            foreach (var node in open.Skip(1))
            {
                if ((node.f < minF.f) || ((node.f == minF.f) && (node.h < minF.h)))
                {
                    minF = node;
                }
            }
            return minF;
        }

        public void Reset()
        {
            open = new ();
            closed = new ();
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    hNodes[x, y].g = 0;
                    hNodes[x, y].h = hNodes[x, y].pos.Distance(endPos);
                    hNodes[x, y].parent = null;
                    
                    vNodes[x, y].g = 0;
                    vNodes[x, y].h = vNodes[x, y].pos.Distance(endPos);
                    vNodes[x, y].parent = null;
                }
            }
        }
        
        public HashSet<Node> open;
        public HashSet<Node> closed;
        
        public bool Walk(bool horizontal)
        {
            open.Add(horizontal ? H(startPos) : V(startPos));
            
            while (open.Count > 0)
            {
                Node node = MinF(open);
                Debug.Assert(node != null);
                if (node.pos == endPos) return true;
                
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
                        open.Add(neighbour.node);
                    }
                }
            }

            Console.WriteLine("No solution");
            return false;
        }
        
        public long CalcPath(bool horizontal)
        {
            List<Node> path = new List<Node>();
            Node? node = horizontal ? H(endPos) : V(endPos);
            while (node.pos != startPos)
            {
                path.Add(node);
                if (node.parent == null)
                {
                    return long.MaxValue; 
                }
                node = node.parent;
            }
            return path.Max(p => p.g);
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
                for (int i = 1; i <= maxMoves; ++i)
                {
                    newPos += direction;
                    if (!InBounds(newPos)) continue;

                    Node newNode = node.isHorizontal ? V(newPos) : H(newPos);
                    newHeat += newNode.heat;

                    if (i >= minMoves)
                    {
                        node.neighbours.Add(new (newNode, newHeat));
                    }
                }
            }
        }        
    }
}
