using System.Numerics;

namespace Aoc2023Cs.Util2d;

public static class NumbersUtil<TValue> {
    public static TValue GetMaxValue => ((TValue)typeof(TValue).GetField("MaxValue")!.GetValue(null)!)!;
    public static TValue GetMinValue => ((TValue)typeof(TValue).GetField("MinValue")!.GetValue(null)!)!;
}

public struct Vec2<T> : IEqualityComparer<Vec2<T>> where T : IBinaryInteger<T>
{
    public Vec2(T x, T y)
    {
        this.x = x;
        this.y = y;
    }

    public T x = -T.One;
    public T y = -T.One;

    public static Vec2<T> zero = new (T.Zero, T.Zero);
    public static Vec2<T> MaxValue = new (NumbersUtil<T>.GetMaxValue, NumbersUtil<T>.GetMaxValue);
    public static Vec2<T> MinValue = new (NumbersUtil<T>.GetMinValue, NumbersUtil<T>.GetMinValue);
    
    public static Vec2<T> up = new (T.Zero, -T.One);
    public static Vec2<T> down = new (T.Zero, T.One);
    public static Vec2<T> left = new (-T.One, T.Zero);
    public static Vec2<T> right = new (T.One, T.Zero);
    public static Vec2<T>[] directions = [Vec2<T>.right, Vec2<T>.down, Vec2<T>.left, Vec2<T>.up];

    public char DirectionChar
    {
        get
        {
            if (y == T.Zero)
            {
                if (x > T.Zero) return '>';
                if (x == T.Zero) return 'o';
                return '<';
            }

            if (x == T.Zero)
            {
                if (y > T.Zero) return 'V';
                if (y == T.Zero) return 'o';
                return '^';
            }
            return ' ';
        }
    }

    public bool InBounds(Vec2<T> dimension)
    {
        if ((x < T.Zero) || (x >= dimension.x)) return false;
        if ((y < T.Zero) || (y >= dimension.y)) return false;
        return true;
    }

    public static Vec2<T> invalid = new (-T.One, -T.One);

    public static Vec2<T> operator *(Vec2<T> v, T n) => new(v.x * n, v.y * n);
    public static Vec2<T> Max(Vec2<T> a, Vec2<T> b) => new(T.Max(a.x, b.x), T.Max(a.y, b.y));
    public static Vec2<T> Min(Vec2<T> a, Vec2<T> b) => new(T.Min(a.x, b.x), T.Min(a.y, b.y));
    public Vec2<T> Max(Vec2<T> other) => new(T.Max(x, other.x), T.Max(y, other.y));
    public Vec2<T> Min(Vec2<T> other) => new(T.Min(x, other.x), T.Min(y, other.y));
    public Vec2<T> MaxSelf(Vec2<T> other) => this = Max(other);
    public Vec2<T> MinSelf(Vec2<T> other) => this = Min(other);
    
    public static Vec2<T> operator +(Vec2<T> a) => new(a.x, a.y);
    public static Vec2<T> operator -(Vec2<T> a) => new(-a.x, -a.y);
    public static Vec2<T> operator +(Vec2<T> a, Vec2<T> b) => new(a.x + b.x, a.y + b.y);
    public static Vec2<T> operator -(Vec2<T> a, Vec2<T> b) => new(a.x - b.x, a.y - b.y);
    public static bool operator ==(Vec2<T> a, Vec2<T> b) => (a.x == b.x) && (a.y == b.y);
    public static bool operator !=(Vec2<T> a, Vec2<T> b) => (a.x != b.x) || (a.y != b.y);
    public T Distance(Vec2<T> b) => Abs(x-b.x) + Abs(y-b.y);

    public static T Abs(T v) => (T.IsNegative(v)) ? -v : v;

    public override int GetHashCode() => HashCode.Combine(x, y);
    public int GetHashCode(Vec2<T> other) => HashCode.Combine(other.x, other.y);

    public bool Equals(Vec2<T> a, Vec2<T> b) => (a.x == b.x) && (a.y == b.y);
    public bool Equals(Vec2<T> b) => (x == b.x) && (y == b.y);
    public override bool Equals(object? b) => (b is Vec2<T> other) && Equals(other);

    public bool IsHorizontal => (x != T.Zero) && (y == T.Zero);
    public bool IsVertical => (x == T.Zero) && (y != T.Zero);

    public override string ToString() => $"({x}, {y})";
}

public struct Line2<T> : IEqualityComparer<Line2<T>> where T : IBinaryInteger<T>
{
    public Vec2<T> from { get; set; }
    public Vec2<T> to { get; set; }

    public Line2(Vec2<T> from, Vec2<T> to)
    {
        this.from = from;
        this.to = to;
    }

    public bool IsVertical() => from.x == to.x;
    public bool IsHorizontal() => from.y == to.y;
    public bool IsAxisParallel() => IsVertical() || IsHorizontal();

    public int GetHashCode(Line2<T> obj) => HashCode.Combine(obj.from, obj.to);
    public override int GetHashCode() => HashCode.Combine(from, to);

    public bool Equals(Line2<T> a, Line2<T> v) => a.from.Equals(a.from, v.from) && a.to.Equals(a.to, v.to);
    public bool Equals(Line2<T> b) => Equals(this, b);
    public override bool Equals(object? b) => (b is Line2<T> other) && Equals(other);

    public override string ToString() => IsAxisParallel() ? $"({from.x}-{to.x}, {from.y})" : $"{from}->{to}";
}    
