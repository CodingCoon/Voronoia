﻿using System.Collections;
using UnityEngine;

public class ApplyPhase : AbstractPhase
{
    [SerializeField] private VoronoiPhase voronoiPhase;
    [SerializeField] private Game game;

    public override PhaseType GetPhaseType()
    {
        return PhaseType.APPLY;
    }

    public override void OnStart()
    {
        StartCoroutine(ExecuteActions());   
    }

    private IEnumerator ExecuteActions()
    {
        foreach (Leader preacher in game.GetPreachers())
        {
            yield return StartCoroutine(preacher.ApplyAction());
        }
        game.NextPhase();
    }

    public override void OnEnd()
    {
    }

    public override AbstractPhase GetNextPhase()
    {
        return voronoiPhase;
    }
}