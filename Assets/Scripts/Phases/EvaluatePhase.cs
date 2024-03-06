using UnityEngine;

public class EvaluatePhase : MonoBehaviour, IPhase
{
    [SerializeField] private ActionPhase actionPhase;
    [SerializeField] private Game game;

    public PhaseType GetPhaseType()
    {
        return PhaseType.EVALUATION;
    }

    public void OnStart()
    {
        game.GetPreachers().ForEach(p => p.Evaluate());
        game.GetReligions().ForEach(r => r.UpdateIncome());
    }

    public void OnEnd()
    {
    }

    public IPhase GetNextPhase()
    {
        return actionPhase;
    }
}