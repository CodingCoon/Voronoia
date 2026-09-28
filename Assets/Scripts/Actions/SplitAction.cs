using UnityEngine;

public class SplitAction : IAction, IPlannedAction
{
    private const float SPLIT_PRICE = 25f;
    private readonly Vector2 newPosition;
    public string Name => "Split";
    public Vector2 Target => newPosition;

    public SplitAction(Leader preacher, Vector2 newPosition) { this.newPosition = newPosition; }
    public float GetPrice() => -SPLIT_PRICE;
    public void UpdateTarget(Vector2 position) { }
    public string GetDetailedInfos() => "Split and move new created leader.";
}
