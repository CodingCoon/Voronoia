using UnityEngine;

public class CCTouchLine
{
    private const float EPSILON = 0.00001f;

    public CCTouchPoint Owner {  get; private set; }
    public Vector2 FirstPoint { get; private set; }
    public Vector2 SecondPoint { get; private set; }

    public CCTouchLine(CCTouchPoint owner, Vector2 firstPoint, Vector2 secondPoint)
    {
        Owner = owner;
        FirstPoint = firstPoint;
        SecondPoint = secondPoint;
    }

    public override string ToString()
    {
       return FirstPoint.x + " / " + FirstPoint.y + " x " + SecondPoint.x + " / " + SecondPoint.y + " (" + Owner + ")";
    }

    public bool IsOnLine(Vector2 point)
    {
        Vector2 secondToFirst = SecondPoint - FirstPoint;
        Vector2 pointToFirst = point - FirstPoint;
        float partInBetween = Vector2.Dot(pointToFirst, secondToFirst) /
                                Vector3.Dot(secondToFirst, secondToFirst);
        return 0 <= partInBetween && partInBetween <= 1;
    }

    //public Vector2? GetIntersection(Vector2 otherPoint, Vector2 otherDirection)
    //{
    //    Plane plane = new Plane (FirstPoint, SecondPoint);
    //Ray ray = new Ray(otherPoint, otherDirection);
    //float distance;

    //bool hit = plane.Raycast(ray, out distance);
    //if (hit)
    //{
    //    return ray.GetPoint(distance);
    //} 
    //else
    //{
    //    return null;
    //}
    //}

    public Vector2 CalcIntersection(CCPlane plane)
    {
        Vector2 lineDir = (FirstPoint - SecondPoint).normalized;
        CCRay ray = new CCRay(FirstPoint, lineDir);

        Vector2 intersectionPoint = plane.CalcIntersection(ray);
        return intersectionPoint;
    }

    public Vector2? CalcIntersection(CCTouchLine other)
    {
        // float tmp = (B2.x - B1.x) * (A2.y - A1.y) -
        //             (B2.y - B1.y) * (A2.x - A1.x);
        float tmp = (other.SecondPoint.x - other.FirstPoint.x) * (SecondPoint.y - FirstPoint.y) - 
                    (other.SecondPoint.y - other.FirstPoint.y) * (SecondPoint.x - FirstPoint.x);

        if (tmp == 0)
        {
            // No solution!
            return null;
        }

//        float mu = ((A1.x - B1.x) * (A2.y - A1.y) -
//                    (A1.y - B1.y) * (A2.x - A1.x)) / tmp;
        float mu = ((FirstPoint.x - other.FirstPoint.x) * (SecondPoint.y - FirstPoint.y) - 
                    (FirstPoint.y - other.FirstPoint.y) * (SecondPoint.x - FirstPoint.x)) / tmp;

        //return new Vector2(B1.x + (B2.x - B1.x) * mu,
        //                   B1.y + (B2.y - B1.y) * mu);
        return new Vector2 (other.FirstPoint.x + (other.SecondPoint.x - other.FirstPoint.x) * mu,
                            other.FirstPoint.y + (other.SecondPoint.y - other.FirstPoint.y) * mu);
    }

    public Vector2? CalcInnerIntersection(CCTouchLine other)
    {
        if (IsInnerIntersection(other)) 
            {
            float denominator = (other.SecondPoint.y - other.FirstPoint.y) * (SecondPoint.x - FirstPoint.x) - 
                                (other.SecondPoint.x - other.FirstPoint.x) * (SecondPoint.y - FirstPoint.y);

            float u_a = ((other.SecondPoint.x - other.FirstPoint.x) * (FirstPoint.y - other.FirstPoint.y) - 
                        (other.SecondPoint.y - other.FirstPoint.y) * (FirstPoint.x - other.FirstPoint.x)) / denominator;

            return FirstPoint + u_a * (SecondPoint - FirstPoint);
        }
        return null;
    }

    public bool IsInnerIntersection(CCTouchLine other, bool includeEndPoints = true)
    {
        //To avoid floating point precision issues we can use a small value
        float epsilon = EPSILON;

        bool isIntersecting = false;

        float denominator = (other.SecondPoint.y - other.FirstPoint.y) * (SecondPoint.x - FirstPoint.x) - 
                            (other.SecondPoint.x - other.FirstPoint.x) * (SecondPoint.y - FirstPoint.y);

        //Make sure the denominator is != 0 (or the lines are parallel)
        if (denominator > 0f + epsilon || denominator < 0f - epsilon)
        {
            float u_a = ((other.SecondPoint.x - other.FirstPoint.x) * (FirstPoint.y - other.FirstPoint.y) - 
                         (other.SecondPoint.y - other.FirstPoint.y) * (FirstPoint.x - other.FirstPoint.x)) / denominator;
            float u_b = ((SecondPoint.x - FirstPoint.x) * (FirstPoint.y - other.FirstPoint.y) - 
                         (SecondPoint.y - FirstPoint.y) * (FirstPoint.x - other.FirstPoint.x)) / denominator;

            //Are the line segments intersecting if the end points are the same
            if (includeEndPoints)
            {
                //The only difference between endpoints not included is the =, which will never happen so we have to subtract 0 by epsilon
                float zero = 0f - epsilon;
                float one = 1f + epsilon;

                //Are intersecting if u_a and u_b are between 0 and 1 or exactly 0 or 1
                if (u_a >= zero && u_a <= one && u_b >= zero && u_b <= one)
                {
                    isIntersecting = true;
                }
            }
            else
            {
                float zero = 0f + epsilon;
                float one = 1f - epsilon;

                //Are intersecting if u_a and u_b are between 0 and 1
                if (u_a > zero && u_a < one && u_b > zero && u_b < one)
                {
                    isIntersecting = true;
                }
            }
        }

        return isIntersecting;
    }

    //Vector2 GetIntersectionPoint(Vector2 FirstPoint, Vector2 SecondPoint, Vector2 p3, Vector2 p4)
    //{
    //    float d1 = Vector3.Cross(FirstPoint - p3, p4 - p3).z;
    //    float d2 = Vector3.Cross(SecondPoint - p3, p4 - p3).z;
    //    if (d1 - d2 == 0) return Vector2.negativeInfinity; // P1P2 and P3P4 is parallel in this case
    //    return (d1 * SecondPoint - d2 * FirstPoint) / (d1 - d2);
    //}
}
