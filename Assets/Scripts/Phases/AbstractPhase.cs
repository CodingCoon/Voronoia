using UnityEngine;

public abstract class AbstractPhase : MonoBehaviour
{
    public abstract PhaseType GetPhaseType();

    public abstract void OnStart();

    public virtual void OnEnd()
    {

    }

    public abstract AbstractPhase GetNextPhase();
}