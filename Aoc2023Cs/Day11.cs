namespace Aoc2023Cs;

using System.Diagnostics;
using System.Text;

public class Day11
{
    public static bool partOne = true;

    public class Map
    {
        public long width = 0;
        public long height = 0;
        
        public List<Vec2L> galaxiesOrdered = new();
        public HashSet<Vec2L> galaxies = new();

        public Map(long width, long height)
        {
            this.width = width;
            this.height = height;
        }

        public Map(string[] mapLines)
        {
            width = mapLines[0].Length;
            height = mapLines.Length;
            
            for (int y = 0; y < mapLines.Length; y++)
            {
                string line = mapLines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        Vec2L pos = new(x, y);
                        galaxies.Add(pos);
                        galaxiesOrdered.Add(pos);
                    }
                }
            }
        }

        public Map Expand(long size)
        {
            List<long> warpY = new ();
            long newY = 0;
            for (int y = 0; y < height; ++y)
            {
                if (galaxies.All(p => (p.y != y)))
                {
                    ++newY;
                    warpY.Add(y);
                }
            }
            
            List<long> warpX = new ();
            long newX = 0;
            for (int x = 0; x < width; ++x)
            {
                if (galaxies.All(p => (p.x != x)))
                {
                    ++newX;
                    warpX.Add(x);
                }
            }
            
            Console.WriteLine($"WarpX: {warpX.MakeList()}");
            Console.WriteLine($"WarpY: {warpY.MakeList()}");
            
            Map newMap = new(width + newX, height + newY);
            foreach (Vec2L galaxy in galaxiesOrdered)
            {
                long warpedX = warpX.Warp(galaxy.x, size);
                long warpedY = warpY.Warp(galaxy.y, size);
                Vec2L pos = new(warpedX, warpedY);
                newMap.galaxies.Add(pos);
                newMap.galaxiesOrdered.Add(pos);
            }
            
            return newMap;
        }

        public override string ToString()
        {
            StringBuilder sb = new ();
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    Vec2L pos = new(x, y);
                    bool isGalaxy = galaxies.Contains(pos);
                    if (!isGalaxy)
                    {
                        sb.Append('.');
                        continue;
                    }
                    sb.Append((char)(galaxiesOrdered.IndexOf(pos) + 1 + '0'));
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] mapLines = "11".ReadLinesArray(test: false);

        Map originalMap = new(mapLines);
        // Console.WriteLine($"{originalMap}\n");
        Map map = originalMap.Expand(partOne ? 2L : 1000000L);
        // Console.WriteLine($"{map}");

        Vec2L[] galaxies = map.galaxiesOrdered.ToArray();
        Console.WriteLine($"{galaxies.Length} galaxies");

        long result = 0;
        int n = 0;
        for (int i = 0; i < galaxies.Length; ++i)
        {
            Vec2L a = galaxies[i];
            for (int j = i + 1; j < galaxies.Length; ++j)
            {
                Debug.Assert(i != j);
                Vec2L b = galaxies[j];
                long distance = a.Distance(galaxies[j]);
                result += distance;
                ++n;
            }
        }
        Debug.Assert(n == (galaxies.Length-1)*galaxies.Length/2);
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }
}

public static class MapExt
{
    public static long Warp(this IEnumerable<long> table, long value, long size)
    {
        long shift = 0;
        foreach (long entry in table)
        {
            if (entry > value) break;
            ++shift;
        }
        return value + (shift * (size-1));
    }
}
