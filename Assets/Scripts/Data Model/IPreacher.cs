public interface IPreacher
{
    float Power { get; }
    float Influence { get; }

    void ImproveInfluence();
    void ImprovePower();

    void SetAction(IAction action);
}
