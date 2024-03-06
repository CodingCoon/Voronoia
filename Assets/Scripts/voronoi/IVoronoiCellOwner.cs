using System.Collections.Generic;
using UnityEngine;

public interface IVoronoiCellOwner
{
    float Power { get;  }

    Vector2 GetPosition();

    // Needs to be Vector3, we need only Vector2, but deeper consumers needs Vector3
    void UpdateVoronoi(List<Vector3> positions);

}