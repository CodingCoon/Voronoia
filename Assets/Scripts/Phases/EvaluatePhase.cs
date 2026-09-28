using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class EvaluatePhase : AbstractPhase
{
    [SerializeField] private DeathPhase deathPhase;
    [SerializeField] private Game game;
    [SerializeField] private EvaluationPanel evaluationPanel;
    private Sequence sequence;
    public override PhaseType GetPhaseType() => PhaseType.EVALUATION;
    public override void OnStart() { game.RunPhase(this, Evaluate()); }

    private IEnumerator Evaluate()
    {
        List<Leader> leaders = game.GetPreachers();
        game.PrepareEvaluation();
        evaluationPanel.ShowRound(game.CurrentResult, game.GetHumanPlayer()?.Id);
        var animations = new List<Tween>();
        foreach (Leader leader in leaders) animations.Add(leader.ShowIncome());
        // Capture completion before yielding: other tweens can finish while the first is awaited.
        bool finished = false;
        sequence = DOTween.Sequence();
        foreach (Tween animation in animations) sequence.Join(animation);
        sequence.SetLink(gameObject).OnComplete(() => finished = true);
        yield return sequence.WaitForCompletion();
        if (!finished) throw new System.OperationCanceledException("Income presentation was cancelled.");
    }
    public override AbstractPhase GetNextPhase() => deathPhase;
    public override void Cancel() { sequence?.Kill(); }
}
