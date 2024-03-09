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
        List<Leader> removedPreacher = new List<Leader>();

        game.GetVoronations().ForEach(religion =>
        {
            if (religion.Money <= 0)
            {
                removedPreacher.Add(religion.ReleaseMostExpensiveLeader());             
            }
        });

        StartCoroutine(RemoveLands(removedPreacher));
    }
    
    private IEnumerator RemoveLands(List<Leader> removedPreacher)
    {
        foreach (Leader preacher in removedPreacher)
        {
            yield return StartCoroutine(RemoveLand(preacher));
        }
        game.NextPhase();
    }


    private IEnumerator RemoveLand(Leader preacher)
    {
        float duration = 1;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            preacher.Dissolve(timeElapsed / duration);
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