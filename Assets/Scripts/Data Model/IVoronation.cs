using System.Collections;
using UnityEngine;

public interface IVoronation
{
    string Name { get; }

    bool IsAi { get;  }

    Color Color { get; }

    ITactic Tactic { get; }

    float Money { get; }

    Leader AddPreacher(Vector2 position);
}
