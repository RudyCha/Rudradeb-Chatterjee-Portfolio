using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class UniversalButtonStyler : EditorWindow
{
    // Style options
    private TMP_FontAsset newFont;
    private int newFontSize = 14;
    private Color newTextColor = Color.white;
    private Color newButtonColor = Color.blue;
    private Vector4 newPadding = new Vector4(10, 5, 10, 5);

    [MenuItem("Tools/UI/Batch Button Styler")]
    public static void ShowWindow()
    {
        GetWindow<UniversalButtonStyler>("Batch Button Styler");
    }

    private void OnGUI()
    {
        GUILayout.Label("Batch Button Styler", EditorStyles.boldLabel);

        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Font", newFont, typeof(TMP_FontAsset), false);
        newFontSize = EditorGUILayout.IntField("Font Size", newFontSize);
        newTextColor = EditorGUILayout.ColorField("Text Color", newTextColor);
        newButtonColor = EditorGUILayout.ColorField("Button Color", newButtonColor);
        newPadding = EditorGUILayout.Vector4Field("Text Padding (L,T,R,B)", newPadding);

        GUILayout.Space(10);

        if (GUILayout.Button("Apply to Selected Buttons"))
        {
            ApplyToSelected();
        }

        if (GUILayout.Button("Apply to All Buttons in Scene"))
        {
            ApplyToAll();
        }
    }

    private void ApplyToSelected()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                StyleButton(btn);
            }
        }
    }

    private void ApplyToAll()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // includes inactive
        foreach (Button btn in buttons)
        {
            StyleButton(btn);
        }
    }

    private void StyleButton(Button btn)
    {
        // Change button background
        Image bg = btn.GetComponent<Image>();
        if (bg != null)
        {
            Undo.RecordObject(bg, "Batch Style Button");
            bg.color = newButtonColor;
        }

        // Change text
        TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            Undo.RecordObject(text, "Batch Style Text");
            if (newFont != null) text.font = newFont;
            text.fontSize = newFontSize;
            text.color = newTextColor;
            text.margin = newPadding;
        }

        EditorUtility.SetDirty(btn);
    }
}
