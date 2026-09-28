using System;
using System.Collections.Generic;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Diagnostics;
#endif
using UnityEngine;

public class VoronoiController : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private Map map;
    [SerializeField] private VoronoiCalculator calculator;
    public float HalfMapSize => map.HalfMapSize;

    public void Recalculate()
    {
        var owners = new List<IVoronoiCellOwner>();
        List<Leader> leaders = game.GetPreachers();
        leaders.ForEach(leader => owners.Add(leader));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        long calculationStart = Stopwatch.GetTimestamp();
#endif
        List<CCVoronoiCell> cells = calculator.CreateCells(owners);
        if (cells.Count != leaders.Count) throw new InvalidOperationException("Missing calculated cells.");
        var seen = new HashSet<IVoronoiCellOwner>();
        foreach (CCVoronoiCell cell in cells)
        {
            if (!owners.Contains(cell.Owner) || !seen.Add(cell.Owner))
                throw new InvalidOperationException("Invalid cell ownership.");
            Vector3[] validated = PolygonGeometry.Validate(cell.Points);
            if (validated.Length == 0) throw new InvalidOperationException("Empty calculated cell is not supported before increment 03.");
            foreach (Vector3 point in validated)
                if (Mathf.Abs(point.x) > map.HalfMapSize + 0.001f || Mathf.Abs(point.y) > map.HalfMapSize + 0.001f)
                    throw new InvalidOperationException("Calculated cell outside map.");
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        long assignmentStart = Stopwatch.GetTimestamp();
#endif
        var previous = new Dictionary<Leader, Vector3[]>();
        foreach (Leader leader in leaders) previous.Add(leader, leader.CopyBounds());
        try
        {
            foreach (CCVoronoiCell cell in cells) cell.Owner.UpdateVoronoi(cell.Points);
        }
        catch (Exception assignmentError)
        {
            var failures = new List<Exception> { assignmentError };
            foreach (var entry in previous)
            {
                try { entry.Key.UpdateVoronoi(new List<Vector3>(entry.Value)); }
                catch (Exception rollbackError) { failures.Add(rollbackError); }
            }
            throw new AggregateException("Cell assignment failed; round stopped.", failures);
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Increment00Diagnostics.GeometryFinished(assignmentStart - calculationStart,
            Stopwatch.GetTimestamp() - assignmentStart, cells.Count);
#endif
    }
}
