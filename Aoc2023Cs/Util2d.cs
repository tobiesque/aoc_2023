﻿using System.Numerics;

namespace Aoc2023Cs.Util2d;

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
    public static Vec2<T> up = new (T.Zero, -T.One);
    public static Vec2<T> down = new (T.Zero, T.One);
    public static Vec2<T> left = new (-T.One, T.Zero);
    public static Vec2<T> right = new (T.One, T.Zero);

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
