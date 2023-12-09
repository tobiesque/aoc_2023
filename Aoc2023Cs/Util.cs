using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace Aoc2023Cs;

public static class Util
{
    public static TEnum ToEnum<TEnum>(this string s) where TEnum : Enum
    {
        return Enum.TryParse(typeof(TEnum), s, out var result) ? (TEnum)result : default!;
    }

    public static ref Span<char> After(this ref Span<char> s, char c)
    {
        s = s[(s.IndexOf(c) + 1)..];
        return ref s;
    }

    public static ref Span<char> Until(this ref Span<char> s, params char[] cs)
    {
        int newPos = int.MaxValue;
        foreach (char c in cs)
        {
            newPos = Math.Min(newPos, s.IndexOf(c));
        }
        s = s[..(newPos + 1)];
        return ref s;
    }

    public static ref Span<char> ExtractWhiteInt(this ref Span<char> s, out int result) => ref s.SkipWhiteRef().ExtractIntRef(out result).SkipWhiteRef();

    public static ref Span<string> SkipEmptyLines(this ref Span<string> l)
    {
        int i = 0;
        while ((l.Length > 0) && (string.IsNullOrWhiteSpace(l[0]))) ++i;
        l = l[i..];
        return ref l;
    }  
    
    public static ref Span<char> SkipRef(this ref Span<char> s, int count)
    {
        s = s[count..];
        return ref s;
    }

    public static Span<char> Skip(this Span<char> s, int count) => s[count..];

    public static ref Span<char> SkipWhiteRef(this ref Span<char> s) => ref s.ExtractRef(out var _, char.IsWhiteSpace);
    public static Span<char> SkipWhite(this Span<char> s) => s.Extract(out var _, char.IsWhiteSpace);

    public static ref Span<char> ExtractRef(this ref Span<char> s, out Span<char> result, Func<char, bool> predicate)
    {
        int i = 0;
        while ((s.Length > i) && predicate(s[i])) ++i;
        result = s[..i];
        return ref s.SkipRef(i);
    }
    
    public static Span<char> Extract(this Span<char> s, out Span<char> result, Func<char, bool> predicate)
    {
        int i = 0;
        while ((s.Length > i) && predicate(s[i])) ++i;
        result = s[..i];
        return s.Skip(i);
    }
    
    public static ref Span<char> ExtractStringRef(this ref Span<char> s, out Span<char> result) => ref ExtractRef(ref s, out result, char.IsWhiteSpace);
    public static Span<char> ExtractString(this Span<char> s, out Span<char> result) => Extract(s, out result, char.IsWhiteSpace);

    public static bool IsIntDigit(this char c) => char.IsDigit(c) || (c == '-');
    
    public static ref Span<char> ExtractIntRef<T>(this ref Span<char> s, out T result) where T : IBinaryInteger<T>
    {
        ExtractRef(ref s, out Span<char> strResult, IsIntDigit);
        result = strResult.ReadIntWhole<T>();
        return ref s;
    }

    public static Span<char> ExtractInt<T>(this Span<char> s, out T result) where T : IBinaryInteger<T>
    {
        Extract(s, out Span<char> strResult, IsIntDigit);
        result = strResult.ReadIntWhole<T>();
        return s;
    }
    
    public static T ReadInt<T>(this Span<char> s) where T : IBinaryInteger<T>
    {
        Extract(s, out Span<char> strResult, IsIntDigit);
        return strResult.ReadIntWhole<T>();
    }
    
    public static T ReadIntWhole<T>(this Span<char> s) where T : IBinaryInteger<T>
    {
        return T.Parse(s, CultureInfo.InvariantCulture);
    }
    
    public static bool Between(int value, int min, int size) => (value >= min) && (value >= (min+size));

    public static string MakeList<T>(this IEnumerable<T> source) where T : IFormattable
    {
        return string.Join(", ", source.Select(c => c.ToString()));
    }

    public static Dictionary<U, T> Inverse<T, U>(this Dictionary<T, U> dict) where T : notnull where U : notnull
    {
        return dict.ToDictionary((t) => t.Value, (u) => u.Key);
    }

    public static IEnumerable<T> AsEnumerable<T>(this Span<T> span) => span.ToArray();
    public static Span<char> AsSpan(this string s) => CollectionsMarshal.AsSpan(s.ToList());
    
    public static List<string> ReadLinesList(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToList();
    public static string[] ReadLinesArray(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToArray();
    public static Span<string> ReadLinesSpan(this string day, bool test = false) => ReadLinesArray(day, test);

    public static IEnumerable<string> ReadLinesEnumerable(this string day, bool test = false)
    {
        string inputFile = $"{day}{(test ? ".tst" : ".txt")}";
        return File.ReadLines(inputFile);
    }

    public static int GeometricSequence(int n, int factor)
    {
        if (n == 0) return 0;
        
        int value = 1;
        for (int i = 1; i < n; ++i)
        {
            value *= factor;
        }
        return value;
    }
    
    public static string ToStrung<T>(this T o)
    {
        return JsonConvert.SerializeObject(o, Formatting.Indented);
    }
    
    public class MultiMap<TKey, TValue> : Dictionary<TKey, HashSet<TValue>> where TValue : class where TKey : notnull
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

    public override int GetHashCode() => HashCode.Combine(x, y);
    public int GetHashCode(Vec2 other) => HashCode.Combine(other.x, other.y);
    
    public bool Equals(Vec2 a, Vec2 b) => (a.x == b.x) && (a.y == b.y);
    public bool Equals(Vec2 b) => (x == b.x) && (y == b.y);
    public override bool Equals(object? b) => (b is Vec2 other) && Equals(other);

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
    public override bool Equals(object? b) => (b is Line2 other) && Equals(other);
    
    public override string ToString() => IsAxisParallel() ? $"({from.x}-{to.x}, {from.y})" : $"{from}->{to}";
}