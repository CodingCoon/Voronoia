using System.Collections;
using UnityEngine;

public class DeathPhase : AbstractPhase
{
    [SerializeField] private ActionPhase actionPhase;
    [SerializeField] private AbstractPhase gameOverPhase;
    [SerializeField] private Game game;
    [SerializeField] private VoronoiController voronoi;
    [SerializeField] private EvaluationPanel evaluationPanel;

    public override PhaseType GetPhaseType() => PhaseType.DEATH;
    public override void OnStart() { game.RunPhase(this, ResolveDeaths(), false); }

    private IEnumerator ResolveDeaths()
    {
        game.ClearInteraction();
        yield return game.PresentDeaths(evaluationPanel);
    }

    public override AbstractPhase GetNextPhase() => game.IsOver() ? gameOverPhase : actionPhase;
}
