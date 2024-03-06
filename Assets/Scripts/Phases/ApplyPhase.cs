using System.Collections;
using UnityEngine;

public class ApplyPhase : MonoBehaviour, IPhase
{
    [SerializeField] private VoronoiPhase voronoiPhase;
    [SerializeField] private Game game;

    public PhaseType GetPhaseType()
    {
        return PhaseType.APPLY;
    }

    public void OnStart()
    {
        StartCoroutine(ExecuteActions());   
    }

    private IEnumerator ExecuteActions()
    {
        foreach (Preacher preacher in game.GetPreachers())
        {
            yield return StartCoroutine(preacher.ApplyAction());
        }
    }

    public void OnEnd()
    {
    }

    public IPhase GetNextPhase()
    {
        return voronoiPhase;
    }
}