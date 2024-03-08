using System.Collections;
using UnityEngine;

public class DoNothingTactic : AbstractTactic
{
    public DoNothingTactic(IReligion religion) : base(religion) { }

    public override void CreateAction(IPreacher preacher)
    {
        preacher.SetAction(NoAction.NO_ACTION);
    }

    public override void Clear()
    {
    }
}