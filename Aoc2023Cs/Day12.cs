using System.Diagnostics;

namespace Aoc2023Cs;

public class Day12
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "12".ReadLinesArray(test: false);

        ulong result = 0;
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
                conditions = string.Join("?", Enumerable.Repeat(conditions_, 5));
                groups = groupList.Repeat(5).ToArray();
            }
            
            Console.WriteLine($"[[{conditions} {groups.MakeList()}]]");
            ulong numArrangements = Evaluate(conditions.ToCharArray(), groups);
            Console.WriteLine($" -> {numArrangements} arrangements found\n");
            result += numArrangements;
        }
        
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }

    public class Entry
    {
        public string s;
        public int[] groups;
        public ulong hashGroup;

        public override int GetHashCode()
        {
            var groupsHash = groups.Aggregate(0, (x,y) => HashCode.Combine(x.GetHashCode(), y.GetHashCode()));
            return HashCode.Combine(s.GetHashCode(), groupsHash, hashGroup.GetHashCode());
        }

        public class EqualityComparer : IEqualityComparer<Entry> {

            public bool Equals(Entry? x, Entry? y) => (x!.s == y!.s) && (x!.hashGroup == y!.hashGroup);
            public int GetHashCode(Entry x) => x.GetHashCode();
        }        
    } 
    
    public static Dictionary<Entry, ulong> cacheDot = new(new Entry.EqualityComparer());
    public static Dictionary<Entry, ulong> cacheHash = new(new Entry.EqualityComparer());

    public static ulong Evaluate(char[] s, Span<int> groups)
    {
        cacheDot.Clear();
        cacheHash.Clear();
        return Arrangement(s, groups, 0, 0UL);
    }
    
    public static ulong Arrangement(char[] s, Span<int> groups, int currentHashGroup, ulong key)
    {
        Debug.Assert(groups.Length != 0);

        int hashGroup = currentHashGroup;

        while (groups.Length > 0)
        {
            int currentGroup = groups[0];

            int i;
            for (i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                if (c == '.')
                {
                    if (hashGroup > 0) break;
                } else if (c == '#')
                {
                    ++hashGroup;
                } else if (c == '?')
                {
                    ulong result = 0;
                    ulong newKey = key + (ulong)i;
                    
                    char[] s1 = s[i..];
                    s1[0] = '.';
                    Entry s1s = new (){ s = new string(s1), groups = groups.ToArray(), hashGroup = (ulong)hashGroup };
                    if (!cacheDot.TryGetValue(s1s, out ulong result1))
                    {
                        result1 = Arrangement(s1, groups, hashGroup, newKey);
                        cacheDot.Add(s1s, result1);
                    }
                    result += result1;

                    char[] s2 = s[i..];
                    s2[0] = '#';
                    Entry s2s = new (){ s = new string(s2), groups = groups.ToArray(), hashGroup = (ulong)hashGroup };
                    if (!cacheHash.TryGetValue(s2s, out ulong result2))
                    {
                        result2 = Arrangement(s2, groups, hashGroup, newKey);
                        cacheHash.Add(s2s, result2);
                    }
                    result += result2;
                    
                    return result;
                }
            }

            if (hashGroup != currentGroup) return 0UL;

            hashGroup = 0;
            s = s[i..];
            groups = groups[1..];
            if (groups.Length == 0) return !s.Contains('#') ? 1UL : 0UL;
        }            
        return 0UL;
    }
}
