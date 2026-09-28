using System;
using System.Collections.Generic;
using UnityEngine;
using VoronationCore;

namespace VoronationGeometry
{
    /// <summary>
    /// Data-only form of the existing weighted bisector rule. Increment 03 may replace the rule itself.
    /// </summary>
    public sealed class LegacyVoronoiCellCalculator : ICellCalculator
    {
        public IReadOnlyList<CalculatedCell> Calculate(float halfMapSize, IReadOnlyList<CellSite> sites)
        {
            if (!PolygonGeometry.IsFinite(halfMapSize) || halfMapSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(halfMapSize));
            if (sites == null) throw new ArgumentNullException(nameof(sites));
            ValidateSites(halfMapSize, sites);

            var result = new List<CalculatedCell>(sites.Count);
            for (int i = 0; i < sites.Count; i++)
            {
                List<Vector2> polygon = CreateMapBounds(halfMapSize);
                for (int j = 0; j < sites.Count && polygon.Count > 0; j++)
                {
                    if (i == j) continue;
                    CellSite site = sites[i];
                    CellSite other = sites[j];
                    Vector2 center = (site.Position * other.Power + other.Position * site.Power) /
                                     (site.Power + other.Power);
                    Vector2 normal = (center - site.Position).normalized;
                    polygon = Clip(polygon, center, normal);
                }

                polygon = Clean(polygon);
                if (polygon.Count < 3)
                {
                    result.Add(new CalculatedCell(sites[i].KnightId, polygon, 0));
                    continue;
                }
                var points3 = new List<Vector3>(polygon.Count);
                foreach (Vector2 point in polygon) points3.Add(point);
                Vector3[] validated = PolygonGeometry.Validate(points3);
                polygon.Clear();
                foreach (Vector3 point in validated) polygon.Add(point);
                result.Add(new CalculatedCell(sites[i].KnightId, polygon, PolygonGeometry.Area(validated)));
            }
            return result;
        }

        private static void ValidateSites(float halfMapSize, IReadOnlyList<CellSite> sites)
        {
            for (int i = 0; i < sites.Count; i++)
            {
                CellSite site = sites[i];
                if (!PolygonGeometry.IsFinite(site.Position) || !PolygonGeometry.IsFinite(site.Power) || site.Power <= 0)
                    throw new InvalidOperationException("Invalid knight position or power.");
                if (Mathf.Abs(site.Position.x) > halfMapSize + PolygonGeometry.Epsilon ||
                    Mathf.Abs(site.Position.y) > halfMapSize + PolygonGeometry.Epsilon)
                    throw new InvalidOperationException("Knight outside map bounds.");
                for (int j = 0; j < i; j++)
                    if ((site.Position - sites[j].Position).sqrMagnitude <
                        RoundResolver.PositionConflictDistance * RoundResolver.PositionConflictDistance)
                        throw new InvalidOperationException("Two knights occupy the same position.");
            }
        }

        private static List<Vector2> CreateMapBounds(float halfMapSize) => new List<Vector2>
        {
            new Vector2(-halfMapSize, -halfMapSize),
            new Vector2(-halfMapSize, halfMapSize),
            new Vector2(halfMapSize, halfMapSize),
            new Vector2(halfMapSize, -halfMapSize)
        };

        // The current site's half-plane is the side opposite the normal.
        private static List<Vector2> Clip(IReadOnlyList<Vector2> input, Vector2 planePoint, Vector2 normal)
        {
            var output = new List<Vector2>(input.Count + 1);
            for (int i = 0; i < input.Count; i++)
            {
                Vector2 start = input[i];
                Vector2 end = input[(i + 1) % input.Count];
                float startDistance = Vector2.Dot(start - planePoint, normal);
                float endDistance = Vector2.Dot(end - planePoint, normal);
                bool startInside = startDistance <= PolygonGeometry.Epsilon;
                bool endInside = endDistance <= PolygonGeometry.Epsilon;
                if (startInside) output.Add(start);
                if (startInside == endInside) continue;
                float denominator = startDistance - endDistance;
                if (Mathf.Abs(denominator) <= PolygonGeometry.Epsilon) continue;
                float factor = Mathf.Clamp01(startDistance / denominator);
                Vector2 intersection = start + (end - start) * factor;
                if (!PolygonGeometry.IsFinite(intersection)) throw new InvalidOperationException("Non-finite cell boundary.");
                output.Add(intersection);
            }
            return output;
        }

        private static List<Vector2> Clean(IReadOnlyList<Vector2> input)
        {
            var result = new List<Vector2>(input.Count);
            foreach (Vector2 point in input)
            {
                if (result.Count == 0 || Vector2.Distance(result[result.Count - 1], point) >= 0.01f)
                    result.Add(point);
            }
            if (result.Count > 1 && Vector2.Distance(result[0], result[result.Count - 1]) < 0.01f)
                result.RemoveAt(result.Count - 1);
            return result;
        }
    }
}
