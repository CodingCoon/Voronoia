using UnityEngine;
using UnityEngine.U2D;

public class PreacherArea : MonoBehaviour, IMouseListener
{
    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private LineRenderer influenceBounds;
    [SerializeField] private SpriteShapeRenderer areaRenderer;
    [SerializeField] private SpriteShapeController areaController;
    [SerializeField] private PreacherKnob knob;

    private IReligion religion;
    private Vector3[] points; 

    public void Setup(IReligion religion)
    {
        this.religion = religion;
        influenceBounds.startColor = Color.black;
        influenceBounds.endColor = Color.black;
        areaRenderer.color = religion.Color;
    }

    public void SetBounds(Vector3[] positions)
    {
        points = positions;
        influenceBounds.positionCount = positions.Length;
        influenceBounds.SetPositions(positions);

        areaController.spline.Clear();

        Vector2[] colliderPositions = new Vector2[positions.Length];
        for (int i = 0; i < positions.Length; i++)
        {
            areaController.spline.InsertPointAt(i, positions[i]);
            areaController.spline.SetHeight(i, 0.01f);
        }

        collider.points = colliderPositions;
    }

    internal float GetArea()
    {
        var result = 0f;
        for (int p = points.Length - 1, q = 0; q < points.Length; p = q++)
        {
            result += (Vector3.Cross(points[q], points[p])).magnitude;
        }
        return result * .5f;
    }

    public void OnHover(bool hovered)
    {
        if (hovered)
        {
            influenceBounds.startColor = Color.white;
            influenceBounds.endColor = Color.white;
            influenceBounds.sortingOrder = 2;
        }
        else
        {
            influenceBounds.startColor = Color.black;
            influenceBounds.endColor = Color.black;
            influenceBounds.sortingOrder = 1;
        }
    }
}
