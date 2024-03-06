public abstract class AbstractTactic : ITactic
{
    protected readonly IReligion religion;

    protected AbstractTactic(IReligion religion)
    {
        this.religion = religion;
    }

    public abstract void CreateAction(IPreacher preacher);

    public abstract void Clear();
}