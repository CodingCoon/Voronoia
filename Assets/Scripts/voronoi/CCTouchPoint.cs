using UnityEngine;

public class CCTouchPoint 
{
    public IVoronoiCellOwner First { get; private set; }
    public IVoronoiCellOwner Second { get; private set; }
    public Vector2 Center { get; private set; }

    public CCPlane Plane { get; private set; }

    public CCTouchPoint(IVoronoiCellOwner first, IVoronoiCellOwner second)
    {
        this.First = first;
        this.Second = second;

        this.Center = (first.GetPosition() * second.Power + second.GetPosition() * first.Power) / 
                      (first.Power + second.Power);
        this.Plane = new CCPlane(Center, (Center - first.GetPosition()).normalized);
    }

    public override string ToString()
    {
        return "TP: " + Plane;
    }
}
