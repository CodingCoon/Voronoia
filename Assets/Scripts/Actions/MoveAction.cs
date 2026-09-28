using UnityEngine;

public class MoveAction : IAction, IPlannedAction
{
    private const float PRICE_PER_DISTANCE = 10f;
    private readonly PreacherKnob preacherKnob;
    private Vector2 newPosition;
    public string Name => "Move";
    public Vector2 Target => newPosition;

    public MoveAction(PreacherKnob preacher, Vector2 newPosition)
    {
        preacherKnob = preacher;
        this.newPosition = newPosition;
    }

    public float GetPrice() => -PRICE_PER_DISTANCE * Vector2.Distance(newPosition, preacherKnob.transform.position);
    public void UpdateTarget(Vector2 position) { newPosition = position; }
    public string GetDetailedInfos() => "Move to another location.";
}
