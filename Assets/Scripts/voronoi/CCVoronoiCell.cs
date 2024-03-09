using System.Collections.Generic;
using UnityEngine;

public class CCVoronoiCell
{
    public IVoronoiCellOwner Owner { get; private set; }
    public List<Vector3> Points { get; private set; } = new List<Vector3>();

    public CCVoronoiCell(IVoronoiCellOwner owner, List<Vector2> points)
    {
        this.Owner = owner;
        Vector2? lastPoint = null;
        points.ForEach(point =>
        {
            if (lastPoint == null || !lastPoint.Equals(point))
            {
                lastPoint = point;
                Points.Add(point);
            }
        });
    }

    public CCVoronoiCell(IVoronoiCellOwner owner, List<CCVoronoiCellPoint> points)
    {
        this.Owner = owner;
        Vector2? lastPoint = null;
        points.ForEach(point =>
        {
            if (lastPoint == null || !lastPoint.Equals(point.Point))
            {
                lastPoint = point.Point;
                Points.Add(point.Point);
            }
        });
    }
}