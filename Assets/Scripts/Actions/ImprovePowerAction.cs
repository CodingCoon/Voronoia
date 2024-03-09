using System.Collections;
using UnityEngine;

public class ImprovePowerAction : IAction, IPlannedAction
{
    public string Name => "Improve Power";

    private static readonly float PRICE_FACTOR = 40f; 
    private static readonly float DURATION = 1f;
    private readonly ILeader preacher;

    public ImprovePowerAction(ILeader preacher)
    {
        this.preacher = preacher;
    }

    public IEnumerator Execute()
    {
        // Todo VFX
        // Todo SFX
        yield return new WaitForSeconds(DURATION);
        preacher.ImprovePower();
    }

    public float GetPrice()
    {
        return -PRICE_FACTOR * (preacher.Power + 0.1f);
    }

    public void UpdateTarget(Vector2 position) {}

    public string GetDetailedInfos()
    {
        return "Power factor: " + preacher.Income + " -> " + preacher.Income + 0.1f;
    }
}