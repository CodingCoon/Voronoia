using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Exercises the existing Assembly-CSharp code without moving the entire project into an asmdef.
public static class Increment01Verification
{
    private static int checks;
    private static readonly List<Voronation> created = new List<Voronation>();

    public static void PrepareAndRun()
    {
        Increment01Assets.Apply();
        Increment01Build.ReserializeDependencyMetadata();
        RunEditMode();
    }

    [MenuItem("Voronation/Increment 01/Verify EditMode integration")]
    public static void RunEditMode()
    {
        checks = 0;
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        Game game = UnityEngine.Object.FindAnyObjectByType<Game>(FindObjectsInactive.Include);
        try
        {
            VerifyGeometry();
            VerifyKnightsAndAccounting(game);
            VerifyResults(game);
            VerifyReferences(game);
            Directory.CreateDirectory("Build/Increment01");
            File.WriteAllText("Build/Increment01/editmode-integration.txt", "PASS: " + checks + " assertions; Unity " + Application.unityVersion);
            Debug.Log("Increment01 EditMode integration PASS: " + checks + " assertions");
        }
        finally
        {
            foreach (Voronation nation in created)
            {
                if (nation == null) continue;
                Invoke(game, "RemoveReligion", nation);
                UnityEngine.Object.DestroyImmediate(nation.gameObject);
            }
            created.Clear();
            Game.INSTANCE = null;
        }
    }

