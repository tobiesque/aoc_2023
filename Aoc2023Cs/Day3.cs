namespace Aoc2023Cs;

using System.Diagnostics;
using System.Text;

using Vec2 = Util2d.Vec2<int>;
using Line2 = Util2d.Line2<int>;

public static class Day3
{
    public class Part
    {
        public char symbol;
        public Vec2 position;
        public HashSet<PartNumber> numbers = new();
        
        public Part(char symbol, Vec2 position)
        {
            this.symbol = symbol;
            this.position = position;
        }

        public bool Adjacent(PartNumber partNumber)
        {
            int dY = int.Abs(position.y - partNumber.line.from.y);
            if (dY > 1) return false;
            
            int dX1 = partNumber.line.from.x - position.x;
            int dX2 = partNumber.line.to.x - position.x;

            if ((dX1 <= 0) && (dX2 >=0)) return true;
            if (int.Abs(dX1) <= 1) return true;
            if (int.Abs(dX2) <= 1) return true;
            
            return false;
        }

        public override string ToString() => $"{symbol} @ {position}";
    }

    public class PartNumber : IEqualityComparer<PartNumber>
    {
        public PartNumber(Vec2 from, Vec2 to, int number) : this(new(from, to), number) {}
        
        public PartNumber(Line2 line, int number)
        {
            this.number = number;
            this.line = line;
            Debug.Assert(line.IsAxisParallel());
        }
        
        public Line2 line { get; }
        public int number { get; }

        public override int GetHashCode() => line.GetHashCode();
        public int GetHashCode(PartNumber obj) => obj.line.GetHashCode();

        public override bool Equals(object? obj) => line.Equals(((PartNumber)obj!).line);
        public bool Equals(PartNumber? x, PartNumber? y) => x!.line.Equals(y!.line);

        public override string ToString() => $"{number} @ {line}";
    }

    public class World
    {
        public List<Part> parts = new();
        public HashSet<PartNumber> partNumbers = new();

        public World(Span<string> lines)
        {
            StringBuilder number = new();
            int y = 0;
            Vec2 numberStart = new(0, 0);
            foreach (var line in lines)
            {
                int x = 0;
                foreach (char c in line)
                {
                    if (Char.IsDigit(c))
                    {
                        if (number.Length == 0)
                        {
                            numberStart = new(x, y);
                        }
                        number.Append(c);
                    } else {
                        // flush number
                        if (number.Length != 0)
                        {
                            int numberInt = int.Parse(number.ToString());
                            Vec2 numberEnd = new(x-1, numberStart.y);
                            PartNumber partNumber = new(numberStart, numberEnd, numberInt);
                            partNumbers.Add(partNumber);
                            number.Clear();
                        }
                        
                        if (c != '.')
                        {
                            Part part = new(c, new(x, y));
                            parts.Add(part);
                        }
                    }
                    ++x;
                }
                ++y;
            }
        }

        public void ConnectPartNumbers()
        {
            foreach (var partNumber in partNumbers)
            {
                foreach (Part part in parts)
                {
                    if (part.Adjacent(partNumber))
                    {
                        part.numbers.Add(partNumber);
                    }
                }
            }
        }
        
        public override string ToString()
        {
            StringBuilder result = new();
            result.AppendLine("Parts:");
            foreach (Part part in parts)
            {
                result.AppendLine($"\t{part.symbol} @ {part.position}");
                foreach (PartNumber partNumber in part.numbers)
                {
                    result.AppendLine($"\t\t{partNumber.number,-8:D} @ {partNumber.line}");
                }
            }
            
            result.AppendLine("Part Numbers:");
            foreach (var partNumber in partNumbers)
            {
                result.AppendLine($"\t{partNumber.number,-8:D} @ {partNumber.line}");
            }

            return result.ToString();
        }
    }

    public static void Run()
    {
        World world = new("3".ReadLinesArray(test: false));
        world.ConnectPartNumbers();
        Console.WriteLine(world.ToString());

        int resultPartOne = 0;
        int resultPartTwo = 0;
        foreach (var part in world.parts)
        {
            if ((part.symbol == '*') && (part.numbers.Count == 2))
            {
                resultPartTwo += part.numbers.Aggregate(1, (n, p) => p.number * n);
            }
            foreach (var partNumber in part.numbers)
            {
                resultPartOne += partNumber.number;
            }
        }
        Console.WriteLine($"Part One: {resultPartOne}");
        Console.WriteLine($"Part Two: {resultPartTwo}");
    }
}