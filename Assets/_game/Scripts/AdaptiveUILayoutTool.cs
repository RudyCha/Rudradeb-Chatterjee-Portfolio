using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class AdaptiveUILayoutTool : EditorWindow
{
    private static readonly Vector2[] testResolutions = new Vector2[]
    {
        new Vector2(1920, 1080), // 16:9 Full HD
        new Vector2(1280, 720),  // 16:9 HD
        new Vector2(1536, 2048), // iPad 4:3
        new Vector2(1080, 2340), // Tall mobile 19.5:9
        new Vector2(2560, 1080), // Ultrawide 21:9
    };

    private int selectedResIndex = 0;
    private Vector2 previewScroll;

    [MenuItem("Tools/UI/Adaptive UI Layout Tool")]
    public static void ShowWindow()
    {
        GetWindow<AdaptiveUILayoutTool>("Adaptive UI Layout Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Adaptive UI Layout Tool", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Auto-Anchor Selected UI Elements"))
        {
            AutoAnchorSelection();
        }

        GUILayout.Space(15);
        GUILayout.Label("Preview Resolutions", EditorStyles.boldLabel);

        selectedResIndex = EditorGUILayout.Popup("Test Resolution", selectedResIndex, new string[]
        {
            "1920x1080 (16:9 Full HD)",
            "1280x720 (16:9 HD)",
            "1536x2048 (4:3 iPad)",
            "1080x2340 (19.5:9 Mobile)",
            "2560x1080 (21:9 Ultrawide)"
        });

        if (GUILayout.Button("Apply Preview Resolution"))
        {
            ApplyResolution(testResolutions[selectedResIndex]);
        }
    }

    private void AutoAnchorSelection()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect == null) continue;

            Undo.RecordObject(rect, "Auto-Anchor");

            // Get parent canvas
            RectTransform parent = rect.parent as RectTransform;
            if (parent == null) continue;

            // Convert to normalized anchors
            Vector2 minAnchor = new Vector2(
                rect.anchorMin.x + rect.offsetMin.x / parent.rect.width,
                rect.anchorMin.y + rect.offsetMin.y / parent.rect.height);

            Vector2 maxAnchor = new Vector2(
                rect.anchorMax.x + rect.offsetMax.x / parent.rect.width,
                rect.anchorMax.y + rect.offsetMax.y / parent.rect.height);

            rect.anchorMin = minAnchor;
            rect.anchorMax = maxAnchor;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            EditorUtility.SetDirty(rect);
        }

        Debug.Log("✅ Auto-anchored selected UI elements!");
    }

    private void ApplyResolution(Vector2 resolution)
    {
        GameViewUtils.SetGameViewSize((int)resolution.x, (int)resolution.y);
        Debug.Log($"🔍 Previewing resolution: {resolution.x}x{resolution.y}");
    }
}

// Utility class to change GameView resolution in Editor
public static class GameViewUtils
{
    public static void SetGameViewSize(int width, int height)
    {
        System.Type gameViewType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        EditorWindow gameView = EditorWindow.GetWindow(gameViewType);

        var getGroup = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleton = typeof(ScriptableSingleton<>).MakeGenericType(getGroup);
        var instance = singleton.GetProperty("instance").GetValue(null, null);
        var group = getGroup.GetMethod("GetGroup").Invoke(instance, new object[] { 0 });

        var addCustomSize = group.GetType().GetMethod("AddCustomSize");
        var gvSize = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var gvType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
        var ctor = gvSize.GetConstructor(new System.Type[] { gvType, typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { 1, width, height, $"{width}x{height}" });
        addCustomSize.Invoke(group, new object[] { newSize });

        var selectedIndexProp = gameViewType.GetProperty("selectedSizeIndex", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        selectedIndexProp.SetValue(gameView, group.GetType().GetMethod("IndexOf").Invoke(group, new object[] { newSize }));
    }
}
