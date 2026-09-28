using System;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class Increment01Assets
{
    [MenuItem("Voronation/Increment 01/Apply authored asset changes")]
    public static void Apply()
    {
        UpgradeLeader();
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/GameScene.unity", OpenSceneMode.Single);
        Game game = UnityEngine.Object.FindAnyObjectByType<Game>(FindObjectsInactive.Include);
        var blend = UnityEngine.Object.FindAnyObjectByType<ScreenBlend>(FindObjectsInactive.Include);
        if (blend == null) throw new InvalidOperationException("GameScene requires its authored ScreenBlend.");
        blend.gameObject.SetActive(true);
        PrefabUtility.RecordPrefabInstancePropertyModifications(blend.gameObject);
        var panel = UnityEngine.Object.FindAnyObjectByType<MatchStatusPanel>(FindObjectsInactive.Include);
        if (panel == null) panel = CreatePanel();
        panel.GetComponent<Canvas>().sortingLayerName = "UI";
        Assign(game, "statusPanel", panel);
        Assign(game, "selection", UnityEngine.Object.FindAnyObjectByType<LeaderSelectionManager>(FindObjectsInactive.Include));
        Assign(game, "plannedActions", UnityEngine.Object.FindAnyObjectByType<PlannedActionController>(FindObjectsInactive.Include));
        var evaluation = UnityEngine.Object.FindAnyObjectByType<EvaluationPanel>(FindObjectsInactive.Include);
        var voronoi = UnityEngine.Object.FindAnyObjectByType<VoronoiController>(FindObjectsInactive.Include);
        Assign(UnityEngine.Object.FindAnyObjectByType<StartPhase>(FindObjectsInactive.Include), "voronoi", voronoi);
        Assign(UnityEngine.Object.FindAnyObjectByType<DeathPhase>(FindObjectsInactive.Include), "voronoi", voronoi);
        Assign(UnityEngine.Object.FindAnyObjectByType<DeathPhase>(FindObjectsInactive.Include), "evaluationPanel", evaluation);
        Assign(UnityEngine.Object.FindAnyObjectByType<EvaluatePhase>(FindObjectsInactive.Include), "evaluationPanel", evaluation);
        Assign(UnityEngine.Object.FindAnyObjectByType<ActionPhase>(FindObjectsInactive.Include), "evaluationPanel", evaluation);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Increment01 authored assets saved.");
    }

    private static void UpgradeLeader()
    {
        const string path = "Assets/Prefabs/Leader.prefab";
        GameObject root = PrefabUtility.LoadPrefabContents(path);
        try
        {
            Leader leader = root.GetComponent<Leader>();
            PreacherKnob knob = root.GetComponentInChildren<PreacherKnob>(true);
            // PreacherArea owns the exact collider, not SpriteShape's generated outline.
            root.GetComponentInChildren<UnityEngine.U2D.SpriteShapeController>(true).autoUpdateCollider = false;
            TextMeshPro label = root.GetComponentsInChildren<TextMeshPro>(true).FirstOrDefault(t => t.name == "KnightNumber");
            if (label == null)
            {
                Transform old = root.GetComponentsInChildren<Transform>(true).First(t => t.name == "NumberSprite");
                SpriteRenderer renderer = old.GetComponent<SpriteRenderer>();
                var number = new GameObject("KnightNumber", typeof(RectTransform), typeof(TextMeshPro));
                number.transform.SetParent(old.parent, false);
                number.transform.localPosition = old.localPosition;
                label = number.GetComponent<TextMeshPro>();
                TextMeshPro income = root.GetComponentsInChildren<TextMeshPro>(true).First(t => t.name == "Income");
                label.font = income.font;
                label.fontSharedMaterial = income.fontSharedMaterial;
                label.text = "20";
                label.color = Color.black;
                label.alignment = TextAlignmentOptions.Center;
                label.fontSize = 14;
                label.enableAutoSizing = true;
                label.fontSizeMin = 4;
                label.fontSizeMax = 14;
                label.textWrappingMode = TextWrappingModes.NoWrap;
                label.rectTransform.sizeDelta = new Vector2(0.72f, 0.65f);
                label.GetComponent<MeshRenderer>().sortingLayerID = renderer.sortingLayerID;
                label.GetComponent<MeshRenderer>().sortingOrder = renderer.sortingOrder;
                UnityEngine.Object.DestroyImmediate(old.gameObject);
            }
            Assign(leader, "numberLabel", label);
            label.fontSize = 14;
            label.fontSizeMin = 4;
            label.fontSizeMax = 14;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            Assign(knob, "numberLabel", label);
            PrefabUtility.SaveAsPrefabAsset(root, path);
        }
        finally { PrefabUtility.UnloadPrefabContents(root); }
    }

    private static MatchStatusPanel CreatePanel()
    {
        var canvasObject = new GameObject("MatchStatus", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight = 0.5f;
        MatchStatusPanel panel = canvasObject.AddComponent<MatchStatusPanel>();

        var overlay = new GameObject("ResultOverlay", typeof(RectTransform), typeof(Image));
        overlay.transform.SetParent(canvasObject.transform, false);
        RectTransform overlayRect = overlay.GetComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.offsetMin = overlayRect.offsetMax = Vector2.zero;
        overlay.GetComponent<Image>().color = new Color(0.06f, 0.08f, 0.13f, 0.96f);
        var title = Label(overlay.transform, "Title", new Vector2(0, 110), new Vector2(900, 90), 52);
        title.text = "Unentschieden";
        var description = Label(overlay.transform, "Description", new Vector2(0, 10), new Vector2(880, 120), 26);
        description.text = "Alle Fraktionen sind gleichzeitig ausgeschieden.";
        Button restart = Button(overlay.transform, "Restart", "Neue Partie", new Vector2(-160, -130));
        Button menu = Button(overlay.transform, "Menu", "Menü", new Vector2(160, -130));
        Assign(panel, "root", overlay);
        Assign(panel, "title", title);
        Assign(panel, "description", description);
        Assign(panel, "restart", restart);
        Assign(panel, "menu", menu);
        return panel;
    }

    private static TextMeshProUGUI Label(Transform parent, string name, Vector2 position, Vector2 size, float fontSize)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var text = go.GetComponent<TextMeshProUGUI>();
        text.font = TMP_Settings.defaultFontAsset;
        if (text.font == null) throw new InvalidOperationException("Default TMP font must be authored.");
        text.fontSize = fontSize;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.rectTransform.anchoredPosition = position;
        text.rectTransform.sizeDelta = size;
        return text;
    }

    private static Button Button(Transform parent, string name, string caption, Vector2 position)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rect = go.GetComponent<RectTransform>();
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(280, 64);
        var image = go.GetComponent<Image>();
        image.color = new Color(0.23f, 0.48f, 0.43f);
        var button = go.GetComponent<Button>();
        button.targetGraphic = image;
        Label(go.transform, "Label", Vector2.zero, rect.sizeDelta, 25).text = caption;
        return button;
    }

    public static void Assign(UnityEngine.Object owner, string field, UnityEngine.Object value)
    {
        if (owner == null || value == null) throw new InvalidOperationException("Missing authored reference: " + field);
        var serialized = new SerializedObject(owner);
        var property = serialized.FindProperty(field);
        if (property == null) throw new InvalidOperationException("Missing serialized field: " + field);
        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
