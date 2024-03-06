public class RisePowerTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RisePowerTactic(IReligion religion) : base(religion) {}

    public override void CreateAction(IPreacher preacher)
    {
        IAction action = new ImprovePowerAction(preacher);
        if (action.GetPrice() <= religion.Faith - blockedMoney) 
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
