﻿public class RisePowerTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RisePowerTactic(IVoronation voronation) : base(voronation) {}

    public override void CreateAction(ILeader leader)
    {
        IAction action = new ImprovePowerAction(leader);
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
