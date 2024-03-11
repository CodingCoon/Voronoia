using System.Collections;

public interface ILeader
{
    float Power { get; }
    float Income { get; }

    void ImproveInfluence();
    void ImprovePower();

    void SetAction(IAction action);
    
    IEnumerator ShowVFX(string name);
}
