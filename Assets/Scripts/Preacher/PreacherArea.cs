using UnityEngine;
using UnityEngine.U2D;

public class PreacherArea : MonoBehaviour
{
    [SerializeField] private new PolygonCollider2D collider;
    [SerializeField] private LineRenderer influenceBounds;
    [SerializeField] private SpriteShapeRenderer areaRenderer;
    [SerializeField] private SpriteShapeController areaController;
    [SerializeField] private PreacherKnob knob;

    private IVoronation religion;
    private Vector3[] points = System.Array.Empty<Vector3>();
    public bool HasArea => points.Length >= 3;
    internal Vector3[] CopyBounds() => (Vector3[])points.Clone();

    private void Awake()
    {
        areaController.spline.Clear();
        collider.enabled = false;
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
        Vector3[] validated = PolygonGeometry.Validate(positions);
        Vector2[] colliderPositions = new Vector2[validated.Length];
        Vector3[] splinePositions = new Vector3[validated.Length];
        for (int i = 0; i < validated.Length; i++)
        {
            colliderPositions[i] = collider.transform.InverseTransformPoint(validated[i]);
            splinePositions[i] = areaController.transform.InverseTransformPoint(validated[i]);
        }
        PolygonGeometry.Validate(splinePositions);
        areaController.spline.Clear();
        collider.enabled = false;
        collider.pathCount = 0;
        for (int i = 0; i < splinePositions.Length; i++)
        {
            areaController.spline.InsertPointAt(i, splinePositions[i]);
            areaController.spline.SetHeight(i, 0.01f);
        }

        influenceBounds.positionCount = validated.Length;
        influenceBounds.SetPositions(validated);
        if (validated.Length > 0)
        {
            collider.pathCount = 1;
            collider.SetPath(0, colliderPositions);
            collider.enabled = true;
        }
        points = validated;
    }

    internal float GetArea()
    {
        return PolygonGeometry.Area(points);
    }

    public Vector2 ClosestPoint(Vector2 pos)
    {
        if (!HasArea) throw new System.InvalidOperationException("No valid area for target selection.");
        // Physics skin can put Collider2D.ClosestPoint outside the authored polygon.
        return PolygonGeometry.ClosestPoint(points, pos);
    }

    public Vector2 ClosestPointInside(Vector2 position, Vector2 interiorReference, float boundaryMargin)
    {
        if (!HasArea) throw new System.InvalidOperationException("No valid area for target selection.");
        return PolygonGeometry.ClosestPointInside(points, position, interiorReference, boundaryMargin);
    }

    internal void Dissolve(float progress)
    {
        influenceBounds.startColor = Color.Lerp(Color.black, Color.clear, progress);
        influenceBounds.endColor = Color.Lerp(Color.black, Color.clear, progress);
        areaRenderer.color = Color.Lerp(religion.Color, Color.clear, progress);
    }
}
