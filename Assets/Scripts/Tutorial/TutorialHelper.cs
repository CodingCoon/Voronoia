using System.Linq;
using UnityEngine;

public class TutorialHelper : MonoBehaviour
{
    [SerializeField] private Game game;

    public bool PlayersLeaderHasAction()
    {
        // todo etwas sehr aufwendig jeden frame
        foreach (var item in game.GetVoronations())
        {
            if (item.IsPlayer)
            {
                Leader p = item.GetLeaders().First();
                return p.HasAction();
            }
        }
        return false;
    }

    public bool IsVoronoiPhase()
    {
        return game.PhaseType == PhaseType.VORONOI;
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
