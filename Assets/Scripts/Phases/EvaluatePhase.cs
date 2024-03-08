using UnityEngine;

public class EvaluatePhase : AbstractPhase
{
    [SerializeField] private DeathPhase deathPhase;
    [SerializeField] private Game game;

    public override PhaseType GetPhaseType()
    {
        return PhaseType.EVALUATION;
    }

    public override void OnStart()
    {
        game.GetPreachers().ForEach(p => p.Evaluate());
        game.GetReligions().ForEach(r => r.UpdateIncome());
    }

    public override AbstractPhase GetNextPhase()
    {
        return deathPhase;
    }
}