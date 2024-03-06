using UnityEngine;

public class ActionPhase : MonoBehaviour, IPhase
{
    [SerializeField] private ApplyPhase applyPhase;
    [SerializeField] private Game game;

    
    public PhaseType GetPhaseType()
    {
        return PhaseType.ACTION;
    }

    public void OnStart()
    {
        game.GetPreachers().ForEach(p => p.Reset());
    }

    public void OnEnd()
    {
        foreach (Preacher preacher in game.GetPreachers())
        {
            if (! preacher.HasAction() && preacher.Religion.IsAi)
            {
                preacher.Religion.Tactic.CreateAction(preacher);
            }
        }
    }

    public IPhase GetNextPhase()
    {
        return applyPhase;
    }
}
