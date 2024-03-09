using UnityEngine;

public class CCIntersection 
{
    public CCTouchLine FirstLine { get; private set; }
    public CCTouchLine SecondLine { get; private set; }
    public Vector2 Point { get; private set; }

    public CCIntersection(CCTouchLine firstLine, CCTouchLine secondLine, Vector2 intersection)
    {
        FirstLine = firstLine;
        SecondLine = secondLine;
        Point = intersection;
    }

    public bool BelongsTo(IVoronoiCellOwner owner)
    {
        return  (FirstLine.Owner != null && (FirstLine.Owner.First == owner
                                         || FirstLine.Owner.Second == owner))
            || (SecondLine.Owner != null && (SecondLine.Owner.First == owner 
                                         || SecondLine.Owner.Second == owner));

    }

}
