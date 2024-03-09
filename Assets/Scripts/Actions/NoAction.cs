using System.Collections;

public class NoAction : IAction
{
    public static readonly NoAction NO_ACTION = new NoAction();
    public string Name => "No action";

    public IEnumerator Execute()
    {
        yield return null;
    }

    public string GetDetailedInfos()
    {
        return "Will do nothing";
    }

    public float GetPrice()
    {
        return 0;
    }
}
