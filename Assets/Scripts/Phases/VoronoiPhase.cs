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
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        //game.NextRound();
    }


    public override void OnEnd()
    {
    }

    public override AbstractPhase GetNextPhase()
    {
        return evaluatePhase;
    }
}
