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
        List<Preacher> removedPreacher = new List<Preacher>();

        game.GetReligions().ForEach(religion =>
        {
            if (religion.Faith <= 0)
            {
                removedPreacher.Add(religion.ReleaseMostExpensiveLeader());             
            }
        });

        StartCoroutine(RemoveLands(removedPreacher));
    }
    
    private IEnumerator RemoveLands(List<Preacher> removedPreacher)
    {
        foreach (Preacher preacher in removedPreacher)
        {
            yield return StartCoroutine(RemoveLand(preacher));
        }
        game.NextPhase();
    }


    private IEnumerator RemoveLand(Preacher preacher)
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
        List<Religion> allReligions = new List<Religion>(game.GetReligions());
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