using System.Collections;
using UnityEngine;

public interface IReligion
{
    string Name { get; }

    bool IsAi { get;  }

    Color Color { get; }

    ITactic Tactic { get; }

    float Faith { get; }

    Preacher AddPreacher(Vector2 position);
}
