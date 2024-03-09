using UnityEngine;

public interface IPlannedAction 
{
    string Name { get; }

    float GetPrice();

    void UpdateTarget(Vector2 position);

    string GetDetailedInfos();
}
