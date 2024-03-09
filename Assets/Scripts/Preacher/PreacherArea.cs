
using System;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class PreacherArea : MonoBehaviour, IMouseListener
{
    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private LineRenderer influenceBounds;
    [SerializeField] private SpriteShapeRenderer areaRenderer;
    [SerializeField] private SpriteShapeController areaController;
    [SerializeField] private PreacherKnob knob;

    private IVoronation religion;
    private Vector3[] points;

    private bool dissolving = false;

    private void Awake()
    {
        areaController.spline.Clear();
    }


    public void Setup(IVoronation religion)
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
        if (dissolving) return;
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

    public Vector2 ClosestPoint(Vector2 pos)
    {
        return collider.ClosestPoint(pos);
    }

    internal void Dissolve(float progress)
    {
        dissolving = true;
        influenceBounds.startColor = Color.Lerp(Color.black, Color.clear, progress);
        influenceBounds.endColor = Color.Lerp(Color.black, Color.clear, progress);
        areaRenderer.color = Color.Lerp(religion.Color, Color.clear, progress);
    }
}
