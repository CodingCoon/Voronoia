using System.Collections;
using UnityEngine;

public class MoveAction : IAction
{
    public string Name => "Move";

    private static readonly float PRICE_PER_DISTANCE = 10f;
    private static readonly float DURATION = 1f;
    private readonly PreacherKnob preacherKnob;
    private readonly Vector2 newPosition;

    public MoveAction(PreacherKnob preacher, Vector2 newPosition)
    {
        this.preacherKnob = preacher;
        this.newPosition = newPosition;
    }

    public IEnumerator Execute()
    {
        float progress = 0f;
        float elapsedTime = 0f;
        Vector2 startPos = preacherKnob.transform.position;

        while (progress < 1f)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / DURATION;
            if (progress > 1f) progress = 1f;
            preacherKnob.transform.position = Vector2.Lerp(startPos, newPosition, progress);
            yield return null;
        }

        preacherKnob.HidePreview();
    }

    public float GetPrice()
    {
        return -PRICE_PER_DISTANCE * Vector2.Distance(newPosition, preacherKnob.transform.position);
    }
}