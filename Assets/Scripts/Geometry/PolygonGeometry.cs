using System;
using System.Collections.Generic;
using UnityEngine;

public static class PolygonGeometry
{
    public const float Epsilon = 0.0001f;

    public static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    public static bool IsFinite(Vector2 point) => IsFinite(point.x) && IsFinite(point.y);
    public static bool IsFinite(Vector3 point) => IsFinite(point.x) && IsFinite(point.y) && IsFinite(point.z);

    public static float Area(IReadOnlyList<Vector3> points)
    {
        if (points == null || points.Count < 3) return 0;
        // A common origin avoids cancellation for translated polygons.
        Vector3 origin = points[0];
        double sum = 0;
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 a = points[i] - origin;
            Vector3 b = points[(i + 1) % points.Count] - origin;
            if (!IsFinite(a) || !IsFinite(b))
                throw new ArgumentException("Polygon contains non-finite coordinates.");
            sum += (double)a.x * b.y - (double)b.x * a.y;
        }
        float area = (float)(Math.Abs(sum) * 0.5);
        if (!IsFinite(area)) throw new ArgumentException("Polygon area is not finite.");
        return area;
    }

    // Empty is a valid presentation state, not a silently accepted computed cell.
    public static Vector3[] Validate(IReadOnlyList<Vector3> points)
    {
        if (points == null || points.Count == 0) return Array.Empty<Vector3>();
        if (points.Count < 3) throw new ArgumentException("Polygon needs at least three vertices.");
        var copy = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            if (!IsFinite(points[i])) throw new ArgumentException("Polygon contains non-finite coordinates.");
            copy[i] = points[i];
            for (int j = 0; j < i; j++)
                if ((points[i] - points[j]).sqrMagnitude < 0.0001f)
                    throw new ArgumentException("Polygon vertices are too close for SpriteShape.");
        }
        if (Area(copy) <= Epsilon) throw new ArgumentException("Polygon has no usable area.");
        for (int i = 0; i < copy.Length; i++)
        {
            int nextI = (i + 1) % copy.Length;
            for (int j = i + 1; j < copy.Length; j++)
            {
                int nextJ = (j + 1) % copy.Length;
                if (nextI == j || nextJ == i) continue;
                if (SegmentsIntersect(copy[i], copy[nextI], copy[j], copy[nextJ]))
                    throw new ArgumentException("Polygon intersects itself.");
            }
        }
        return copy;
    }

    private static bool SegmentsIntersect(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
    {
        double abC = Cross(b - a, c - a), abD = Cross(b - a, d - a);
        double cdA = Cross(d - c, a - c), cdB = Cross(d - c, b - c);
        if (abC * abD < 0 && cdA * cdB < 0) return true;
        return (Math.Abs(abC) < Epsilon && OnSegment(a, b, c)) ||
               (Math.Abs(abD) < Epsilon && OnSegment(a, b, d)) ||
               (Math.Abs(cdA) < Epsilon && OnSegment(c, d, a)) ||
               (Math.Abs(cdB) < Epsilon && OnSegment(c, d, b));
    }

    private static double Cross(Vector2 a, Vector2 b) => (double)a.x * b.y - (double)a.y * b.x;

    public static Vector2 ClosestPoint(IReadOnlyList<Vector3> points, Vector2 position)
    {
        if (points == null || points.Count < 3 || !IsFinite(position))
            throw new ArgumentException("Target selection requires a valid polygon and position.");
        bool inside = false;
        float nearestDistance = float.PositiveInfinity;
        Vector2 nearest = position;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 a = points[i], b = points[(i + 1) % points.Count];
            Vector2 edge = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(position - a, edge) / edge.sqrMagnitude);
            Vector2 candidate = a + edge * t;
            float distance = (candidate - position).sqrMagnitude;
            if (distance < nearestDistance) { nearestDistance = distance; nearest = candidate; }
            if ((a.y > position.y) != (b.y > position.y) &&
                position.x < (b.x - a.x) * (position.y - a.y) / (b.y - a.y) + a.x)
                inside = !inside;
        }
        return inside ? position : nearest;
    }

    public static Vector2 ClosestPointInside(IReadOnlyList<Vector3> points, Vector2 position,
        Vector2 interiorReference, float boundaryMargin)
    {
        if (boundaryMargin <= 0) return ClosestPoint(points, position);
        Vector2 target = ClosestPoint(points, position);
        Vector2 reference = ClosestPoint(points, interiorReference);
        Vector2 centroid = Vector2.zero;
        for (int i = 0; i < points.Count; i++) centroid += (Vector2)points[i];
        centroid /= points.Count;
        centroid = ClosestPoint(points, centroid);
        if (DistanceToBoundary(points, centroid) > DistanceToBoundary(points, reference)) reference = centroid;
        if (DistanceToBoundary(points, reference) < boundaryMargin)
            throw new InvalidOperationException("Area is too narrow for the required target margin.");
        if (DistanceToBoundary(points, target) >= boundaryMargin) return target;
        Vector2 valid = reference;
        Vector2 invalid = target;
        for (int i = 0; i < 24; i++)
        {
            Vector2 middle = (valid + invalid) * 0.5f;
            if (DistanceToBoundary(points, middle) >= boundaryMargin) valid = middle;
            else invalid = middle;
        }
        return valid;
    }

    public static float DistanceToBoundary(IReadOnlyList<Vector3> points, Vector2 position)
    {
        if (points == null || points.Count < 3 || !IsFinite(position)) return 0;
        float nearestDistance = float.PositiveInfinity;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 a = points[i], b = points[(i + 1) % points.Count];
            Vector2 edge = b - a;
            float t = edge.sqrMagnitude <= Epsilon * Epsilon ? 0 :
                Mathf.Clamp01(Vector2.Dot(position - a, edge) / edge.sqrMagnitude);
            nearestDistance = Mathf.Min(nearestDistance, Vector2.Distance(position, a + edge * t));
        }
        return nearestDistance;
    }
    private static bool OnSegment(Vector2 a, Vector2 b, Vector2 p) =>
        p.x >= Mathf.Min(a.x, b.x) - Epsilon && p.x <= Mathf.Max(a.x, b.x) + Epsilon &&
        p.y >= Mathf.Min(a.y, b.y) - Epsilon && p.y <= Mathf.Max(a.y, b.y) + Epsilon;
}
