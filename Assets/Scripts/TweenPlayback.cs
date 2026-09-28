using System;
using System.Collections;
using DG.Tweening;

public static class TweenPlayback
{
    // DOTween's wait also ends when killed. A cancelled animation is not a completed phase.
    public static IEnumerator Wait(Tween tween)
    {
        bool completed = false;
        tween.onComplete += () => completed = true;
        yield return tween.WaitForCompletion();
        if (!completed) throw new OperationCanceledException("Animation was cancelled before completion.");
    }
}