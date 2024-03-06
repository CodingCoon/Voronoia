using System.Collections;
using UnityEngine;

public class SplitAction : IAction
{
    public string Name => "Split";

    private static readonly float SPLIT_PRICE = 25f;
    private static readonly float DURATION = 1f;
    private readonly Preacher preacher;
    private readonly Vector2 newPosition;

    public SplitAction(Preacher preacher, Vector2 newPosition)
    {
        this.preacher = preacher;
        this.newPosition = newPosition;
    }

    public IEnumerator Execute()
    {
        PreacherKnob splittedKnob = preacher.Split(newPosition);
        float progress = 0f;
        float elapsedTime = 0f;

        preacher.HideKnob();
        while (progress < 1f)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / DURATION;
            if (progress > 1f) progress = 1f;
            splittedKnob.SetColor(progress);
            yield return null;
        }
    }


    public float GetPrice()
    {
        return -SPLIT_PRICE;
    }
}