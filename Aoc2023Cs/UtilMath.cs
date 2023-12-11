using System.Diagnostics;
using System.Numerics;
using Aoc2023Cs;

namespace Aoc2023Cs;

public static class UtilMath
{
    public static IEnumerable<T> Range<T>(T start, T end) where T : IBinaryNumber<T> => Range(start, end, T.One);

    public static IEnumerable<T> Range<T>(T start, T end, T step) where T : IBinaryNumber<T>
    {
        for (T i = start; i < end; i += step) yield return i;
    }

    public static T Product<T>(this IEnumerable<T> source) where T : IBinaryNumber<T>
    {
        T result = T.One;
        foreach (T value in source) result *= value;
        return result;
    }

    public static T Pow<T>(this T value, int power) where T : IBinaryNumber<T>
    {
        T result = T.One;
        for (int i = 0; i < power; ++i)
        {
            result *= value;
        }

        return result;
    }

    public static T GeometricSequence<T>(this T ratio, int times) where T : IBinaryNumber<T> =>
        GeometricSequence(ratio, times, T.One);

    public static T GeometricSequence<T>(this T ratio, int times, T first) where T : IBinaryNumber<T>
    {
        if (!T.IsPositive(ratio) || !T.IsPositive(first) || (times <= 0)) return T.Zero;
        return first * Pow(ratio, times - 1);
    }

    public static T GreatestCommonDivisor<T>(T a, T b) where T : INumber<T>
    {
        while (a != T.Zero)
        {
            T a_ = a;
            a = b % a;
            b = a_;
        }
        return b;
    }

    public static T GreatestCommonDivisor<T>(this IEnumerable<T> e) where T : INumber<T> => e.Aggregate(GreatestCommonDivisor);
    public static T LeastCommonDenominator<T>(T a, T b) where T : INumber<T> => (a / GreatestCommonDivisor(a, b)) * b;
    public static T LeastCommonDenominator<T>(this IEnumerable<T> e) where T : INumber<T>  => e.Aggregate(LeastCommonDenominator);
}

public interface INumberBase
{
    static abstract int Base { get ; }
    static abstract char[] Digits { get; }
    
    static virtual long LongFromDigit(char c) => DefaultLongFromDigit(c);
    
    private static long DefaultLongFromDigit(char c)
    {
        if (c.Between('0', '9')) return (c - '0'); 
        if (c.Between('A', 'Z')) return (c - 'A' + 10);
        Debug.Fail($"Unknown character: '{c}'({(uint)c})");
        return 0;
    }
}

public struct Base10 : INumberBase
{
    public static int Base => 10;
    public static char[] Digits => new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
}

public struct Base12 : INumberBase
{
    public static int Base => 12;
    public static char[] Digits => new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B' };
}

public readonly struct Number<T> where T : INumberBase, new () 
{
    public Number(Number<T> other) { value = other.value; }

    public Number(string s)
    {
        foreach (char c in s.Reverse())
        {
            value += T.LongFromDigit(c);
            value *= T.Base;
        }
    }

    public static Number<T> operator +(Number<T> a, Number<T> b) => new (a.value + b.value); 
    public static Number<T> operator -(Number<T> a, Number<T> b) => new (a.value - b.value); 
    public static Number<T> operator *(Number<T> a, Number<T> b) => new (a.value * b.value); 
    public static Number<T> operator /(Number<T> a, Number<T> b) => new (a.value / b.value);
    
    private Number(long other) { value = other; }
    private readonly long value = 0;
} 
