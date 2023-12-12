namespace Aoc2023Cs;

public class Day12
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        // PumpConditionValid(".###....##.#".AsSpan(), new List<int> { 3, 2, 1 });
        
        partOne = (part == 1);
        string[] lines = "12".ReadLinesArray(test: false);

        long result = 0;
        foreach (string lineStr in lines)
        {
            List<int> groups = new();
            Span<char> line = lineStr.AsSpan();
            line.ExtractStringRef(out var conditions_);
            while (line.Length > 0)
            {
                line.ExtractIntRef(out int value).SkipWhite();
                groups.Add(value);
            }

            int repeats = partOne ? 1 : 5;
            string conditions = conditions_.Repeat(repeats);
            groups = groups.Repeat(repeats).ToList();
            
            Console.WriteLine($"{conditions} {groups.MakeList()}");

            int numArrangements = 0;
            int[] unknowns = conditions.Select((c, i) => Tuple.Create(c, i)).Where(t => (t.Item1=='?')).Select(t => t.Item2).ToArray();
            if (unknowns.Length > 0)
            {
                long bitSize = 1 << unknowns.Length;
                for (long i = 0; i < bitSize; ++i)
                {
                    long fixValue = i;
                    char[] arrangement = conditions.ToArray();
                    foreach (var unknownIndex in unknowns)
                    {
                        arrangement[unknownIndex] = ((fixValue & 1) == 1) ? '#' : '.';
                        fixValue /= 2;
                    }

                    if (PumpConditionValid(arrangement, groups))
                    {
                        ++numArrangements;
                    }
                }
                Console.WriteLine($" -> {numArrangements} arrangements found");
                result += numArrangements;
            }
            Console.WriteLine();
        }
        
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }

    public static bool PumpConditionValid(Span<char> conditions, IList<int> groups)
    {
        int groupSize = 0;
        int groupIndex = 0;
        for (int i = 0; i < conditions.Length; ++i)
        {
            if (conditions[i] == '#')
            {
                if ((groupSize == 0) && (groupIndex == groups.Count)) return false;
                ++groupSize;
            }
            else
            {
                if (groupSize == 0) continue;
                if (groups[groupIndex] != groupSize) return false;
                ++groupIndex;
                groupSize = 0;
            }
        }

        if (groupSize > 0)
        {
            if (groups[groupIndex] != groupSize) return false;
            ++groupIndex;
        }

        return (groupIndex == groups.Count);
    }
}
