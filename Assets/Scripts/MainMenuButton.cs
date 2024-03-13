using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SpriteShapeController shape;
    [SerializeField] private SpriteShapeRenderer shapeRenderer;
    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private Color areaColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color borderColor;

    public bool Interactable { get; private set; } = true;

    void Start()
    {
        lineRenderer.positionCount = shape.spline.GetPointCount();
        Vector2[] points = new Vector2[shape.spline.GetPointCount()];
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            lineRenderer.SetPosition(i, shape.spline.GetPosition(i));
            points[i] = shape.spline.GetPosition(i);
        }

        collider.points = points;
        lineRenderer.startColor = borderColor;
        lineRenderer.endColor = borderColor;
        shapeRenderer.color = areaColor;
        lineRenderer.enabled = false;
    }


    private void OnMouseEnter()
    {
        if (!Interactable) return;
        lineRenderer.enabled = true;
    }

    private void OnMouseExit()
    {
        lineRenderer.enabled = false;
    }

    private void OnMouseUpAsButton()
    {
        if (!Interactable) return;
        shapeRenderer.color = areaColor;
        onClick.Invoke();
    }

    private void OnMouseUp()
    {
        if (!Interactable) return;
        shapeRenderer.color = areaColor;
    }

    private void OnMouseDown()
    {
        if (!Interactable) return;

        shapeRenderer.color = Color.black;
    }

    public void SetInteractable(bool interactable)
    {
        this.Interactable = interactable;
        shapeRenderer.color = interactable ? areaColor : disabledColor;
    }
}
