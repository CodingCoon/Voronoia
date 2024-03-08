using System.Collections;
using UnityEngine;

public class VoronoiPhase : AbstractPhase
{
    [SerializeField] private EvaluatePhase evaluatePhase;
    [SerializeField] private VoronoiController voronoi;
    [SerializeField] private Game game;   
    
    public override PhaseType GetPhaseType()
    {
        return PhaseType.VORONOI;
    }

    public override void OnStart()
    {
        voronoi.MarkDirty();
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        // todo lass polygon für polygpn sich verändern
        yield return new WaitForSeconds(0.2f);
        game.NextPhase();
    }

    public override void OnEnd()
    {
    }

    public override AbstractPhase GetNextPhase()
    {
        return evaluatePhase;
    }
}
