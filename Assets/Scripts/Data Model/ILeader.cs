public interface ILeader
{
    float Power { get; }
    float Income { get; }
    void SetAction(IAction action);
}
