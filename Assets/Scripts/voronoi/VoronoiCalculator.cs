using System.Collections.Generic;
using UnityEngine;
using VoronationCore;
using VoronationGeometry;

// Scene adapter retained so existing serialized references keep their GUID and role.
public class VoronoiCalculator : MonoBehaviour
{
    [SerializeField] private Map map;
    private readonly LegacyVoronoiCellCalculator calculator = new LegacyVoronoiCellCalculator();

    public List<CCVoronoiCell> CreateCells(List<IVoronoiCellOwner> sites)
    {
        var inputs = new List<CellSite>(sites.Count);
        for (int i = 0; i < sites.Count; i++)
            inputs.Add(new CellSite(new KnightId(new FactionId(1), i + 1), sites[i].GetPosition(), sites[i].Power));
        IReadOnlyList<CalculatedCell> calculated = calculator.Calculate(map.HalfMapSize, inputs);
        var result = new List<CCVoronoiCell>(calculated.Count);
        for (int i = 0; i < calculated.Count; i++)
            result.Add(new CCVoronoiCell(sites[i], new List<Vector2>(calculated[i].Points)));
        return result;
    }
}
