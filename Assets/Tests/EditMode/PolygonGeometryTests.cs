using System;
using NUnit.Framework;
using UnityEngine;

public class PolygonGeometryTests
{
    [TestCase(0, 0)]
    [TestCase(100, -70)]
    [TestCase(-10000, 10000)]
    public void SquareAreaIsIndependentOfTranslationAndWinding(float x, float y)
    {
        var points = new[] { new Vector3(x, y), new Vector3(x + 4, y), new Vector3(x + 4, y + 4), new Vector3(x, y + 4) };
        Assert.AreEqual(16, PolygonGeometry.Area(points), 0.0001);
        Array.Reverse(points);
        Assert.AreEqual(16, PolygonGeometry.Area(points), 0.0001);
        Assert.AreEqual(4, PolygonGeometry.Validate(points).Length);
    }

    [Test]
    public void UninitializedAndEmptyPresentationHaveZeroArea()
    {
        Assert.AreEqual(0, PolygonGeometry.Area(null));
        Assert.AreEqual(0, PolygonGeometry.Area(Array.Empty<Vector3>()));
        Assert.IsEmpty(PolygonGeometry.Validate(null));
    }

    [Test]
    public void InvalidCoordinatesAreRejectedBeforePresentation()
    {
        foreach (float invalid in new[] { float.NaN, float.PositiveInfinity, float.NegativeInfinity })
            Assert.Throws<ArgumentException>(() => PolygonGeometry.Validate(new[] { Vector3.zero, Vector3.right, new Vector3(0, invalid) }));
    }

    [Test]
    public void DegenerateOrSelfIntersectingCellsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => PolygonGeometry.Validate(new[] { Vector3.zero, Vector3.right }));
        Assert.Throws<ArgumentException>(() => PolygonGeometry.Validate(new[] { Vector3.zero, Vector3.right, Vector3.right * 2 }));
        Assert.Throws<ArgumentException>(() => PolygonGeometry.Validate(new[] { Vector3.zero, new Vector3(3, 3), new Vector3(0, 3), new Vector3(3, 0) }));
        Assert.Throws<ArgumentException>(() => PolygonGeometry.Validate(new[] { Vector3.zero, new Vector3(0.001f, 0), Vector3.up }));
    }

    [Test]
    public void ConcavePolygonUsesSignedCrossProducts()
    {
        var points = new[] { new Vector3(10, 10), new Vector3(14, 10), new Vector3(14, 14), new Vector3(12, 12), new Vector3(10, 14) };
        Assert.AreEqual(12, PolygonGeometry.Area(PolygonGeometry.Validate(points)), 0.0001);
    }

    [Test]
    public void TargetClampingHasNoPhysicsSkinOutsideTheMap()
    {
        var points = new[] { new Vector3(-10, -10), new Vector3(10, -10), new Vector3(10, 10), new Vector3(-10, 10) };
        Assert.AreEqual(new Vector2(10, 5), PolygonGeometry.ClosestPoint(points, new Vector2(20, 5)));
        Assert.AreEqual(new Vector2(10, 10), PolygonGeometry.ClosestPoint(points, new Vector2(20, 20)));
        Assert.AreEqual(Vector2.one, PolygonGeometry.ClosestPoint(points, Vector2.one));
        Array.Reverse(points);
        Assert.AreEqual(new Vector2(10, 5), PolygonGeometry.ClosestPoint(points, new Vector2(20, 5)));
    }
}
