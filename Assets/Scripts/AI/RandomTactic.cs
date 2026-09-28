using System;

public class RandomTactic : AbstractTactic
{
    public RandomTactic(IVoronation voronation) : base(voronation) {}

    public override void CreateAction(ILeader leader)
    {
        leader.SetAction(new IncreaseIncomeAction(leader));
    }

    public override void Clear()
    {
    }

}
