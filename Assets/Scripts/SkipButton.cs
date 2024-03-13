using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class SkipButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private SpriteShapeController shape;
    [SerializeField] private SpriteShapeRenderer shapeRenderer;
    [SerializeField] private new PolygonCollider2D collider;
    
    private Color startColor;

    void Start()
    {
        Vector2[] points = new Vector2[shape.spline.GetPointCount()];
        for (int i = 0; i < shape.spline.GetPointCount(); i++)
        {
            points[i] = shape.spline.GetPosition(i);
        }
        collider.points = points;
        startColor = shapeRenderer.color;
    }


    private void OnMouseEnter()
    {
        shapeRenderer.color = Color.white;
    }

    private void OnMouseExit()
    {
        shapeRenderer.color = startColor;
    }

    private void OnMouseUpAsButton()
    {
        onClick.Invoke();
    }
}
