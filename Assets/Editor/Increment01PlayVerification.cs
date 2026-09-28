using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class Increment01PlayVerification
{
    private const string RunningKey = "Increment01.PlayVerification";
    private static int stage;
    private static int lastFrame = -1;
    private static double deadline;
    private static float expectedMoney;
    private static bool sawApply, sawGeometry, sawEvaluation, sawDeath;
    private static string failure;

    static Increment01PlayVerification()
    {
        if (SessionState.GetBool(RunningKey, false))
        {
            EditorApplication.playModeStateChanged += StateChanged;
            EditorApplication.update += Tick;
            deadline = EditorApplication.timeSinceStartup + 150;
        }
    }

    [MenuItem("Voronation/Increment 01/Verify PlayMode integration")]
    public static void Run()
    {
        Directory.CreateDirectory("Build/Increment01");
        File.WriteAllText("Build/Increment01/playmode-integration.txt", "RUNNING");
        EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity");
        var manager = UnityEngine.Object.FindAnyObjectByType<GameManager>(FindObjectsInactive.Include);
        var serialized = new SerializedObject(manager);
        serialized.FindProperty("tutorialHook").boolValue = false;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        SessionState.SetBool(RunningKey, true);
        EditorApplication.EnterPlaymode();
    }

    private static void StateChanged(PlayModeStateChange change)
    {
        if (change != PlayModeStateChange.EnteredEditMode) return;
        SessionState.SetBool(RunningKey, false);
        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= StateChanged;
        Directory.CreateDirectory("Build/Increment01");
        string result = failure ?? SessionState.GetString("Increment01.PlayFailure", "");
        bool passed = SessionState.GetBool("Increment01.PlayPassed", false);
        File.WriteAllText("Build/Increment01/playmode-integration.txt",
            passed && string.IsNullOrEmpty(result) ? "PASS: automatic phases, repeated input and exactly-once resolved round." : "FAIL: " + result);
        if (Application.isBatchMode) EditorApplication.Exit(passed && string.IsNullOrEmpty(result) ? 0 : 1);
    }

    private static void Tick()
    {
        if (!EditorApplication.isPlaying) return;
        try
        {
            if (EditorApplication.timeSinceStartup > deadline) throw new Exception("PlayMode scenario timed out in stage " + stage);
            if (lastFrame == Time.frameCount) return;
            lastFrame = Time.frameCount;
            Game game = Game.INSTANCE;
            if (game == null) return;
            if (stage < 4 && game.IsFaulted) throw new Exception("Unexpected round failure in stage " + stage);
            switch (stage)
            {
                case 0:
                    if (!game.CanPlan) return;
                    SessionState.SetBool("Increment01.PlayPassed", false);
                    SessionState.SetString("Increment01.PlayFailure", "");
                    Check(game.GetPreachers().Count == 6, "Standard setup");
                    Directory.CreateDirectory("Build/Increment01");
                    CaptureView("match");
                    Leader human = game.GetVoronations().Single(n => n.IsPlayer).GetLeaders().Single();
                    human.SetAction(new ImprovePowerAction(human));
                    for (int i = 0; i < 20; i++) game.NextPhase();
                    Check(game.PhaseType == PhaseType.APPLY, "Repeated confirmation stays in apply");
                    stage = 1;
                    break;
                case 1:
                    sawApply |= game.PhaseType == PhaseType.APPLY;
                    sawGeometry |= game.PhaseType == PhaseType.VORONOI;
                    sawEvaluation |= game.PhaseType == PhaseType.EVALUATION;
                    sawDeath |= game.PhaseType == PhaseType.DEATH;
                    if (game.CanAdvance)
                    {
                        Check(sawApply && sawGeometry && sawEvaluation && sawDeath, "All automatic phases observed");
                        Voronation player = game.GetVoronations().Single(n => n.IsPlayer);
                        Leader knight = player.GetLeaders().Single();
                        expectedMoney = 100 + knight.GetArea() * knight.Income - 40 - 44;
                        Check(Mathf.Abs(player.Money - expectedMoney) < 0.01f, "Income and cost booked once");
                        Check(knight.RoundsExist == 1 && Mathf.Abs(knight.Power - 1.1f) < 0.001f, "Action applied once");
                        game.NextPhase();
                        Check(game.PhaseType == PhaseType.ACTION, "Ready death advances to planning");
                        stage = 2;
                    }
                    else
                    {
                        PhaseType before = game.PhaseType;
                        for (int i = 0; i < 20; i++) game.NextPhase();
                        Check(game.PhaseType == before, "Manual input cannot skip automatic phase");
                    }
                    break;
                case 2:
                    SessionState.SetBool("Increment01.PlayPassed", true);
                    Debug.Log("Increment01 PlayMode integration PASS");
                    EditorApplication.ExitPlaymode();
                    break;
            }
        }
        catch (Exception exception)
        {
            failure = exception.ToString();
            SessionState.SetString("Increment01.PlayFailure", failure);
            Debug.LogError("Increment01 PlayMode verification failed: " + failure);
            EditorApplication.ExitPlaymode();
        }
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void CaptureView(string name)
    {
        if (!SessionState.GetBool("Increment01.Capture", false)) return;
        Camera camera = Camera.main;
        var texture = new RenderTexture(1280, 720, 24);
        var previousTarget = camera.targetTexture;
        var previousActive = RenderTexture.active;
        Canvas overlay = UnityEngine.Object.FindObjectsByType<Canvas>()
            .First(c => c.name == "MatchStatus");
        var oldMode = overlay.renderMode;
        var oldCamera = overlay.worldCamera;
        float oldDistance = overlay.planeDistance;
        Texture2D image = null;
        try
        {
            overlay.renderMode = RenderMode.ScreenSpaceCamera;
            overlay.worldCamera = camera;
            overlay.planeDistance = camera.nearClipPlane + 0.1f;
            Canvas.ForceUpdateCanvases();
            camera.targetTexture = texture;
            camera.Render();
            RenderTexture.active = texture;
            image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
            image.Apply();
            File.WriteAllBytes("Build/Increment01/" + name + ".png", image.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = previousTarget;
            RenderTexture.active = previousActive;
            overlay.renderMode = oldMode;
            overlay.worldCamera = oldCamera;
            overlay.planeDistance = oldDistance;
            UnityEngine.Object.DestroyImmediate(image);
            UnityEngine.Object.DestroyImmediate(texture);
        }
    }
}
