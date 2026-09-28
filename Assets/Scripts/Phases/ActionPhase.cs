using UnityEngine;

public class ActionPhase : AbstractPhase
{
    [SerializeField] private ApplyPhase applyPhase;
    [SerializeField] private Game game;
    [SerializeField] private EvaluationPanel evaluationPanel;

    
    public override PhaseType GetPhaseType()
    {
        return PhaseType.ACTION;
    }

    public override void OnStart()
    {
        evaluationPanel.Clear();
    }

    public override void OnEnd()
    {
        game.ClearInteraction();
        foreach (Leader preacher in game.GetPreachers())
        {
            if (! preacher.HasAction() && preacher.Voronation.IsAi)
            {
                preacher.Voronation.Tactic.CreateAction(preacher);
            }
        }
    }

    public override AbstractPhase GetNextPhase()
    {
        return applyPhase;
    }
}
