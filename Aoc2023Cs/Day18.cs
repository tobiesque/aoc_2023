﻿using System.Diagnostics;
using System.Text;

namespace Aoc2023Cs;

using Vec2 = Vec2L;

public class Day18
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "18".ReadLinesArray(test: true);

        Dig dig = new(lines);
        Console.WriteLine(dig);
        long result = dig.Count();
        
        string partName = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partName}: {result}");
        
    }

    public class Dig
    {
        public class Span
        {
            public readonly Vec2 from;
            public readonly Vec2 to;
            public bool IsHorizontal => (from.x != to.x);
            public long Length => (from.x != to.x) ? (to.x - from.x) : (to.y - from.y);
            public long LengthH => Math.Abs(to.x - from.x);
            public long LengthV => Math.Abs(to.y - from.y);

            public bool IsConnectedTo(Span other)
            {
                return (other.from == from) || (other.to == from) || (other.from == to) || (other.to == to);
            }
            
            public Span(Vec2 from, Vec2 to)
            {
#if false                
                if ((from.x > to.x) || (from.y > to.y)) (from, to) = (to, from);
#endif
                if (from.y != to.y)
                {
                    to.y -= long.Sign(to.y - from.y);
                }

                if (from.x != to.x)
                {
                    to.x -= long.Sign(to.x - from.x);
                }
                
                this.from = from;
                this.to = to;
            }

            public override int GetHashCode() => HashCode.Combine(from, to);

            public override string ToString()
            {
                string dirLabel = IsHorizontal ? "HSpan" : "VSpan";
                return $"{dirLabel}: {from} {to}";
            }
        }

        public List<Span> spans = new ();
        public SortedList<long, List<Span>> spanLines = new ();

        public void Add(Vec2 from, Vec2 to)
        {
            Span span = new(from, to);
            spans.Add(span);
            
            long[] linesToAdd = span.IsHorizontal ? [span.from.y] : [span.from.y, span.to.y];
            foreach (long line in linesToAdd)
            {
                spanLines.MultiAdd(line, span);
                Console.WriteLine($"[{line}] {span}");
            }
        }

        public Vec2 Decode(string coded)
        {
            long len = Convert.ToInt64(coded[2..7], 16);
            int dirI = Convert.ToInt32(coded[7..8], 16);
            switch (dirI)
            {
                case 0: return Vec2.Right * len;
                case 1: return Vec2.Down * len;
                case 2: return Vec2.Left * len;
                case 3: return Vec2.Up * len;
            }
            Debug.Fail("");
            return Vec2.Zero;
        }
        
        Vec2 min;
        Vec2 max;
        
        public Dig(string[] lines)
        {
            min = Vec2.MaxValue;
            max = Vec2.MinValue;
            Vec2 pos = Vec2.Zero;
            foreach (string line in lines)
            {
                min = min.Min(pos);
                max = max.Max(pos);
                var parts = line.Split(' ');
                Vec2 dir;
                if (partOne)
                {
                    dir = FromDir(parts[0][0]) * long.Parse(parts[1]);
                }
                else
                {
                    dir = Decode(parts[2]);
                }

                Add(pos, pos + dir);
                pos += dir;
            }
        }

        public long IterateAreas()
        {
            SortedList<long, Span> horizontals = new();
            SortedList<long, Span> verticals = new();
            
            long area = 0L;
            foreach (var line in spans)
            {
                if (line.IsHorizontal)
                {
                    horizontals.Add(line.from.x, line);
                }
                else
                {
                    verticals.Add(line.from.y, line);
                }
            }

            bool fillVertical = true;
            foreach (var (_, vertical) in verticals)
            {
                bool fillHorizontal = false;
                foreach (var (_, horizontal) in horizontals)
                {
                    area += Visit(horizontal, vertical, ref fillHorizontal, ref fillVertical);
                }
            }
            return area;
        }
        
        public long Visit(Span horizontal, Span vertical, ref bool fillHorizontal, ref bool fillVertical)
        {
            if (horizontal.IsHorizontal)
            fillHorizontal = !fillHorizontal;
            if (fillHorizontal && fillVertical)
            {
                return horizontal.LengthH * vertical.LengthV;
            }
            return 0L;
        }

        public long Count()
        {
            long result = 0;

            long lastVSpanResult = 0;
            long lastY = spanLines.Keys.First()-1;
            SortedList<long, Span> activeSpans = new ();
            foreach ((long y, List<Span> spans) in spanLines)
            {
                long catchUp = (y - lastY) * lastVSpanResult;
                if (lastVSpanResult != 0)
                {
                    Console.WriteLine($"[{lastY+1}-{y}] : {lastVSpanResult} = {catchUp}");
                }
                // accumulate vspans in last lines 
                result += catchUp;

                long lineResult = 0;

                foreach (Span span in spans)
                {
                    if (!activeSpans.Remove(span.from.x))
                    {
                        activeSpans.Add(span.from.x, span);
                    };
                }

                lastVSpanResult = 0;
                bool fill = false;
                long lastX = 0;
                foreach ((long x, var aSpan) in activeSpans)
                {
                    if (fill)
                    {
                        lastVSpanResult += x - lastX + 1;
                    }

                    fill = !fill;

                    lastX = x;
                }

                lineResult += lastVSpanResult;
                Console.WriteLine($"[{y}] : {lineResult}");
                result += lineResult;

                foreach (Span span in spans.Where(span => span.IsHorizontal))
                {
                    activeSpans.Remove(span.from.x);
                }

                
                lastY = y;
            }
            
            Debug.Assert(activeSpans.Count == 0);
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.AppendLine($"size: {max-min} ({min} - {max}), {spanLines.Count} spans");
            return sb.ToString();
        }
        
        public static Vec2 FromDir(char dir)
        {
            if (dir == 'R') return Vec2.Right;
            if (dir == 'D') return Vec2.Down;
            if (dir == 'L') return Vec2.Left;
            if (dir == 'U') return Vec2.Up;
            return Vec2.Zero;
        }
    }
}
