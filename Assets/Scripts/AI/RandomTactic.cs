using System;

public class RandomTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RandomTactic(IVoronation voronation) : base(voronation) {}

    public override void CreateAction(ILeader leader)
    {
        IAction action = new IncreaseIncomeAction(leader);
        if (action.GetPrice() <= voronation.Money - blockedMoney)
        {
            leader.SetAction(action);
            blockedMoney += action.GetPrice();
        }
    }

    public override void Clear()
    {
        blockedMoney = 0;
    }

}
