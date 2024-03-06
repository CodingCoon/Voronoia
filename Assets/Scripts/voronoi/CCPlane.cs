using UnityEngine;

public class CCPlane
{
    public readonly Vector2 pos;
    public readonly Vector2 normal;

    public CCPlane(Vector2 pos, Vector2 normal)
    {
        this.pos = pos;
        this.normal = normal;
    }

    public override string ToString()
    {
        return pos + " -> " + normal;
    }

    internal Vector2 CalcIntersection(CCRay ray)
    {
        float denominator = Vector2.Dot(-normal, ray.direction);
        Vector2 vecBetween = pos - ray.pos;

        float t = Vector2.Dot(vecBetween, -normal) / denominator;
        Vector2 intersectionPoint = ray.pos + ray.direction * t;
        return intersectionPoint;
    }
}