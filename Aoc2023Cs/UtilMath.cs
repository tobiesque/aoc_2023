global using Vec2 = Aoc2023Cs.Util2d.Vec2<int>;
global using Box2 = Aoc2023Cs.Util2d.Box2<int>;
global using Line2 = Aoc2023Cs.Util2d.Line2<int>;
global using Vec2L = Aoc2023Cs.Util2d.Vec2<long>;
global using Box2L = Aoc2023Cs.Util2d.Vec2<long>;
global using Line2L = Aoc2023Cs.Util2d.Line2<long>;

using System.Numerics;
    
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

    public static void AddPrimeFactors(this ISet<ulong> primes, ulong number)
    {
        if ((number % 2) == 0)
        {
            primes.Add(2);
            while ((number % 2) == 0) number /= 2;
        }

        ulong limit = (ulong)Math.Sqrt(number);
        ulong div = 3UL;
        while (div <= limit)
        {
            if ((number % div) == 0)
            {
                primes.Add(div);
                while ((number % div) == 0) number /= div;
                limit = (ulong)Math.Sqrt(number);
            }
            div += 2UL;
        }
        if (number > 2)
        {
            primes.Add(number);
        }
    }

    public static IEnumerator<ulong> PrimeFactors(IEnumerable<ulong> e)
    {
        HashSet<ulong> primes = new();
        foreach (ulong number_ in e)
        {
            ulong number = number_;
            for (ulong div = 2; div <= (ulong)Math.Sqrt(number); div++)
            {
                while (number % div == 0)
                {
                    if (primes.Add(div)) yield return div;
                    number /= div;
                }
            }
            if (number > 2)
            {
                if (primes.Add(number)) yield return number;
            }
        }
    }
}
