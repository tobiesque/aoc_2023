using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day22
{
    public static void Run()
    {
        var lines = Day.Str.ReadLinesArray(test: false);
        Pile.verbose = false;
        
        Pile pile = new(lines);
        Console.WriteLine($"Dimension: {pile.dimension.from}-{pile.dimension.to}, {pile.dimensionZ.x}-{pile.dimensionZ.y}");

        if (Pile.verbose)
        {
            Console.WriteLine(pile.ToStringX());
            Console.WriteLine(pile.ToStringY());
        }

        pile.Settle();

        if (Pile.verbose)
        {
            Console.WriteLine(pile.ToStringX());
            Console.WriteLine(pile.ToStringY());
        }

        pile.CalcSupport();
        var destroyable = pile.CalcDestroyable();
        
        Console.WriteLine($"Part One: {destroyable.Count}");
    }

    public class Brick
    {
        public Brick(int[] fromV, int[] toV, char name)
        {
            box = new Box2(new(fromV[0], fromV[1]), new(toV[0], toV[1]));
            highZ = Math.Max(fromV[2], toV[2]);
            lowZ = Math.Min(fromV[2], toV[2]);;
            this.name = name;
        }

        public bool IntersectsXy(Brick otherBrick) => box.Intersects(otherBrick.box);

        public bool IsPointIn(Vec2 pos, int z)
        {
            if (!box.IsPointIn(pos)) return false;
            return (z >= lowZ) && (z <= highZ);
        }

        public override int GetHashCode() => HashCode.Combine(box.GetHashCode(), highZ, lowZ);

        public Box2 box;
        public int highZ;
        public int lowZ;
        public char name;

        public HashSet<Brick> supports = new();
        public HashSet<Brick> supportedBy = new();

        public override string ToString() => $"{box}, {lowZ}-{highZ})";
    }
    
    public class Pile
    {
        public static bool verbose = true;
        
        public SortedList<int, HashSet<Brick>> bricksZ = new ();
        public List<Brick> brickList = new ();
        public Box2 dimension = Box2.MinMaxStartBox;
        public Vec2 dimensionZ = new(int.MaxValue, 0); 

        public Pile(string[] lines)
        {
            int i = 0;
            foreach (var line in lines)
            {
                var fromTo = line.Split('~');
                int[] from = fromTo[0].Split(',').Select(int.Parse).ToArray();
                int[] to = fromTo[1].Split(',').Select(int.Parse).ToArray();
                Brick brick = new(from, to, (char)('A' + i));
                
                // Console.WriteLine($"Brick {brick.name}: {brick}");
                dimension.from = brick.box.from.Min(dimension.from);
                dimension.to = brick.box.from.Max(dimension.to);
                dimensionZ.x = Math.Min(brick.lowZ, dimensionZ.x);
                dimensionZ.y = Math.Max(brick.highZ, dimensionZ.y);
                
                brickList.Add(brick);
                ++i;
            }
            CalcSortedList();
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

        public void Settle()
        {
            int i = 1;
            bool allSettled;
            do
            {
                if (Pile.verbose)
                {
                    Console.WriteLine($"Drop step {i}");
                }

                allSettled = true;
                foreach (var brick in brickList)
                {
                    bool supported = false;
                    HashSet<Brick>? bricksBelow = null;
                    if ((brick.lowZ - 1) <= 0)
                    {
                        supported = true;
                    }
                    else if (bricksZ.TryGetValue(brick.lowZ - 1, out bricksBelow))
                    {
                        supported = bricksBelow.Any(b => brick.IntersectsXy(b));
                    }

                    if (supported)
                    {
                        if (verbose)
                        {
                            string supportedBy = bricksBelow?.Select(b => b.name).MakeList() ?? "ground";
                            Console.WriteLine($"    Brick {brick.name} supported by {supportedBy}");
                        }
                    }
                    else
                    {
                        if (verbose)
                        {
                            Console.WriteLine($"    Brick {brick.name} falls from {brick.lowZ} to {brick.lowZ - 1}");
                        }

                        allSettled = false;
                        bricksZ.MultiRemove(brick.lowZ, brick);
                        bricksZ.MultiRemove(brick.highZ, brick);
                        --brick.lowZ;
                        --brick.highZ;
                        bricksZ.MultiAdd(brick.lowZ, brick);
                        bricksZ.MultiAdd(brick.highZ, brick);
                    }
                }
                ++i;
            } while (!allSettled);

            if (verbose)
            {
                Console.WriteLine("Drops done.\n");
            }
        }
        
        public void CalcSupport()
        {
            foreach(var brick in brickList)
            {
                if (verbose)
                {
                    Console.WriteLine($"Brick {brick.name} ({brick}):");
                }

                if (bricksZ.TryGetValue(brick.highZ + 1, out var bricksAbove))
                {
                    if (verbose)
                    {
                        Console.WriteLine($"    Bricks above: {bricksAbove.Select(b => b.name).MakeList()}");
                    }

                    foreach (Brick supportedBrick in bricksAbove.Where(b => brick.IntersectsXy(b)))
                    {
                        brick.supports.Add(supportedBrick);
                        supportedBrick.supportedBy.Add(brick);
                        if (verbose)
                        {
                            Console.WriteLine($"    supports brick {supportedBrick.name} ({supportedBrick})");
                        }
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
                    sb.Append(minBrick?.name ?? '.');
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}