using System;
using System.Collections.Generic;
using UnityEngine;

public class RingMenu : MonoBehaviour
{
    [SerializeField] private Leader preacher;
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
        preacher.SetAction(new ImprovePowerAction(preacher));
    }

    private void ImproveInfluence()
    {
        preacher.SetAction(new IncreaseIncomeAction(preacher));
    }

    internal void Plan(bool hovered, MenuButton.ActionType actionType)
    {
        if (hovered)
        {
            PlannedActionController.INSTANCE.Plan(CreatePlannedAction(actionType));
        }
        else
        {
            PlannedActionController.INSTANCE.UnPlan();
        }
    }

    private IPlannedAction CreatePlannedAction(MenuButton.ActionType actionType)
    {
        switch (actionType)
        {
            case MenuButton.ActionType.MOVE: return new MoveAction(preacherKnob, preacher.transform.position);
            case MenuButton.ActionType.SPLIT: return new SplitAction(preacher, preacher.transform.position);
            case MenuButton.ActionType.IMPROVE_INFLUENCE: return new IncreaseIncomeAction (preacher);
            case MenuButton.ActionType.IMPROVE_POWER: return new ImprovePowerAction(preacher);
        }
        return null;
    } 
}