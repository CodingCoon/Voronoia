using System.Collections;
using UnityEngine;

public class DoNothingTactic : AbstractTactic
{
    public DoNothingTactic(IVoronation religion) : base(religion) { }

    public override void CreateAction(ILeader preacher)
    {
        preacher.SetAction(NoAction.NO_ACTION);
    }

    public override void Clear()
    {
    }
}