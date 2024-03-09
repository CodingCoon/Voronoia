﻿using System.Collections;
using UnityEngine;

public class IncreaseIncomeAction : IAction, IPlannedAction
{
    public string Name => "Increase income";

    private static readonly float PRICE_FACTOR = 40f;
    private static readonly float DURATION = 1f;
    private readonly ILeader preacher;

    public IncreaseIncomeAction(ILeader preacher)
    {
        this.preacher = preacher;
    }

    public IEnumerator Execute()
    {
        // Todo VFX
        // Todo SFX
        yield return new WaitForSeconds(DURATION);
        preacher.ImproveInfluence();
    }

    public float GetPrice()
    {
        return -PRICE_FACTOR * (preacher.Income + 0.1f);
    }

    public string GetDetailedInfos()
    {
        return "Income factor: " + preacher.Income + " -> " + preacher.Income + 0.1f;
    }

    public void UpdateTarget(Vector2 position){}
}