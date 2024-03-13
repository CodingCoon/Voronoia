public abstract class AbstractTactic : ITactic
{
    protected readonly IVoronation voronation;

    protected AbstractTactic(IVoronation voronation)
    {
        this.voronation = voronation;
    }

    public abstract void CreateAction(ILeader leader);

    public abstract void Clear();
}