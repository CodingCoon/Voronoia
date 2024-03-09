using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPhase : AbstractPhase
{
    [SerializeField] private ActionPhase actionPhase;
    [SerializeField] private AbstractPhase gameOverPhase;

    [SerializeField] private Game game;

    public override PhaseType GetPhaseType()
    {
        return PhaseType.DEATH;
    }

    public override void OnStart()
    {
        List<Leader> removedLeader = new List<Leader>();

        game.GetVoronations().ForEach(voronation =>
        {
            if (voronation.Money <= 0)
            {
                removedLeader.Add(voronation.ReleaseMostExpensiveLeader());             
            }
        });

        StartCoroutine(RemoveLands(removedLeader));
    }
    
    private IEnumerator RemoveLands(List<Leader> removedLeader)
    {
        foreach (Leader Leader in removedLeader)
        {
            yield return StartCoroutine(RemoveLand(Leader));
        }
        game.NextPhase();
    }


    private IEnumerator RemoveLand(Leader leader)
    {
        float duration = 1;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            leader.Dissolve(timeElapsed / duration);
            yield return null;
        }
    }

    public override void OnEnd()
    {
        List<Voronation> allReligions = new List<Voronation>(game.GetVoronations());
        allReligions.ForEach(religion =>
        {
            if (religion.GetLeaderCount() == 0)
            {
                religion.GameOver();
            }
        });

    }

    public override AbstractPhase GetNextPhase()
    {
        if (game.IsOver()) return gameOverPhase;

        return actionPhase;
    }
}