using System.Collections;
using UnityEngine;

public class ImproveInfluenceAction : IAction
{
    public string Name => "Improve Influence";

    private static readonly float PRICE_FACTOR = 40f;
    private static readonly float DURATION = 1f;
    private readonly IPreacher preacher;

    public ImproveInfluenceAction(IPreacher preacher)
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
        return -PRICE_FACTOR * (preacher.Influence + 0.1f);
    }
}