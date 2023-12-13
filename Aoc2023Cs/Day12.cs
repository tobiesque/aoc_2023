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
            List<int> groupList = new();
            Span<char> line = lineStr.AsSpan();
            line.ExtractStringRef(out string conditions_);
            while (line.Length > 0)
            {
                line.ExtractIntRef(out int value).SkipWhite();
                groupList.Add(value);
            }

            string conditions;
            Span<int> groups;
            
            if (partOne)
            {
                conditions = conditions_;
                groups = groupList.ToArray();
            }
            else
            {
                var one = new [] { 1 };
                conditions = string.Join("?", Enumerable.Repeat(conditions_, 5));
                groups = groupList.Concat(groupList).Concat(one).
                                   Concat(groupList).Concat(one).
                                   Concat(groupList).Concat(one).
                                   Concat(groupList).Concat(one).
                                   Concat(groupList).ToArray();
            }
            
            Console.WriteLine($"{conditions} {groups.MakeList()}");

            conditions = Simplify(conditions.AsSpan(), groups);
            
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

                    if (PumpConditionValid(arrangement, groups.ToArray()))
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

    public static string Simplify(Span<char> s, Span<int> groups)
    {
        
        return new(s);
    }
    
    public static bool PumpConditionValid(Span<char> conditions, Span<int> groups)
    {
        int groupSize = 0;
        int groupIndex = 0;
        for (int i = 0; i < conditions.Length; ++i)
        {
            if (conditions[i] == '#')
            {
                if ((groupSize == 0) && (groupIndex == groups.Length)) return false;
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

        return (groupIndex == groups.Length);
    }
}
