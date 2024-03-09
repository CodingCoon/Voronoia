public class RisePowerTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RisePowerTactic(IVoronation religion) : base(religion) {}

    public override void CreateAction(ILeader preacher)
    {
        IAction action = new ImprovePowerAction(preacher);
        if (action.GetPrice() <= religion.Money - blockedMoney) 
        {
            preacher.SetAction(action);
            blockedMoney += action.GetPrice();
        }
    }

    public override void Clear()
    {
        blockedMoney = 0;
    }
}
