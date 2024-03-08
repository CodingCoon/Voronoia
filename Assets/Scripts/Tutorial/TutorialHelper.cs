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
                print(p.HasAction());
                return p.HasAction();
            }
        }
        return false;
    }

    public bool IsApplyPhase()
    {
        return game.PhaseType == PhaseType.APPLY;
    }


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
