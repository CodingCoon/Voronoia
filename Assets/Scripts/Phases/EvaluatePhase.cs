using System.Collections;
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
        game.GetVoronations().ForEach(v => v.UpdateIncome());

        StartCoroutine(DoSth());
    }

    public IEnumerator DoSth()
    {
        yield return new WaitForSeconds(1f);
        Game.INSTANCE.NextPhase();
    }

    public override AbstractPhase GetNextPhase()
    {
        return deathPhase;
    }
}