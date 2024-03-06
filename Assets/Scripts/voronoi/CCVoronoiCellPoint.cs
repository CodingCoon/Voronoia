using System;

public class CCVoronoiCellPoint : IComparable<CCVoronoiCellPoint>
{
    public CCIntersection Intersection { get; private set; }
    public float Angle { get; private set; }

    public CCVoronoiCellPoint(CCIntersection intersection, float angle)
    {
        Intersection = intersection;
        Angle = angle;
    }

    public int CompareTo(CCVoronoiCellPoint other)
    {
        if (Angle > other.Angle) return 1;
        if (Angle < other.Angle) return -1;
        return 0;
    }
}
