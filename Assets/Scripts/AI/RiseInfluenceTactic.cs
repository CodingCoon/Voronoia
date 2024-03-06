public class RiseInfluenceTactic : AbstractTactic
{
    private float blockedMoney = 0;

    public RiseInfluenceTactic(IReligion religion) : base(religion) {}

    public override void CreateAction(IPreacher preacher)
    {
        IAction action = new ImproveInfluenceAction(preacher);
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
