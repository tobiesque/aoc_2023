using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day22
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: false);
        Pile.verbose = false;
        Pile.verbose2 = false;
        Pile.verbose3 = false;
        
        Pile pile = new(lines);
        pile.CalcSortedList();
        Console.WriteLine($"Dimension: {pile.dimension.from}-{pile.dimension.to}, {pile.dimensionZ.x}-{pile.dimensionZ.y}");

        if (Pile.verbose3)
        {
            Console.WriteLine(pile.ToStringX());
            Console.WriteLine(pile.ToStringY());
        }
        
        if (!Day.PartOne)
        {
            HashSet<Brick> falling = new();
            pile.Settle(falling);
            Console.WriteLine($"Initial step: {falling.Count} bricks fell");
            
            int result = 0;
            int i = 1;
            int numBricks = pile.brickList.Count;
            for (var j = 0; j < numBricks; j++)
            {
                Pile pile2 = new Pile(pile, j);
                pile2.CalcSortedList();
                
                falling.Clear();
                pile2.Settle(falling);

                if (falling.Any())
                {
                    Console.WriteLine($"Removing brick {pile.brickList[j].id} would cause {falling.Count} bricks to fall");
                }

                result += falling.Count;
            }

            Console.WriteLine($"Part Two: {result}");
            return;
        } 
        else 
        {
            pile.Settle();
        
            if (Pile.verbose)
            {
                Console.WriteLine(pile.ToStringX());
                Console.WriteLine(pile.ToStringY());
            }

            pile.CalcSupport();
            var destroyable = pile.CalcDestroyable();

            Console.WriteLine($"Part One: {destroyable.Count}");
            return;
        }
    }

    public class Brick
    {
        public Brick(int[] fromV, int[] toV, int id)
        {
            box = new Box2(new(fromV[0], fromV[1]), new(toV[0], toV[1]));
            highZ = Math.Max(fromV[2], toV[2]);
            lowZ = Math.Min(fromV[2], toV[2]);;
            this.id = id;
        }

        public Brick(Brick other)
        {
            box = other.box;
            highZ = other.highZ;
            lowZ = other.lowZ;
            id = other.id;
        }
        
        public bool IntersectsXy(Brick otherBrick) => box.Intersects(otherBrick.box);

        public bool IsPointIn(Vec2 pos, int z)
        {
            if (!box.IsPointIn(pos)) return false;
            return (z >= lowZ) && (z <= highZ);
        }

        public Box2 box;
        public int highZ;
        public int lowZ;
        public int id;

        public char Name => (char)('A' + id);
        
        public HashSet<Brick> supports = new();
        public HashSet<Brick> supportedBy = new();

        public override string ToString() => $"{box}, {lowZ}-{highZ})";
    }
    
    public class Pile
    {
        public static bool verbose = true;
        public static bool verbose2 = false;
        public static bool verbose3 = false;
        
        public SortedList<int, HashSet<Brick>> bricksZ = new ();
        public List<Brick> brickList = new ();
        public Box2 dimension = Box2.MinMaxStartBox;
        public Vec2 dimensionZ = new(int.MaxValue, 0);

        public Pile(Pile other, int brickToRemove)
        {
            for (var i = 0; i < other.brickList.Count; i++)
            {
                if (i == brickToRemove) continue;
                brickList.Add(new Brick(other.brickList[i]));
            }

            dimension = other.dimension;
            dimensionZ = other.dimensionZ;
        }

        public Pile(string[] lines)
        {
            int i = 0;
            foreach (var line in lines)
            {
                var fromTo = line.Split('~');
                int[] from = fromTo[0].Split(',').Select(int.Parse).ToArray();
                int[] to = fromTo[1].Split(',').Select(int.Parse).ToArray();
                Brick brick = new(from, to, i);
                
                dimension.from = brick.box.from.Min(dimension.from);
                dimension.to = brick.box.from.Max(dimension.to);
                dimensionZ.x = Math.Min(brick.lowZ, dimensionZ.x);
                dimensionZ.y = Math.Max(brick.highZ, dimensionZ.y);
                
                brickList.Add(brick);
                ++i;
            }
        }

        public void CalcSortedList()
        {
            bricksZ.Clear();
            foreach (var brick in brickList)
            {
                bricksZ.MultiAdd(brick.lowZ, brick);
                if (brick.lowZ != brick.highZ)
                {
                    bricksZ.MultiAdd(brick.highZ, brick);
                }
            }
        }

        // returns a collection of all falling bricks
        public bool SettleStep(ICollection<Brick>? allFalling = null)
        {
            List<Brick> falling = new ();
            foreach (var brick in brickList)
            {
                bool supported = false;
                HashSet<Brick>? bricksBelow = null;

                int zBelow = brick.lowZ - 1;
                if (zBelow == 0)
                {
                    supported = true;
                }
                else if (bricksZ.TryGetValue(zBelow, out bricksBelow))
                {
                    supported = bricksBelow.Any(b => (b.highZ == zBelow) && brick.IntersectsXy(b));
                }

                if (supported)
                {
                    if (verbose2)
                    {
                        string supportedBy = bricksBelow?.Select(b => b.Name).MakeList() ?? "ground";
                        Console.WriteLine($"    Brick {brick.Name} supported by {supportedBy}");
                    }
                }
                else
                {
                    if (verbose2) Console.WriteLine($"    Brick {brick.Name} falls from {brick.lowZ} to {brick.lowZ - 1}");
                    falling.Add(brick);
                }
            }

            foreach (var brick in falling)
            {
                allFalling?.Add(brick);
                --brick.lowZ;
                --brick.highZ;
            }
            CalcSortedList();
            
            return !falling.Any();
        }
        
        public void Settle(ICollection<Brick>? falling = null)
        {
            int i = 1;
            bool allSettled;
            
            do
            {
                if (verbose2) Console.WriteLine($"Drop step {i}");
                allSettled = SettleStep(falling);
                ++i;
            } while (!allSettled);

            if (verbose2) Console.WriteLine("Drops done.\n");
        }

        public void ClearSupport()
        {
            foreach (var brick in brickList)
            {
                brick.supports.Clear();
                brick.supportedBy.Clear();
            }
        }

        public void CalcSupport()
        {
            if (verbose3) Console.WriteLine("Calculating support...");
            ClearSupport();
            foreach(var brick in brickList)
            {
                if (verbose3) Console.WriteLine($"    Brick {brick.Name} ({brick}):");

                if (bricksZ.TryGetValue(brick.highZ + 1, out var bricksAbove))
                {
                    if (verbose3) Console.WriteLine($"        Bricks above: {bricksAbove.Select(b => b.Name).MakeList()}");

                    foreach (Brick supportedBrick in bricksAbove.Where(b => brick.IntersectsXy(b)))
                    {
                        brick.supports.Add(supportedBrick);
                        supportedBrick.supportedBy.Add(brick);
                        if (verbose3) Console.WriteLine($"        supports brick {supportedBrick.Name} ({supportedBrick})");
                    }
                }
            }
        }

        public List<Brick> CalcDestroyable()
        {
            List<Brick> destroyable = new();
            foreach (Brick brick in brickList)
            {
                if (brick.supports.All(b => b.supportedBy.Count > 1))
                {
                    destroyable.Add(brick);
                }
            }
            return destroyable;
        }

        public string ToStringX() => ToStringXy(v => v.x, v => v.y);
        public string ToStringY() => ToStringXy(v => v.y, v => v.x);

        public string ToStringXy(Func<Vec2, int> axis, Func<Vec2, int> otherAxis)
        {
            StringBuilder sb = new ();
            for (int z = dimensionZ.y; z > 0; --z)
            {
                for (int xy = axis(dimension.from); xy <= axis(dimension.to); ++xy)
                {
                    int minOtherAxis = int.MaxValue;
                    Brick? minBrick = null;
                    foreach (var brick in brickList)
                    {
                        if ((z < brick.lowZ) || (z > brick.highZ) || 
                            (xy < axis(brick.box.from)) || (xy > axis(brick.box.to))) continue;

                        var otherAxisValue = Math.Min(otherAxis(brick.box.from), otherAxis(brick.box.to));
                        if (otherAxisValue < minOtherAxis)
                        {
                            minOtherAxis = otherAxisValue;
                            minBrick = brick;
                        }
                    }
                    sb.Append(minBrick?.Name ?? '.');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}