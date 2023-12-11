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

    public static ref Span<char> SkipNonInt(this ref Span<char> s) => ref s.ExtractRef(out var _, c => !IsIntDigit(c));
    public static ref Span<char> SkipWhiteRef(this ref Span<char> s) => ref s.ExtractRef(out var _, char.IsWhiteSpace);
    public static Span<char> SkipWhite(this Span<char> s) => s.Extract(out var _, char.IsWhiteSpace);

    public static ref Span<char> ExtractRef(this ref Span<char> s, out Span<char> result, Func<char, bool> predicate)
    {
        int i = 0;
        while ((s.Length > i) && !predicate(s[i])) ++i;
        int j = i;
        while ((s.Length > j) && predicate(s[j])) ++j;
        result = s[i..j];
        return ref s.SkipRef(j);
    }
    
    public static Span<char> Extract(this Span<char> s, out Span<char> result, Func<char, bool> predicate)
    {
        int i = 0;
        while ((s.Length > i) && !predicate(s[i])) ++i;
        int j = i;
        while ((s.Length > j) && predicate(s[j])) ++j;
        result = s[i..j];
        return s.Skip(j);
    }
    
    public static ref Span<char> ExtractStringRef(this ref Span<char> s, out Span<char> result) => ref ExtractRef(ref s, out result, IsNotWhitespace);
    public static Span<char> ExtractString(this Span<char> s, out Span<char> result) => Extract(s, out result, IsNotWhitespace);

    public static bool IsNotWhitespace(this char c) => !char.IsWhiteSpace(c);
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

    public static Span<char> ExtractLetters(this Span<char> s, out Span<char> result) => Extract(s, out result, char.IsAsciiLetterOrDigit);
    public static ref Span<char> ExtractLettersRef(this ref Span<char> s, out Span<char> result) => ref ExtractRef(ref s, out result, char.IsAsciiLetterOrDigit);

    public static string Replicate(this char value, int num) => new (Enumerable.Repeat(value, num).ToArray());

    public static string Append(this string s, char c) => s + c;

    public static bool Between<T>(this T value, T min, T size) where T : INumber<T> => (value >= min) && (value >= (min+size));

    public static string MakeList<T>(this IEnumerable<T> source)
    {
        return string.Join(", ", source.Select(e => e!.ToString()));
    }

    public static string MakeList<T>(this Span<T> source)
    {
        return string.Join(", ", source.ToArray().Select(e => e.ToString()));
    }
    
    public static Dictionary<U, T> Inverse<T, U>(this Dictionary<T, U> dict) where T : notnull where U : notnull
    {
        return dict.ToDictionary((t) => t.Value, (u) => u.Key);
    }

    public static IEnumerable<T> AsEnumerable<T>(this Span<T> span) => span.ToArray();
    public static Span<char> AsSpan(this string s) => CollectionsMarshal.AsSpan(s.ToList());

    public static List<T> SortSelf<T>(this List<T> list, Comparison<T> comparison)
    {
        list.Sort(comparison);
        return list;
    }
    
    public static List<T> SortSelf<T>(this List<T> list)
    {
        list.Sort();
        return list;
    }
    
    public static List<string> ReadLinesList(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToList();
    public static string[] ReadLinesArray(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToArray();
    public static Span<string> ReadLinesSpan(this string day, bool test = false) => ReadLinesArray(day, test);

    public static IEnumerable<string> ReadLinesEnumerable(this string day, bool test = false)
    {
        string inputFile = $"{day}{(test ? ".tst" : ".txt")}";
        return File.ReadLines(inputFile);
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
