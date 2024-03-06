using System.Collections;

public class NoAction : IAction
{
    public static readonly NoAction NO_ACTION = new NoAction();
    public string Name => "Nothing";

    public IEnumerator Execute()
    {
        yield return null;
    }

    public float GetPrice()
    {
        return 0;
    }
}
