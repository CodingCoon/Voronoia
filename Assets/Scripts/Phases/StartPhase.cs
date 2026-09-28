using System.Collections;
using UnityEngine;

public class StartPhase : AbstractPhase
{
    [SerializeField] private ActionPhase actionPhase;
    [SerializeField] private TutorialSetup tutorialSetup;
    [SerializeField] private Random6PlayerSetup defaultGameSetup;
    [SerializeField] private Game game;
    [SerializeField] private VoronoiController voronoi;

    public override PhaseType GetPhaseType()
    {
        return PhaseType.START;
    }

    public override void OnStart()
    {
        if (GameManager.Instance.IsTutorial)
        {
            tutorialSetup.GeneratePlayers();
        }
        else
        {
            defaultGameSetup.GeneratePlayers();
        }

        game.RunPhase(this, HideBlend());
    }

    private IEnumerator HideBlend()
    {
        yield return null;
        game.InitializeMatch(voronoi.HalfMapSize);
    }

    public override void OnEnd() {}

    public override AbstractPhase GetNextPhase()
    {
        return actionPhase;
    }
}
