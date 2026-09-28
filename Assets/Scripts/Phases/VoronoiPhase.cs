using System.Collections;
using UnityEngine;
public class VoronoiPhase : AbstractPhase
{
    [SerializeField] private EvaluatePhase evaluatePhase;
    [SerializeField] private VoronoiController voronoi;
    [SerializeField] private Game game;
    public override PhaseType GetPhaseType() => PhaseType.VORONOI;
    public override void OnStart() { game.RunPhase(this, Calculate()); }
    private IEnumerator Calculate()
    {
        game.ApplyAccountingCells();
        // Keep the phase observable for tutorial/UI; readiness is determined by Recalculate.
        yield return null;
    }
    public override AbstractPhase GetNextPhase() => evaluatePhase;
}
