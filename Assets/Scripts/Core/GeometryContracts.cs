using System;
using System.Collections.Generic;
using UnityEngine;

namespace VoronationCore
{
    public readonly struct CellSite
    {
        public KnightId KnightId { get; }
        public Vector2 Position { get; }
        public float Power { get; }

        public CellSite(KnightId knightId, Vector2 position, float power)
        {
            KnightId = knightId;
            Position = position;
            Power = power;
        }
    }

    public sealed class CalculatedCell
    {
        private readonly Vector2[] points;
        public KnightId KnightId { get; }
        public IReadOnlyList<Vector2> Points => points;
        public float Area { get; }

        public CalculatedCell(KnightId knightId, IReadOnlyList<Vector2> points, float area)
        {
            KnightId = knightId;
            this.points = new Vector2[points.Count];
            for (int i = 0; i < points.Count; i++) this.points[i] = points[i];
            Area = area;
        }
    }

    public interface ICellCalculator
    {
        IReadOnlyList<CalculatedCell> Calculate(float halfMapSize, IReadOnlyList<CellSite> sites);
    }

    public static class CoreGeometry
    {
        public const float Epsilon = 0.0001f;

        public static bool IsFinite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
        public static bool IsFinite(Vector2 value) => IsFinite(value.x) && IsFinite(value.y);

        public static bool Contains(IReadOnlyList<Vector2> polygon, Vector2 point)
        {
            if (polygon == null || polygon.Count < 3 || !IsFinite(point)) return false;
            bool inside = false;
            for (int i = 0; i < polygon.Count; i++)
            {
                Vector2 a = polygon[i], b = polygon[(i + 1) % polygon.Count];
                if (DistanceToSegment(point, a, b) <= Epsilon) return true;
                if ((a.y > point.y) != (b.y > point.y) &&
                    point.x < (b.x - a.x) * (point.y - a.y) / (b.y - a.y) + a.x)
                    inside = !inside;
            }
            return inside;
        }

        public static float BoundaryDistance(IReadOnlyList<Vector2> polygon, Vector2 point)
        {
            float minimum = float.PositiveInfinity;
            for (int i = 0; i < polygon.Count; i++)
                minimum = Mathf.Min(minimum, DistanceToSegment(point, polygon[i], polygon[(i + 1) % polygon.Count]));
            return minimum;
        }

        public static float Area(IReadOnlyList<Vector2> points)
        {
            if (points == null || points.Count < 3) return 0;
            Vector2 origin = points[0];
            double sum = 0;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 a = points[i] - origin;
                Vector2 b = points[(i + 1) % points.Count] - origin;
                if (!IsFinite(a) || !IsFinite(b)) throw new ArgumentException("Polygon contains non-finite coordinates.");
                sum += (double)a.x * b.y - (double)b.x * a.y;
            }
            float area = (float)(Math.Abs(sum) * 0.5);
            if (!IsFinite(area)) throw new ArgumentException("Polygon area is not finite.");
            return area;
        }

        private static float DistanceToSegment(Vector2 point, Vector2 a, Vector2 b)
        {
            Vector2 edge = b - a;
            if (edge.sqrMagnitude <= Epsilon * Epsilon) return Vector2.Distance(point, a);
            float t = Mathf.Clamp01(Vector2.Dot(point - a, edge) / edge.sqrMagnitude);
            return Vector2.Distance(point, a + edge * t);
        }
    }
}
