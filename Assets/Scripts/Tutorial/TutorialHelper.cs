using System.Linq;
using UnityEngine;

public class TutorialHelper : MonoBehaviour
{
    [SerializeField] private Game game;

    public bool PlayersLeaderHasAction()
    {
        // todo etwas sehr aufwendig jeden frame
        foreach (var item in game.GetReligions())
        {
            if (item.IsPlayer)
            {
                Preacher p = item.GetPreachers().First();
                return p.HasAction();
            }
        }
        return false;
    }

    public bool IsDeathPhase()
    {
        return game.PhaseType == PhaseType.DEATH;
    }

    public bool IsApplyPhase()
    {
        return game.PhaseType == PhaseType.APPLY;
    }

    public bool IsEvaluationPhase()
    {
        return game.PhaseType == PhaseType.EVALUATION;
    }

    public bool IsActionPhase()
    {
        return game.PhaseType == PhaseType.ACTION;
    }
}
