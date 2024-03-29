﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VoronoiCalculator : MonoBehaviour
{
    [SerializeField] private Map map;

    private CCTouchLine topLine;
    private CCTouchLine rightLine;
    private CCTouchLine bottomLine;
    private CCTouchLine leftLine;

    private CCIntersection tlIntersection;
    private CCIntersection trIntersection;
    private CCIntersection brIntersection;
    private CCIntersection blIntersection;

    private List<CCTouchPoint> touchPoints = new List<CCTouchPoint>();
    private List<CCTouchLine> touchLines = new List<CCTouchLine>();
    private List<CCIntersection> allIntersections = new List<CCIntersection>();
    private List<CCIntersection> reducedIntersections = new List<CCIntersection>();

    private void Start()
    {
        Vector2 bl = new Vector2(-map.HalfMapSize, -map.HalfMapSize);
        Vector2 tl = new Vector2(-map.HalfMapSize, map.HalfMapSize);
        Vector2 tr = new Vector2(map.HalfMapSize, map.HalfMapSize);
        Vector2 br = new Vector2(map.HalfMapSize, -map.HalfMapSize);

        topLine = new CCTouchLine(null, tl, tr);
        rightLine = new CCTouchLine(null, tr, br);
        bottomLine = new CCTouchLine(null, br, bl);
        leftLine = new CCTouchLine(null, bl, tl);

        tlIntersection = new CCIntersection(topLine, leftLine, tl);
        trIntersection = new CCIntersection(topLine, rightLine, tr);
        brIntersection = new CCIntersection(bottomLine, rightLine, br);
        blIntersection = new CCIntersection(bottomLine, leftLine, bl);
    }

    IVoronoiCellOwner first;

    public List<CCVoronoiCell> CreateCells(List<IVoronoiCellOwner> sites)
    {
        this.touchPoints.Clear();
        this.touchLines.Clear();
        this.allIntersections.Clear();
        this.reducedIntersections.Clear();

        if (sites.Count == 0) return new List<CCVoronoiCell>();
        if (sites.Count == 1) return new List<CCVoronoiCell>() { CreateFullCell(sites[0]) };

        List<CCVoronoiCell> voronoiCells = new List<CCVoronoiCell> ();
        first = sites[0];

        // 1. Bilde Punkte zwischen allen Preachern
        List<CCTouchPoint> tmpTouchPoints = new List<CCTouchPoint>();
        Dictionary<IVoronoiCellOwner, List<CCTouchPoint>> siteToTouchPoints = new Dictionary<IVoronoiCellOwner, List<CCTouchPoint>>();
        SetupTouchPoints(sites, tmpTouchPoints, siteToTouchPoints);
        this.touchPoints.AddRange(tmpTouchPoints);

        // 2. Bilde Linien, aus den TouchPoints, die von Rand zu Rand gehen 
        Dictionary<IVoronoiCellOwner, List<CCTouchLine>> siteToTouchLines = new Dictionary<IVoronoiCellOwner, List<CCTouchLine>>();
        List<CCTouchLine> tmpTouchLines = new List<CCTouchLine>();
        SetupTouchLines(tmpTouchPoints, tmpTouchLines, siteToTouchLines);         
        touchLines.AddRange(tmpTouchLines);

        // Nun bauen wir die Zellen zusammen
        foreach (IVoronoiCellOwner site in sites)
        {
            // 3. Bilde die Schnittpunkte aus allen TouchLines und BorderLines
            List<CCTouchLine> touchLines = siteToTouchLines[site];
            List<CCIntersection> tmpIntersections = new List<CCIntersection>();
            SetupIntersections(touchLines, tmpIntersections);
            allIntersections.AddRange(tmpIntersections);
            //print("--- " + site + " Intersections");
            //foreach (var item in tmpIntersections) print("\t\t  " + item.Point);

            // 4. Entferne alle Schnittpunkte die hinter anderen TouchLines liegen
            CleanupIntersections(site, touchLines, tmpIntersections);
            reducedIntersections.AddRange(tmpIntersections);
            //print("--- " + site + " after cleanup");
            //foreach (var item in tmpIntersections) print("\t\t  " + item.Point);

            // 5. Entferne Duplikate        
            List<Vector2> points = new List<Vector2>();
            tmpIntersections.ForEach(intersection => 
            {
                bool distinct = true;
                foreach (Vector2 point in points)
                {
//                    print(intersection.Point + " X  " + point + " = " + Vector2.Distance(intersection.Point, point) + " (" + (Vector2.Distance(intersection.Point, point) == 0)  + ")");
                    if (Vector2.Distance(intersection.Point, point) < 0.01f ) //< 0.1f)
                    {
                        distinct = false;
                        break;
                    }
                }
                if (distinct) points.Add(intersection.Point);
            });
            List<Vector2> distinctPoints = points.Distinct().ToList();

            // 6. Sortiere die Punkte nach ihrem Winkel 
            List<CCVoronoiCellPoint> cellPoints = new List<CCVoronoiCellPoint>();
            distinctPoints.ForEach(point =>
            {
                float angle = Mathf.Atan2(point.y - site.GetPosition().y,
                                          point.x - site.GetPosition().x);
                CCVoronoiCellPoint cellPoint = new CCVoronoiCellPoint(point, angle);
                cellPoints.Add(cellPoint);
            });
            
            cellPoints.Sort();
            //print("--- " + site + " after sort");
            //foreach (var item in cellPoints) print("\t\t  " + item.Point);

            // 7. Bilde die VoronoiZelle
            voronoiCells.Add(new CCVoronoiCell(site, cellPoints));
        }

        return voronoiCells;
    }

    // Entfernt alle Schnittpunkte, die nicht relevant sind, weil sie hinter einer touchLine liegen
    private void CleanupIntersections(IVoronoiCellOwner site, List<CCTouchLine> touchLines, List<CCIntersection> tmpIntersections)
    {
        List<CCIntersection> workingCopy = new List<CCIntersection>(tmpIntersections);

        foreach (CCIntersection intersection in workingCopy)
        {
            // Bilde eine Linie von Hauptpunkt zum Schnittpunkt
            CCTouchLine siteToIntersection = new CCTouchLine(null, site.GetPosition(), intersection.Point);

            // Wenn ich eine Linie finde, die nicht die zur Intersection beitragende Linie ist und die ich schneide
            // von Site zur Intersection, dann liegt diese Intersection hinter relevanten Linien und kann verworfen werden
            // Aber achtung: Ein Schnittpunkt kann der Schnittpunkt verschiedener Linien sein
            foreach (CCTouchLine line in touchLines)
            {
                Vector2? point = siteToIntersection.CalcInnerIntersection(line);
                //if (point == null) print("\t\t Hit erst dahinter, kann bleiben");
                //if (point != null && point == intersection.Point) print("\t\t Hit an der selben Stelle, also okay");
                if (point != null && point != intersection.Point)
                {
                    tmpIntersections.Remove(intersection);
                }
                //if (intersection.FirstLine == line || intersection.SecondLine == line) continue;
                //if (line.IsInnerIntersection(siteToIntersection)) tmpIntersections.Remove(intersection);
            }

            if (intersection.FirstLine != topLine && intersection.SecondLine != topLine && 
                topLine.IsInnerIntersection(siteToIntersection)) tmpIntersections.Remove(intersection);
            if (intersection.FirstLine != rightLine && intersection.SecondLine != rightLine &&
                rightLine.IsInnerIntersection(siteToIntersection)) tmpIntersections.Remove(intersection);
            if (intersection.FirstLine != bottomLine && intersection.SecondLine != bottomLine &&
                bottomLine.IsInnerIntersection(siteToIntersection)) tmpIntersections.Remove(intersection);
            if (intersection.FirstLine != leftLine && intersection.SecondLine != leftLine &&
                leftLine.IsInnerIntersection(siteToIntersection)) tmpIntersections.Remove(intersection);
        }
    }

    private void SetupIntersections(List<CCTouchLine> touchLines, List<CCIntersection> intersections)
    {
        HashSet<Vector2> intersectionPoints = new HashSet<Vector2> ();  // todo duplikate entfernen

        for (int i = 0; i < touchLines.Count; i++)
        {
            for (int j = i + 1; j < touchLines.Count; j++)
            {
                CheckIntersection(intersections, touchLines[i], touchLines[j]);
            }

            CheckIntersection(intersections, touchLines[i], topLine);
            CheckIntersection(intersections, touchLines[i], rightLine);
            CheckIntersection(intersections, touchLines[i], bottomLine);
            CheckIntersection(intersections, touchLines[i], leftLine);

            intersections.Add(brIntersection);
            intersections.Add(blIntersection);
            intersections.Add(trIntersection);
            intersections.Add(tlIntersection);
        }
    }

    private void CheckIntersection(List<CCIntersection> intersections, CCTouchLine first, CCTouchLine second)
    {
        Vector2? point = first.CalcIntersection(second);

        if (point != null)
        {
            var intersection = new CCIntersection(first, second, (Vector2) point);
            intersections.Add(intersection);
        }
    }

    private void SetupTouchPoints(List<IVoronoiCellOwner> cellOwners, 
                                  List<CCTouchPoint> allTouchPoints, 
                                  Dictionary<IVoronoiCellOwner, List<CCTouchPoint>> siteToTouchPoints)
    {
        for (int i = 0; i < cellOwners.Count; i++)
        {
            for (int j = i+1; j < cellOwners.Count; j++)
            {
                IVoronoiCellOwner first  = cellOwners[i];
                IVoronoiCellOwner second = cellOwners[j];
                CCTouchPoint touchPoint  = new CCTouchPoint(first, second);

                siteToTouchPoints.AddIfAbsent(first, touchPoint);
                siteToTouchPoints.AddIfAbsent(second, touchPoint);
                allTouchPoints.Add(touchPoint);
            }
        }
    }

    private void SetupTouchLines(List<CCTouchPoint> touchPoints,
                                  List<CCTouchLine> tmpTouchLines, 
                                  Dictionary<IVoronoiCellOwner, List<CCTouchLine>> siteToTouchLines)
    {


        List<Vector2> borderTouches = new List<Vector2>();

        foreach (CCTouchPoint point in touchPoints)
        {
            borderTouches.Clear();

            CheckBorderLine(topLine, point, borderTouches);
            CheckBorderLine(rightLine, point, borderTouches);
            CheckBorderLine(bottomLine, point, borderTouches);
            CheckBorderLine(leftLine, point, borderTouches);

            if (borderTouches.Count == 2)
            {
                CCTouchLine line = new CCTouchLine(point, borderTouches[0], borderTouches[1]);
                tmpTouchLines.Add(line);
                siteToTouchLines.AddIfAbsent(point.First, line);
                siteToTouchLines.AddIfAbsent(point.Second, line);
            }
            else 
            {
                print("------------- " + borderTouches.Count);
                print(topLine);
                print(rightLine);
                print(bottomLine);
                print(leftLine);
                print(point);
                throw new Exception("snh: ");
            }
        }
    }

 
    private void CheckBorderLine(CCTouchLine border, CCTouchPoint touchPoint, List<Vector2> borderIntersections)
    {
        Vector2? intersectionWithBorder = border.CalcIntersection(touchPoint.Plane);
        //print("Intersects at " + intersectionWithBorder + " " +
            //((intersectionWithBorder != null) ? border.IsOnLine((Vector2)intersectionWithBorder) : " "));
        if (intersectionWithBorder != null && border.IsOnLine((Vector2)intersectionWithBorder)) {
            borderIntersections.Add((Vector2) intersectionWithBorder);
        }
    }

    private CCVoronoiCell CreateFullCell(IVoronoiCellOwner owner)
    {
        Vector2 bl = new Vector2(-map.HalfMapSize, -map.HalfMapSize);
        Vector2 tl = new Vector2(-map.HalfMapSize,  map.HalfMapSize);
        Vector2 tr = new Vector2( map.HalfMapSize,  map.HalfMapSize);
        Vector2 br = new Vector2( map.HalfMapSize, -map.HalfMapSize);
        return new CCVoronoiCell(owner, new List<Vector2>() { bl, tl, tr, br });
    }

    //private void OnDrawGizmos()
    //{
        //Vector2 bl = new Vector2(-map.HalfMapSize, -map.HalfMapSize);
        //Vector2 tl = new Vector2(-map.HalfMapSize, map.HalfMapSize);
        //Vector2 tr = new Vector2(map.HalfMapSize, map.HalfMapSize);
        //Vector2 br = new Vector2(map.HalfMapSize, -map.HalfMapSize);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(tl, tr);
        //Gizmos.DrawLine(tr, br);
        //Gizmos.DrawLine(br, bl);
        //Gizmos.DrawLine(bl, tl);

        //Gizmos.color = Color.green;
        //foreach (CCTouchPoint point in touchPoints)
        //{
        //    if (point.First == first || point.Second == first)
        //    {
        //        Gizmos.DrawSphere(point.Center, 0.1f);
        //    }
        //}

        //Gizmos.color = Color.black;
        //foreach (CCTouchLine line in touchLines)
        //{
        //    if (line.Owner.First == first || line.Owner.Second == first)
        //    {
        //        Gizmos.DrawLine(line.FirstPoint, line.SecondPoint);
        //    }
        //}

        //Gizmos.color = Color.cyan;
        //foreach (CCIntersection point in allIntersections)
        //{
        //    if (point.BelongsTo(first))
        //    { 
        //        Gizmos.DrawSphere(point.Point, 0.15f);
        //    }
        //}

        //Gizmos.color = Color.yellow;
        //foreach (CCIntersection point in reducedIntersections)
        //{
        //    // Gizmos.DrawSphere(point.Point, 0.1f);
        //}
    //}

}
