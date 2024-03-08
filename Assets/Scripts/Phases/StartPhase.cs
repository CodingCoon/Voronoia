using System.Collections;
using UnityEngine;

public class StartPhase : AbstractPhase
{
    [SerializeField] private ActionPhase actionPhase;
    [SerializeField] private TutorialSetup tutorialSetup;
    [SerializeField] private Random6PlayerSetup defaultGameSetup;
    [SerializeField] private Game game;

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

        StartCoroutine(HideBlend());   
    }

    private IEnumerator HideBlend()
    {

        yield return null;
        game.NextPhase();
    }

    public override void OnEnd() {}

    public override AbstractPhase GetNextPhase()
    {
        return actionPhase;
    }
}