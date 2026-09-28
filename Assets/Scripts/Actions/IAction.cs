public interface IAction
{
    string Name { get; }
    float GetPrice();
    string GetDetailedInfos();
}
