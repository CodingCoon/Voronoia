using System.Collections.Generic;
using UnityEngine;

public class VoronoiController : MonoBehaviour
{
    [SerializeField] private Game game;
    [SerializeField] private Map map;
    [SerializeField] private VoronoiCalculator calculator;

    private bool changed = true;

    private void Update()
    {
        if (changed)
        {
            Repaint();
            changed = false;
        }
    }

    private void Repaint()
    {
        List<IVoronoiCellOwner> cellOwners = new List<IVoronoiCellOwner>();
        game.GetPreachers().ForEach(p => cellOwners.Add(p));
        List<CCVoronoiCell> cells = calculator.CreateCells(cellOwners);
        
        AssignCellsToPreachers(cells);
    }

    private void AssignCellsToPreachers(List<CCVoronoiCell> cells)
    {
        cells.ForEach(cell => cell.Owner.UpdateVoronoi(cell.Points));
    }

    internal void MarkDirty()
    {
        changed = true;
    }
}