public abstract class AbstractTactic : ITactic
{
    protected readonly IVoronation religion;

    protected AbstractTactic(IVoronation religion)
    {
        this.religion = religion;
    }

    public abstract void CreateAction(ILeader preacher);

    public abstract void Clear();
}