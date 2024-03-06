using UnityEngine;

public class RingMenu : MonoBehaviour
{
    [SerializeField] private Preacher preacher;

    internal void Execute(MenuButton.ActionType actionType)
    {
        switch (actionType)
        {
            case MenuButton.ActionType.IMPROVE_POWER: 
                ImprovePower();
                break;
            case MenuButton.ActionType.IMPROVE_INFLUENCE: 
                ImproveInfluence(); 
                break;
            case MenuButton.ActionType.MOVE:
                StartMove();
                break;
            case MenuButton.ActionType.SPLIT:
                StartSplit();
                break;

        }
    }

    private void StartSplit()
    {
        print("Todo Split");
    }

    private void StartMove()
    {
        print("Todo Move");
    }

    private void ImprovePower()
    {
        print("Improve Power");
        preacher.SetAction(new ImprovePowerAction(preacher));
    }

    private void ImproveInfluence()
    {
        print("Improve Influence");
        preacher.SetAction(new ImproveInfluenceAction(preacher));
    }
}