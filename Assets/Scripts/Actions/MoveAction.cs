using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class MoveAction : IAction, IPlannedAction
{
    public string Name => "Move";

    private static readonly float PRICE_PER_DISTANCE = 10f;
    private static readonly float DURATION = 1f;
    private readonly PreacherKnob preacherKnob;
    private Vector2 newPosition;

    public MoveAction(PreacherKnob preacher, Vector2 newPosition)
    {
        this.preacherKnob = preacher;
        this.newPosition = newPosition;
    }

    public IEnumerator Execute()
    {
        float progress = 0f;
        float elapsedTime = 0f;
        Vector3 startPos = preacherKnob.transform.position;
        Vector3 targetPos = new Vector3(newPosition.x, newPosition.y, startPos.z);

        while (progress < 1f)
        {
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / DURATION;
            if (progress > 1f) progress = 1f;
            preacherKnob.transform.position = Vector3.Lerp(startPos, targetPos, progress);
            yield return null;
        }

        preacherKnob.HidePreview();
    }

    public float GetPrice()
    {
        Debug.Log(Vector2.Distance(newPosition, preacherKnob.transform.position));
        return -PRICE_PER_DISTANCE * Vector2.Distance(newPosition, preacherKnob.transform.position);
    }

    public void UpdateTarget(Vector2 position)
    {
        newPosition = position;
    }

    public string GetDetailedInfos()
    {
        return "Move to another location.";
    }
}