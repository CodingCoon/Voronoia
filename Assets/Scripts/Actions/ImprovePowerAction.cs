using System.Collections;
using UnityEngine;

public class ImprovePowerAction : IAction, IPlannedAction
{
    private static float INCREASE = 0.1f;
    public string Name => "Improve Power";

    private static readonly float PRICE_FACTOR = 40f; 
    private readonly ILeader preacher;

    public ImprovePowerAction(ILeader preacher)
    {
        this.preacher = preacher;
    }

    public IEnumerator Execute()
    {
        SoundManager.PlaySound("Improve Leader");
        yield return preacher.ShowVFX(Name);
        preacher.ImprovePower();
    }

    public float GetPrice()
    {
        return -PRICE_FACTOR * (preacher.Power + INCREASE);
    }

    public void UpdateTarget(Vector2 position) {}

    public string GetDetailedInfos()
    {
        return "Power factor: \n" + preacher.Income + " -> " + (preacher.Income + INCREASE);
    }
}