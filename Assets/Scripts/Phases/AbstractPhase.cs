using UnityEngine;

public abstract class AbstractPhase : MonoBehaviour
{
    public abstract PhaseType GetPhaseType();

    public abstract void OnStart();

    public abstract void OnEnd();

    public abstract AbstractPhase GetNextPhase();
}