    private static Voronation Nation(Game game, float money, bool ai = false)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Voronation.prefab");
        var nation = ((GameObject)PrefabUtility.InstantiatePrefab(prefab)).GetComponent<Voronation>();
        nation.Setup(ai ? "AI" : "Human", Color.green, ai ? new DoNothingTactic(nation) : null, money);
        game.AddReligion(nation);
        created.Add(nation);
        return nation;
    }

    private static void VerifyKnightsAndAccounting(Game game)
    {
        Voronation nation = Nation(game, 100);
        var knights = new List<Leader>();
        for (int i = 1; i <= 20; i++)
        {
            Leader leader = nation.AddPreacher(new Vector2(i * 0.1f, 0));
            knights.Add(leader);
            if (i == 5 || i == 6 || i == 10 || i == 20)
            {
                Check(leader.Number == i, "Stable knight ID " + i);
                Check(leader.GetComponentsInChildren<TextMeshPro>(true).Single(t => t.name == "KnightNumber").text == i.ToString(), "TMP number " + i);
            }
        }

        Leader first = knights[0];
        var area = first.GetComponentInChildren<PreacherArea>(true);
        Check(first.GetArea() == 0, "Uninitialized area");
        var square = new List<Vector3> { new Vector3(10, 10), new Vector3(14, 10), new Vector3(14, 14), new Vector3(10, 14) };
        first.UpdateVoronoi(square);
        Check(Mathf.Approximately(first.GetArea(), 16), "Translated square area");
        var collider = area.GetComponent<PolygonCollider2D>();
        Check(collider.points.Distinct().Count() == 4, "Collider populated with vertices");
        Physics2D.SyncTransforms();
        Vector2 closest = area.ClosestPoint(new Vector2(20, 12));
        Check(Vector2.Distance(closest, new Vector2(14, 12)) < 0.001f,
            "Collider clamps world target: got " + closest + "; bounds " + collider.bounds + "; active " + collider.isActiveAndEnabled);
        square.Reverse();
        first.UpdateVoronoi(square);
        Check(Mathf.Approximately(first.GetArea(), 16), "Reverse winding area");

        Check(new ImprovePowerAction(first).GetDetailedInfos().Contains(first.Power.ToString()), "Power tooltip uses power");
        Check(nation.AddPreacher(first.GetPosition()).Number == 21, "Knight IDs remain monotonic");

        // Failed calculation must not change already displayed cells.
        var controller = UnityEngine.Object.FindAnyObjectByType<VoronoiController>(FindObjectsInactive.Include);
        var before = first.GetArea();
        Throws(() => controller.Recalculate(), "Invalid calculation rejected");
        Check(first.GetArea() == before, "Failed calculation preserves displayed area");

        foreach (Voronation existing in game.GetVoronations()) Invoke(game, "RemoveReligion", existing);
    }

    private static void VerifyResults(Game game)
    {
        Voronation human = Nation(game, 0);
        Voronation ai = Nation(game, 0, true);
        human.AddPreacher(Vector2.zero);
        ai.AddPreacher(Vector2.one);
        Invoke(game, "UpdateGameState");
        Check(game.State == Game.GameState.RUNNING, "Both factions active");
        Invoke(game, "RemoveReligion", human);
        Invoke(game, "UpdateGameState");
        Check(game.State == Game.GameState.PLAYER_LOOSE, "First faction removal is defeat");
        Check(Invoke(game, "GetHumanPlayer") == null, "AI cannot become human by index");
        Invoke(game, "RemoveReligion", ai);
        Invoke(game, "UpdateGameState");
        Check(game.State == Game.GameState.DRAW, "No factions is draw");
        game.AddReligion(human);
        Invoke(game, "UpdateGameState");
        Check(game.State == Game.GameState.PLAYER_WON, "Human alone wins");
        Invoke(game, "RemoveReligion", human);
    }

    private static void VerifyGeometry()
    {
        var calculator = UnityEngine.Object.FindAnyObjectByType<VoronoiCalculator>(FindObjectsInactive.Include);
        foreach (var positions in new[]
        {
            new[] { Vector2.zero },
            new[] { new Vector2(-2, -2), new Vector2(2, 2) },
            new[] { new Vector2(-10, -10), new Vector2(10, 10) },
            new[] { new Vector2(-4, 0), Vector2.zero, new Vector2(4, 0) },
            Enumerable.Range(0, 6).Select(i => new Vector2(Mathf.Cos(i * Mathf.PI / 3), Mathf.Sin(i * Mathf.PI / 3)) * 4).ToArray()
        })
        {
            var owners = positions.Select(p => (IVoronoiCellOwner)new Site(p)).ToList();
            var cells = calculator.CreateCells(owners);
            Check(cells.Count == positions.Length, "One cell per site");
            foreach (var cell in cells) Check(PolygonGeometry.Validate(cell.Points).Length >= 3, "Usable cell");
        }
        Throws(() => calculator.CreateCells(new List<IVoronoiCellOwner> { new Site(Vector2.zero), new Site(Vector2.zero) }), "Coincident sites rejected");
        Throws(() => calculator.CreateCells(new List<IVoronoiCellOwner> { new Site(new Vector2(float.NaN, 0)) }), "NaN site rejected");
        var line = new CCTouchLine(null, new Vector2(-1, 0), new Vector2(1, 0));
        Check(line.CalcIntersection(new CCPlane(Vector2.up, Vector2.up)) == null, "Parallel plane has no intersection");
    }

    private static void VerifyReferences(Game game)
    {
        foreach (var target in new UnityEngine.Object[] { game,
            UnityEngine.Object.FindAnyObjectByType<MatchStatusPanel>(FindObjectsInactive.Include),
            UnityEngine.Object.FindAnyObjectByType<EvaluatePhase>(FindObjectsInactive.Include),
            UnityEngine.Object.FindAnyObjectByType<DeathPhase>(FindObjectsInactive.Include),
            UnityEngine.Object.FindAnyObjectByType<StartPhase>(FindObjectsInactive.Include) })
        {
            Check(target != null, "Scene component exists");
            var property = new SerializedObject(target).GetIterator();
            while (property.NextVisible(true))
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                    Check(property.objectReferenceValue != null, target.name + "." + property.name + " assigned");
        }
    }

    internal static void Check(bool condition, string message)
    {
        checks++;
        if (!condition) throw new InvalidOperationException("Increment01 assertion: " + message);
    }
    private static void Throws(Action action, string message)
    {
        bool failed = false;
        try { action(); } catch (Exception) { failed = true; }
        Check(failed, message);
    }
    internal static object Invoke(object target, string method, params object[] args) =>
        target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Invoke(target, args);
    private class Site : IVoronoiCellOwner
    {
        private readonly Vector2 position;
        public float Power => 1;
        public Site(Vector2 position) { this.position = position; }
        public Vector2 GetPosition() => position;
        public void UpdateVoronoi(List<Vector3> points) { }
    }
}
