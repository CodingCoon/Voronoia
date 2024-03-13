using System.Collections;
using UnityEngine;

public class DoNothingTactic : AbstractTactic
{
    public DoNothingTactic(IVoronation voronation) : base(voronation) { }

    public override void CreateAction(ILeader leader)
    {
        leader.SetAction(NoAction.NO_ACTION);
    }

    public override void Clear()
    {
    }
}