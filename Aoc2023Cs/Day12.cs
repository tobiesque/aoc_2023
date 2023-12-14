using System.Text.RegularExpressions;

namespace Aoc2023Cs;

public class Day12
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        // PumpConditionValid(".###....##.#".AsSpan(), new List<int> { 3, 2, 1 });
        
        partOne = (part == 1);
        string[] lines = "12".ReadLinesArray(test: true);

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
            int[] groups;
            
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

            int count = 0;
            int numArrangements = Arrangement(conditions.ToCharArray(), groups, ref count);
            Console.WriteLine($" -> {numArrangements} arrangements found");
        }
        
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }

    public static int Arrangement(char[] s, IList<int> groups, ref int count)
    {
        ++count;
        
        int firstUnknown = Array.IndexOf(s, '?');
        if (firstUnknown < 0)
        {
            if (PumpConditionValid(s.AsSpan(), groups))
                return 1;
            else
                return 0;
        }

        if (!IsValid(s, groups))
        {
            return 0;
        }

        s[firstUnknown] = '.';
        int dotResult = Arrangement(s, groups, ref count);

        s[firstUnknown] = '#';
        int hashResult = Arrangement(s, groups, ref count);
        
        s[firstUnknown] = '?';

        return dotResult + hashResult;
    }

    public static Regex groupByHash = new(@"(\#+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    
    
    public static bool IsValid(char[] s, IList<int> groups)
    {
        MatchCollection matches = groupByHash.Matches(new (s));
        if (matches.Count == 0) return false;

        int maxGroups = groups.Max();
        int maxHashGroups = matches.Select(m => m.Groups.Count).Max();
        if (maxHashGroups > maxGroups)
        {
            return false;
        }

        /*
        int minHashGroups = matches.Select(m => m.Groups.Count).Min();
        if (minHashGroups < maxGroups)
        {
            return false;
        }
        */
        
        return true;
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
