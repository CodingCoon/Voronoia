public class RisePowerTactic : AbstractTactic
{
    public RisePowerTactic(IVoronation voronation) : base(voronation) {}

    public override void CreateAction(ILeader leader)
    {
        leader.SetAction(new ImprovePowerAction(leader));
    }

    public override void Clear()
    {
    }
}
