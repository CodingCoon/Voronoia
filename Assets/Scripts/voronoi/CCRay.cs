using UnityEngine;

public class CCRay
{
    public readonly Vector2 pos;

    public readonly Vector2 direction;


    public CCRay(Vector2 pos, Vector2 direction)
    {
        this.pos = pos;

        this.direction = direction;
    }
}