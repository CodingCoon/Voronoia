public class NoAction : IAction
{
    public static readonly NoAction NO_ACTION = new NoAction();
    public string Name => "No action";
    public string GetDetailedInfos() => "Do nothing";
    public float GetPrice() => 0;
}
