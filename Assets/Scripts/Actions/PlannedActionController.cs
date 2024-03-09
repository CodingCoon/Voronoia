using UnityEngine;

public class PlannedActionController : MonoBehaviour
{
    public static PlannedActionController INSTANCE;


    private IPlannedAction plannedAction;
    public bool IsPlanned => plannedAction != null;
    public string PlannedAction => plannedAction.Name;
    public string PlannedActionPrice => "" + (int) plannedAction.GetPrice();
    public string PlannedActionInfo => plannedAction.GetDetailedInfos();


    private void Awake()
    {
        INSTANCE = this;
    }

    internal void Plan(IPlannedAction action)
    {
        plannedAction = action; 
    }

    internal void UnPlan()
    {
        plannedAction = null;
    }

    internal void UpdatePosition(Vector2 position)
    {
        if (plannedAction != null)
        {
            plannedAction.UpdateTarget(position);
        }
    }
}
