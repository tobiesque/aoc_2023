using System.Text.RegularExpressions;

namespace Aoc2023Cs;

public static class Util
{
    public static TEnum ToEnum<TEnum>(this string s) where TEnum : Enum
    {
        return Enum.TryParse(typeof(TEnum), s, out var result) ? (TEnum)result : default!;
    }

    public static string After(this string s, char c)
    {
        int pos = s.IndexOf(c);
        return (pos != -1) ? s.Substring(pos + 1) : s;
    }
    
    public static string Until(this string s, params char[] chars)
    {
        List<int> positions = new();
        foreach (char c in chars)
        {
            int pos = s.IndexOf(c);
            if (pos != -1)
            {
                positions.Add(pos);
            }
        }

        if (positions.Count == 0) return s;
        return s.Substring(0, positions.Min());
    }

    public static string ExtractInt(string s, out int result)
    {
        Match match = Regex.Match(s, @"\b\d*\b", RegexOptions.CultureInvariant);
        if (!match.Success || (match.Length == 0))
        {
            result = int.MinValue;
            return s;
        }
        
        result = int.Parse(s.Substring(0, match.Length));
        if (match.Length >= s.Length) return "";
        return s.Substring(match.Length);
    }

    public static string SkipSpaces(string s)
    {
        int spaces = 0;
        while ((s.Length > spaces) && (s[spaces] == ' '))
        {
            ++spaces;
        }
        return s.Substring(spaces);
    }

    public static string MakeList<T>(IEnumerable<T> source) where T : IFormattable
    {
        return string.Join(", ", source.Select(c => c.ToString()));
    }

    public static IEnumerable<string> ReadLines(string day, bool test = false)
    {
        string inputFile = $"{day}{(test ? ".tst" : ".txt")}";
        return File.ReadLines(inputFile);
    }
    
    public class MultiMap<TKey, TValue> : Dictionary<TKey, HashSet<TValue>> where TValue : class
    {
        public TValue MultiAdd(TKey key, TValue value)
        {
            if (!TryGetValue(key, out var set))
            {
                set = new();
                Add(key, set);
            }

            if (set.TryGetValue(value, out var originalValue))
            {
                return originalValue!;
            }
            return value!;
        }
        
        public void MultiRemove(TKey key, TValue value)
        {
            if (TryGetValue(key, out var set))
            {
                set.Remove(value);
                if (set.Count == 0)
                {
                    Remove(key); 
                }
            }
        }
    }

}

public struct Vec2 : IEqualityComparer<Vec2>
{
    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public int x { get; set; }
    public int y { get; set; }

    public static Vec2 operator +(Vec2 a) => new(a.x, a.y);
    public static Vec2 operator -(Vec2 a) => new(-a.x, -a.y);
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.x + b.x, a.y + b.y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.x - b.x, a.y - b.y);

    public int GetHashCode(Vec2 other) => HashCode.Combine(other.x, other.y);
    
    public bool Equals(Vec2 a, Vec2 b) => (a.x == b.x) && (a.y == b.y);
    public bool Equals(Vec2 b) => (x == b.x) && (y == b.y);
    public override bool Equals(object b) => (b is Vec2 other) && Equals(other);

    public override string ToString() => $"({x}, {y})";
}

public struct Line2 : IEqualityComparer<Line2>
{
    public Vec2 from { get; set; }
    public Vec2 to { get; set; }

    public Line2(Vec2 from, Vec2 to)
    {
        this.from = from;
        this.to = to;
    }

    public bool IsVertical() => from.x == to.x;
    public bool IsHorizontal() => from.y == to.y;
    public bool IsAxisParallel() => IsVertical() || IsHorizontal();
    
    public int GetHashCode(Line2 obj) => HashCode.Combine(obj.from, obj.to);
    public override int GetHashCode() => HashCode.Combine(from, to);

    public bool Equals(Line2 a, Line2 v) => a.from.Equals(a.from, v.from) && a.to.Equals(a.to, v.to);
    public bool Equals(Line2 b) => Equals(this, b);
    public override bool Equals(object b) => (b is Line2 other) && Equals(other);
    
    public override string ToString() => IsAxisParallel() ? $"({from.x}-{to.x}, {from.y})" : $"{from}->{to}";
}