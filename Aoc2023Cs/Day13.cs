using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day13
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "13".ReadLinesArray(test: false);

        Map[] maps = Read(lines);

        int result = 0;
        foreach (Map map in maps)
        {
            int scoreH = map.FindMirroringRows();
            int scoreV = map.FindMirroringColumns();
            if (scoreH > 0)
            {
                Debug.Assert(scoreV == 0);
                result += scoreH * 100;
                Console.WriteLine($"ScoreH = {scoreH * 100}");
            }
            else
            {
                Debug.Assert(scoreV != 0);
                result += scoreV;
                Console.WriteLine($"ScoreV = {scoreV}");
            }

            Console.WriteLine();
        }
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }
    
    public struct Map
    {
        public Map(int width, int height)
        {
            map = new char[width, height];
        }
        
        public char[,] map;

        int width => map.GetLength(0);
        int height => map.GetLength(1);
        
        public override string ToString()
        {
            StringBuilder sb = new() { Capacity = width * (height + 1) };
            for (int y = 0; y < height; ++y)
            {
                sb.Append(Row(y));
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public string Column(int x)
        {
            StringBuilder sb = new(height);
            for (int y = 0; y < height; ++y)
            {
                sb.Append(map[x, y]);
            }
            return sb.ToString();
        }

        public string Row(int y)
        {
            StringBuilder sb = new(width);
            for (int x = 0; x < width; ++x)
            {
                sb.Append(map[x, y]);
            }
            return sb.ToString();
        }

        public int CompareColumns(int x)
        {
            int result = 0;
            for (int i = 0; i < width; ++i)
            {
                int posLeft = x - i;
                int posRight = x + i + 1;
                Debug.Assert(posLeft != posRight);
                Debug.Assert(posRight - posLeft == (2 * i) + 1);
                if ((posLeft < 0) || (posLeft >= width)) return result;
                if ((posRight < 0) || (posRight >= width)) return result;
                string left = Column(posLeft);
                string right = Column(posRight);
                if (left != right) return 0;
                result = x + 1;
                Console.WriteLine($"    [{posLeft:000}] [{posRight:000}] {left}");
            }

            Debug.Fail("");
            return width;
        }
        
        public int CompareRows(int y)
        {
            int result = 0;
            for (int i = 0; i < height; ++i)
            {
                int posUp = y - i;
                int posDown = y + i + 1;
                Debug.Assert(posUp != posDown);
                Debug.Assert(posDown - posUp == (2 * i) + 1);
                if ((posUp < 0) || (posUp >= height)) return result;
                if ((posDown < 0) || (posDown >= height)) return result;
                var up = Row(posUp);
                var down = Row(posDown);
                if (up != down) return 0;
                result = y + 1;
                Console.WriteLine($"    [{posUp:000}] [{posDown:000}] {up}");
            }

            Debug.Fail("");
            return height;
        }
        
        public int FindMirroringColumns()
        {
            int result = 0;
            for (int x = 0; x < width; ++x)
            {
                int resultX = CompareColumns(x);
                result = Math.Max(result, resultX);
                if (resultX > 0)
                {
                    Console.WriteLine($"  Y[{x}] {resultX} @ {x + 1}");
                }
            }
            return result;
        }
        
        public int FindMirroringRows()
        {
            int result = 0;
            for (int y = 0; y < height; ++y)
            {
                int resultY = CompareRows(y);
                result = Math.Max(result, resultY);
                if (resultY > 0)
                {
                    Console.WriteLine($"  Y[{y}] {result} @ {y + 1}");
                }
            }
            return result;
        }
    }

    public static Map[] Read(string[] lines)
    {
        List<Map> result = new();
        foreach (var block in lines.Split(string.IsNullOrWhiteSpace))
        {
            int width = block[0].Length;
            int height = block.Length;
            Map map = new(width, height);
            result.Add(map);

            int y = 0;
            foreach (string line in block)
            {
                for (int x = 0; x < width; ++x) map.map[x,y] = line[x];
                ++y;
            }
        }
        
        return result.ToArray();
    }
}
