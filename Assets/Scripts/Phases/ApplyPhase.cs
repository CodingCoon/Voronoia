using System.Collections;
using UnityEngine;
public class ApplyPhase : AbstractPhase
{
    [SerializeField] private VoronoiPhase voronoiPhase;
    [SerializeField] private Game game;
    public override PhaseType GetPhaseType() => PhaseType.APPLY;
    public override void OnStart()
    {
        game.ResolveRound();
        game.RunPhase(this, ExecuteActions());
    }
    private IEnumerator ExecuteActions()
    {
        yield return game.PresentActions();
    }
    public override AbstractPhase GetNextPhase() => voronoiPhase;
}
