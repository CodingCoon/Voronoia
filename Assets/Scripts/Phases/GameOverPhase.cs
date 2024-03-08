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
    }

    public override AbstractPhase GetNextPhase()
    {
        throw new System.Exception("do not call");
    }
}