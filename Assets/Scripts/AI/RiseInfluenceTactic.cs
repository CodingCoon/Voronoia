public class RiseInfluenceTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RiseInfluenceTactic(IVoronation religion) : base(religion) {}

    public override void CreateAction(ILeader preacher)
    {
        IAction action = new IncreaseIncomeAction(preacher);
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
