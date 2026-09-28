using UnityEngine;

public class IncreaseIncomeAction : IAction, IPlannedAction
{
    private const float INCREASE = 0.1f;
    private const float PRICE_FACTOR = 40f;
    private readonly ILeader preacher;
    public string Name => "Improve Income";

    public IncreaseIncomeAction(ILeader preacher) { this.preacher = preacher; }
    public float GetPrice() => -PRICE_FACTOR * (preacher.Income + INCREASE);
    public void UpdateTarget(Vector2 position) { }
    public string GetDetailedInfos() =>
        "Income factor: \n" + preacher.Income + " -> " + (preacher.Income + INCREASE);
}
