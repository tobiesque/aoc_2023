// #define ENABLE_DEBUG_OUTPUT

using System.Diagnostics;


namespace Aoc2023Cs;

public class Day12
{
    public static bool partOne = true;

    public static void Run(int part)
    {
        partOne = (part == 1);
        string[] lines = "12".ReadLinesArray(test: false);

        UInt64 result = 0;
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

            char[] ca = conditions.ToCharArray();
            Context context = new Context(ca);
            Arrangement(ca, groups, ref context, 0);
            Console.WriteLine($" -> {context.numArrangements} arrangements found\n");
            result += context.numArrangements;
        }
        
        string partStr = partOne ? "One" : "Two";
        Console.WriteLine($"Part {partStr}: {result}\n");        
    }

    public ref struct Context
    {
        public UInt64 numArrangements = 0;
        public UInt64 count = 0;
        public int depth = 0;

        public Span<char> original;

        public Context(Span<char> original)
        {
            this.original = original;
        }
    }

    public static int Arrangement(Span<char> s, Span<int> groups, ref Context context, int currentHashGroup)
    {
        Debug.Assert(groups.Length != 0);

        // backtrack if there are more hashes in groups than our string is long
        // (not including required dots as separators between them)
        if ((s.Length + currentHashGroup) < groups.Sum())
        {
            return 0;
        }

        ++context.count;

        // for debug visualization
        // pre = pre.Replace('#', '=').Replace('.', ':');
        
        // continued block of #
        int hashGroup = currentHashGroup;

        while (groups.Length > 0)
        {
            int cursor = context.original.Length - s.Length;
            string indent = new (' '.Replicate(cursor));
            
#if ENABLE_DEBUG_OUTPUT            
            Console.WriteLine($"{pre}{new string (s)} - ({preGroups.MakeList()}),{groups.ToArray().MakeList()} [{context.depth}]");
#endif
            
            // Try to match groups
            int currentGroup = groups[0];

            int i;
            for (i = 0; i < s.Length; ++i)
            {
                char c = s[i];
                if (c == '.')
                {
                    // skip dots, except when they're ending a group
                    if (hashGroup > 0) break;
                } else if (c == '#')
                {
                    // continue a block of #
                    ++hashGroup;
                } else if (c == '?')
                {
                    ++context.depth;

                    Span<char> s2 = s[i..];
                    s2[0] = '.';
                    Arrangement(s2, groups, ref context, hashGroup);
                    s2[0] = '#';
                    Arrangement(s2, groups, ref context, hashGroup);
                    s2[0] = '?';
                    
                    --context.depth;
                    return 0;
                }
            }

            if (hashGroup != currentGroup)
            {
#if ENABLE_DEBUG_OUTPUT
                Console.WriteLine($"{indent} - {hashGroup} != {currentGroup}");
#endif                
                return 0;
            }

            groups = groups[1..];
            s = s[i..];

            if (groups.Length == 0)
            {
                bool success = !s.Contains('#');
                if (success)
                {
                    ++context.numArrangements;
                }
#if ENABLE_DEBUG_OUTPUT
                Console.WriteLine("{0}{1}", indent, success ? '+' : '-');
#endif                
                return 0;
            }

            hashGroup = 0;
            if (s.Length == 0)
            {
#if ENABLE_DEBUG_OUTPUT
                Console.WriteLine($"{indent} - string ended");
#endif                
                return 0;
            }
        }

#if ENABLE_DEBUG_OUTPUT
        Console.WriteLine($"- everything ended.");
#endif                
        return 0;
    }
}
