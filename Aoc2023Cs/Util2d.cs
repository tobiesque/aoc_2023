namespace Aoc2023Cs.Util2d;

public struct Vec2 : IEqualityComparer<Vec2>
{
    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x = -1;
    public int y = -1;

    public static Vec2 invalid = new Vec2(-1, -1);

    public static Vec2 operator +(Vec2 a) => new(a.x, a.y);
    public static Vec2 operator -(Vec2 a) => new(-a.x, -a.y);
    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.x + b.x, a.y + b.y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.x - b.x, a.y - b.y);
    public static bool operator ==(Vec2 a, Vec2 b) => (a.x == b.x) && (a.y == b.y);
    public static bool operator !=(Vec2 a, Vec2 b) => (a.x != b.x) || (a.y != b.y);

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
