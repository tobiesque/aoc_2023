namespace Aoc2023Cs;

public static class g3Util
{
    public static g3.Vector2d Max(this g3.Vector2d a, g3.Vector2d b) => new(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    public static g3.Vector2d Min(this g3.Vector2d a, g3.Vector2d b) => new(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
    public static g3.Vector3d Max(this g3.Vector3d a, g3.Vector3d b) => new(Math.Max(a.x, b.x), Math.Max(a.y, b.y), Math.Max(a.z, b.z));
    public static g3.Vector3d Min(this g3.Vector3d a, g3.Vector3d b) => new(Math.Min(a.x, b.x), Math.Min(a.y, b.y), Math.Min(a.z, b.z));
    public static g3.Vector2d ToVec2(this g3.Vector3d v) => new(v.x, v.y);

    public static string ToStr(this g3.Line2d l) => $"[({l.Origin})/({l.Direction})]";
    public static string ToStr(this g3.AxisAlignedBox2d b) => $"[({b.Min}), ({b.Max})]";
}