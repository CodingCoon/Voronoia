using System.Collections;
using UnityEngine;

public class VoronoiPhase : MonoBehaviour, IPhase
{
    [SerializeField] private EvaluatePhase evaluatePhase;
    [SerializeField] private VoronoiController voronoi;
    [SerializeField] private Game game;   
    
    public PhaseType GetPhaseType()
    {
        return PhaseType.VORONOI;
    }

    public void OnStart()
    {
        voronoi.MarkDirty();
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
        //game.NextRound();
    }


    public void OnEnd()
    {
    }

    public IPhase GetNextPhase()
    {
        return evaluatePhase;
    }
}
