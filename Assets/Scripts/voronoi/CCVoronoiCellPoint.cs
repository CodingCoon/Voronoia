using System;
using UnityEngine;

public class CCVoronoiCellPoint : IComparable<CCVoronoiCellPoint>
{
    public Vector2 Point { get; private set; }
    public float Angle { get; private set; }

    public CCVoronoiCellPoint(Vector2 point, float angle)
    {
        Point = point;
        Angle = angle;
    }

    public int CompareTo(CCVoronoiCellPoint other)
    {
        if (Angle > other.Angle) return 1;
        if (Angle < other.Angle) return -1;
        return 0;
    }
}
