using System.Diagnostics;

namespace Aoc2023Cs;

using Vec2 = g3.Vector2d;
using Box2 = g3.AxisAlignedBox2d;
using Vec3 =g3.Vector3d;
using Line2 =g3.Line2d;
using Line3 =g3.Line3d;

public class Day24
{
    public static void Run()
    {
        bool test = false;
        var lines = Day.Str.ReadLinesArray(test: test);
        Weather weather = new(lines);
        weather.area = test
            ? new(7, 7, 27, 27)
            : new(200000000000000, 200000000000000, 400000000000000, 400000000000000);
        Console.WriteLine(weather);
        long result = weather.Process();
        Console.WriteLine($"Day One: {result}");
    }

    public class Weather
    {
        public class Hailstone(double[] pos, double[] speed, int id)
        {
            public Vec3 pos = new(pos[0], pos[1], pos[2]);
            public Vec3 speed = new(speed[0], speed[1], speed[2]);
            public int id = id;

            public override string ToString() => $"[{id}] {pos}, {speed}";
        }

        public List<Hailstone> hailstones = new();
        public Box2 area;

        public Weather(string[] lines)
        {
            Vec2 dim = Vec2.Zero;
            int i = 0;
            foreach (var split in lines.Select(l => l.Split('@')))
            {
                Hailstone stone = new(split[0].Split(',').Select(double.Parse).ToArray(),
                                          split[1].Split(',').Select(double.Parse).ToArray(),
                                          i++);
                hailstones.Add(stone);
                dim = dim.Max(stone.pos.ToVec2());
            }
        }

        public long Process()
        {
            List<Line2> lines = new(hailstones.Count);
            foreach (var stone in hailstones)
            {
                Line2 line = new(stone.pos.ToVec2(), stone.speed.ToVec2());
                lines.Add(line);
            }

            long result = 0;
            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                for (var j = i+1; j < lines.Count; j++)
                {
                    var line2 = lines[j];
                    // Console.WriteLine($"{line.ToStr()} / {line2.ToStr()}");
                    var s = line.IntersectionPointProjected(ref line2);
                    if (s == double.MaxValue)
                    {
                        // Console.WriteLine($"    Parallel");
                        continue;
                    }
                    var s2 = line2.IntersectionPointProjected(ref line);
                    Vec2 intersection = line.Origin + s * line.Direction;
                    // Console.WriteLine($"    {intersection} : {s}/{s2}");
                    if ((s < 0) || (s2 < 0)) continue;
                    if (area.Contains(intersection))
                    {
                        // Console.WriteLine($"    ->{area.ToStr()}");
                        ++result;
                    }
                }
            }
            return result;
        }
        
        public override string ToString()
        {
            string stones = hailstones.Select(h => h.ToString()).MakeList("\n");
            return $"{area.ToStr()}\n{stones}";
        }
    }
}