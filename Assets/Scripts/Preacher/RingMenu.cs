using System.Collections.Generic;
using UnityEngine;

public class RingMenu : MonoBehaviour
{
    [SerializeField] private Preacher preacher;
    [SerializeField] private PreacherKnob preacherKnob;

    internal void OnShow()
    {
        if (GameManager.Instance.IsTutorial) 
        {
            List<MenuButton.ActionType> disabledActionTypes = TutorialManager.Instance.DisabledActionTypes;
            foreach (var item in GetComponentsInChildren<MenuButton>())
            {
                item.SetInteractable(! disabledActionTypes.Contains(item.GetActionType()));
            }
        }
    }

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
        preacherKnob.StartDrag(PreacherKnob.PreviewType.SPLIT);
    }

    private void StartMove()
    {
        preacherKnob.StartDrag(PreacherKnob.PreviewType.MOVE);
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