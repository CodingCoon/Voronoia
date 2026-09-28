using UnityEngine;

public class ImprovePowerAction : IAction, IPlannedAction
{
    private const float INCREASE = 0.1f;
    private const float PRICE_FACTOR = 40f;
    private readonly ILeader preacher;
    public string Name => "Improve Power";

    public ImprovePowerAction(ILeader preacher) { this.preacher = preacher; }
    public float GetPrice() => -PRICE_FACTOR * (preacher.Power + INCREASE);
    public void UpdateTarget(Vector2 position) { }
    public string GetDetailedInfos() =>
        "Power factor: \n" + preacher.Power + " -> " + (preacher.Power + INCREASE);
}
