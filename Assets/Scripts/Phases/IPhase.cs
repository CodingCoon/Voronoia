public interface IPhase 
{
    PhaseType GetPhaseType();
    
    void OnStart();

    void OnEnd();

    IPhase GetNextPhase();
}