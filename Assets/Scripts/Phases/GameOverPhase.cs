using UnityEngine;

public class GameOverPhase : AbstractPhase
{
    [SerializeField] private Game game;

    public override PhaseType GetPhaseType()
    {
        return PhaseType.GAME_OVER;
    }

    public override void OnStart()
    {
        game.ClearInteraction();
        game.ShowResult();
    }

    public override AbstractPhase GetNextPhase()
    {
        return this;
    }
}
