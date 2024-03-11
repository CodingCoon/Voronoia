using System.Collections;
using UnityEngine;

public class IncreaseIncomeAction : IAction, IPlannedAction
{
    private static float INCREASE = 0.1f;
    public string Name => "Improve Income";

    private static readonly float PRICE_FACTOR = 40f;
    private readonly ILeader preacher;

    public IncreaseIncomeAction(ILeader preacher)
    {
        this.preacher = preacher;
    }

    public IEnumerator Execute()
    {
        SoundManager.PlaySound("Improve Leader");
        yield return preacher.ShowVFX(Name);
        preacher.ImproveInfluence();
    }

    public float GetPrice()
    {
        return -PRICE_FACTOR * (preacher.Income + INCREASE);
    }

    public string GetDetailedInfos()
    {
        return "Income factor: " + preacher.Income + " -> " + (preacher.Income + INCREASE);
    }

    public void UpdateTarget(Vector2 position){}
}