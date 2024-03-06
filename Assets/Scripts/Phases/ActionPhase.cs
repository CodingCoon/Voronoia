using UnityEngine;

public class ActionPhase : AbstractPhase
{
    [SerializeField] private ApplyPhase applyPhase;
    [SerializeField] private Game game;

    
    public override PhaseType GetPhaseType()
    {
        return PhaseType.ACTION;
    }

    public override void OnStart()
    {
        game.GetPreachers().ForEach(p => p.Reset());
    }

    public override void OnEnd()
    {
        foreach (Preacher preacher in game.GetPreachers())
        {
            if (! preacher.HasAction() && preacher.Religion.IsAi)
            {
                preacher.Religion.Tactic.CreateAction(preacher);
            }
        }
    }

    public override AbstractPhase GetNextPhase()
    {
        return applyPhase;
    }
}
