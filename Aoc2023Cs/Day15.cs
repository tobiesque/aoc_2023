using Rock.Collections;

namespace Aoc2023Cs;

public class Day15
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] entries = File.ReadAllText("15".GetInputFile(test: false)).Split(',');

        if (partOne) PartOne(entries); else PartTwo(entries);
    }

    public static void PartTwo(string[] entries)
    {
        Dictionary<long, Box> boxes = new ();

        Box.Dump(boxes.Values);
        
        foreach (string entry in entries)
        {
            Console.WriteLine(entry);
            if (entry.Contains('-'))
            {
                string label = entry[..^1];
                long boxNumber = Hash(label);
                if (boxes.TryGetValue(boxNumber, out var box))
                {
                    box.Remove(label);
                }
            }
            else if (entry.Contains('='))
            {
                var values = entry.Split('=');
                string label = values[0];
                long boxNumber = Hash(label);
                long focal = long.Parse(values[1]);

                if (!boxes.TryGetValue(boxNumber, out var box))
                {
                    box = new Box(boxNumber);
                    boxes[boxNumber] = box;
                }

                box.Add(label, focal);
            }
            
        }
        Box.Dump(boxes.Values);
        Console.WriteLine($"Part Two: {Box.Score(boxes.Values)}");
    }
    
    public static void PartOne(string[] entries)
    {
        Console.WriteLine($"Part One: {entries.Sum(Hash)}");
    }
    
    public static long Hash(string s)
    {
        int value = 0;
        foreach (var c in s)
        {
            value += c;
            value *= 17;
            value %= 256;
        }
        return value;
    }

    public class Box
    {
        public long id;

        public readonly OrderedDictionary<string, long> lenses = new ();

        public void Add(string label, long focal) => lenses[label] = focal;
        public void Remove(string label) => lenses.Remove(label);

        public Box(long id) => this.id = id;

        public long Score()
        {
            long score = 0;
            int i = 1;
            foreach (var (_, lens) in lenses)
            {
                long result = id + 1;
                result *= i;
                result *= lens;
                score += result;
                ++i;
            }
            return score;
        }

        public static long Score(IEnumerable<Box> boxes) => boxes.Sum(box => box.Score());

        public override string ToString()
        {
            if (lenses.Count == 0) return "";
            string lensesList = lenses.MakeList("][");
            return $"Box {id}: {lensesList}";
        }

        public static void Dump(IEnumerable<Box> boxes)
        {
            foreach (var box in boxes)
            {
                string boxPrint = box.ToString();
                if (string.IsNullOrWhiteSpace(boxPrint)) continue;
                Console.WriteLine(boxPrint);
            }
        }
    }
}