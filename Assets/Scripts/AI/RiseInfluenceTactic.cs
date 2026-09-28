public class RiseInfluenceTactic : AbstractTactic
{
    public RiseInfluenceTactic(IVoronation voronation) : base(voronation) {}

    public override void CreateAction(ILeader leader)
    {
        leader.SetAction(new IncreaseIncomeAction(leader));
    }

    public override void Clear()
    {
    }
}
