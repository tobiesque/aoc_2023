using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

namespace Aoc2023Cs;

public static class Util
{
    public static TEnum ToEnum<TEnum>(this string s) where TEnum : Enum
    {
        return Enum.TryParse(typeof(TEnum), s, out var result) ? (TEnum)result : default!;
    }

    public static string Textify(this object o, [CallerArgumentExpression("o")] string objectName = "")
    {
        StringBuilder sb = new();
        sb.Append($"{objectName} = ");
        sb.Append(JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings() { TypeNameHandling  = TypeNameHandling.All }));
        return sb.ToString();
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

    public static ref Span<char> SkipNonInt(this ref Span<char> s) => ref s.ExtractRef(out Span<char> _, c => !IsIntDigit(c));
    public static ref Span<char> SkipWhiteRef(this ref Span<char> s) => ref s.ExtractRef(out Span<char> _, char.IsWhiteSpace);
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
    
    public static ref Span<char> ExtractRef(this ref Span<char> s, out string result, Func<char, bool> predicate)
    {
        ref Span<char> s2 = ref s.ExtractRef(out Span<char> spanResult, predicate);
        result = new(spanResult);
        return ref s2;
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
    public static ref Span<char> ExtractStringRef(this ref Span<char> s, out string result) => ref ExtractRef(ref s, out result, IsNotWhitespace);
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
        s = Extract(s, out Span<char> strResult, IsIntDigit);
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

    public static string Replicate(this string value, int num) => value.AsSpan().Replicate(num);

    public static string Replicate(this Span<char> value, int num)
    {
        StringBuilder sb = new();
        for (int i = 0; i < num; ++i)
        {
            sb.Append(value);
        }
        return sb.ToString();
    }
    
    public static IEnumerable<T> Repeat<T>(this IEnumerable<T> e, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            foreach (var v in e)
            {
                yield return v;
            }
        }
    }

    public static T Sum<T>(this ref Span<T> s) where T : IBinaryInteger<T>
    {
        T sum = T.Zero;
        foreach (T e in s)
        {
            sum += e;
        }
        return sum;
    }
    
    public static string Append(this string s, char c) => s + c;

    public static bool Between<T>(this T value, T min, T size) where T : INumber<T> => (value >= min) && (value >= (min+size));

    public static string MakeList<T>(this IEnumerable<T> source, string separator = ",")
    {
        return string.Join(separator, source.Select(e => e!.ToString()));
    }

    public static Dictionary<U, T> Inverse<T, U>(this Dictionary<T, U> dict) where T : notnull where U : notnull
    {
        return dict.ToDictionary((t) => t.Value, (u) => u.Key);
    }

    public static Span<char> AsSpan(this string s) => CollectionsMarshal.AsSpan(s.ToList());

    
    public static List<T> SortSelf<T>(this List<T> list)
    {
        list.Sort();
        return list;
    }

    public static IEnumerable<string[]> Split(this string[] ss, Func<string, bool> splitFunc)
    {
        List<string> current = new();
        foreach (var s in ss)
        {
            if (splitFunc(s))
            {
                yield return current.ToArray();
                current.Clear();
            }
            else
            {
                current.Add(s);
            }
        }

        if (current.Count > 0)
        {
            yield return current.ToArray();
        }
    }

    public static void Swap<T>(this ref T a, ref T b) where T : struct
    {
        (a, b) = (b, a);
    }

    public delegate void Iterate2DAction<T>(Vec2 pos, ref T t);
    public static void Iterate2D<T>(this T[,] t, Iterate2DAction<T> action, Action<Vec2>? lineAction = null) where T : struct
    {
        for (var y = 0; y < t.GetLength(1); y++)
        {
            for (var x = 0; x < t.GetLength(0); x++)
            {
                action(new Vec2(x, y), ref t[x, y]);
            }
            lineAction?.Invoke(new Vec2(0, y));
        }
    }
    
    public static List<string> ReadLinesList(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToList();
    public static string[] ReadLinesArray(this string day, bool test = false) => ReadLinesEnumerable(day, test).ToArray();
    public static Span<string> ReadLinesSpan(this string day, bool test = false) => ReadLinesArray(day, test);

    public static IEnumerable<string> ReadLinesEnumerable(this string day, bool test = false)
    {
        string inputFile = $"Input/{day}{(test ? ".tst" : ".txt")}";
        return File.ReadLines(inputFile);
    }
}
