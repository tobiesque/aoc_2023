using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

public class Day13
{
    public static void Run(int part)
    {
        string[] lines = "13".ReadLinesArray(test: false);
        DoRun(Read(lines));
    }

    public static void Toggle(ref char c) => c = (c == '#') ? '.' : '#';
    
    public static void DoRun(Map[] maps)
    {
        int resultPartOne = 0;
        int resultPartTwo = 0;
        foreach (Map map in maps)
        {
            
            int regularRankH = 0;
            int regularRankV = 0;
            {
                int scoreH = map.FindMirroringRows(out int rankH, -1, silent: true);
                int scoreV = map.FindMirroringColumns(out int rankV, -1, silent: true);
                if (scoreH > 0)
                {
                    Debug.Assert(scoreV == 0);
                    regularRankH = rankH;
                    regularRankV = -1;
                    
                    resultPartOne += scoreH * 100;
                }
                else
                {
                    Debug.Assert(scoreV != 0);
                    regularRankV = rankV;
                    regularRankH = -1;
                    
                    resultPartOne += scoreV;
                }
            }
            
            Console.WriteLine($"Original: {regularRankH}/{regularRankV}");
            
            int maxResultSmudge = 0;
            int maxScore = 0;
            Vec2 maxPos = new Vec2();
            for (int y = 0; y < map.height; ++y)
            {
                for (int x = 0; x < map.width; ++x)
                {
                    Vec2 pos = new Vec2(x, y);
                    int resultSmudge = 0;
                    int resultScore = 0;
                    
                    Toggle(ref map.map[x, y]);
                    int scoreH = map.FindMirroringRows(out int rankH, regularRankH, silent: true);
                    int scoreV = map.FindMirroringColumns(out int rankV, regularRankV, silent: true);
                    Toggle(ref map.map[x, y]);

                    bool isH = (regularRankH > 0) && (regularRankH == rankH);
                    if (scoreH > 0)
                    {
                        if (!isH)
                        {
                            resultSmudge = scoreH;
                            resultScore = scoreH * 100;
                        }
                        else
                        {
                            Console.WriteLine($"  skipping {pos} - it's the original {rankH}/{regularRankH}");
                        }
                    }
                    
                    if ((scoreH <= 0) || isH)
                    {
                        bool isV = (regularRankV > 0) && (regularRankV == rankV);
                        if (isV)
                        {
                            Console.WriteLine($"  skipping {pos} - it's the original {rankV}/{regularRankV}");
                            continue;
                        }
                        resultSmudge = scoreV;
                        resultScore = scoreV;
                    }

                    if (resultSmudge > maxResultSmudge)
                    {
                        maxResultSmudge = resultSmudge;
                        maxScore = resultScore;
                        maxPos = pos;
                    }
                }
            }

            if (maxResultSmudge > 0)
            {
                char[] smudgedColumn = map.Column(maxPos.x).ToCharArray();
                Toggle(ref smudgedColumn[maxPos.y]);
                Console.WriteLine($"Smudged {maxPos+new Vec2(1, 1)}: {new string(smudgedColumn)}");

                resultPartTwo += maxScore;
            }
            else
            {
                Console.WriteLine(map.ToString());
                Debug.Fail("No smudge!");
            }
        }
        
        Console.WriteLine($"Part One: {resultPartOne}");
        Console.WriteLine($"Part Two: {resultPartTwo}");
    }

    public struct Map
    {
        public Map(int width, int height)
        {
            map = new char[width, height];
        }
        
        public char[,] map;

        public int width => map.GetLength(0);
        public int height => map.GetLength(1);
        
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

        public int CompareColumns(int x, bool silent = false)
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
                if (!silent) Console.WriteLine($"    [{posLeft:000}] [{posRight:000}] {left}");
            }

            Debug.Fail("");
            return width;
        }
        
        public int CompareRows(int y, bool silent = false)
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
                if (!silent) Console.WriteLine($"    [{posUp:000}] [{posDown:000}] {up}");
            }

            Debug.Fail("");
            return height;
        }
        
        public int FindMirroringColumns(out int rank, int except, bool silent = false)
        {
            int result = 0;
            rank = 0;
            for (int x = 0; x < width; ++x)
            {
                int resultX = CompareColumns(x, silent);
                if (resultX > 0)
                {
                    if ((x + 1) != except)
                    {
                        rank = x + 1;
                        result = Math.Max(result, resultX);
                        if (!silent) Console.WriteLine($"  Y[{x}] {resultX} @ {rank}");
                    }
                }
            }
            return result;
        }
        
        public int FindMirroringRows(out int rank, int except, bool silent = false)
        {
            int result = 0;
            rank = 0;
            for (int y = 0; y < height; ++y)
            {
                int resultY = CompareRows(y, silent);
                if (resultY > 0)
                {
                    if ((y + 1) != except)
                    {
                        rank = y + 1;
                        result = Math.Max(result, resultY);
                        if (!silent) Console.WriteLine($"  Y[{y}] {result} @ {rank}");
                    }
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
