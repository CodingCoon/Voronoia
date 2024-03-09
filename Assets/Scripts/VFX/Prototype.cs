using System;
using UnityEngine;

// A wrapper class, that connects inspector and source code by using a generic and Instantiate prefabs of
// a defined type T.  
[Serializable]
public class Prototype<T> where T : Component
{
    [SerializeField] private T prefab;

    public T Create(Transform parent, Quaternion rotation, Vector3 initialPosition)
    {
        return GameObject.Instantiate(prefab, initialPosition, rotation, parent);
    }

    public T Create(Transform parent, Vector3 initialPosition)
    {
        return GameObject.Instantiate(prefab, initialPosition, Quaternion.identity, parent);
    }

    public T Create(Transform parent)
    {
        return GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
    }

    public T Create()
    {
        return GameObject.Instantiate(prefab);
    }

    public bool IsFilled()
    {
        return prefab != null;
    }
}
