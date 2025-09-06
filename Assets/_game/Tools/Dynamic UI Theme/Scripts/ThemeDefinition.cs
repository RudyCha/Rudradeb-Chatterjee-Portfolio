using System;
using UnityEngine;
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
using TMPro;
#endif

[CreateAssetMenu(fileName = "ThemeDefinition", menuName = "UI/Theming/Theme Definition", order = 0)]
public class ThemeDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique ID used by ThemeManager. Keep it stable.")]
    public string themeId = "light";
    public string displayName = "Light";

    [Header("Optional: Team Skin")]
    [Tooltip("Use for team-specific themes (e.g., 'Lakers').")]
    public string teamId;
    public Sprite teamLogo;

    [Header("Color Tokens")]
    public Color background = Color.white;
    public Color surface = new Color(0.95f, 0.95f, 0.95f);
    public Color primary = new Color(0.13f, 0.52f, 0.96f);
    public Color secondary = new Color(0.2f, 0.2f, 0.2f);
    public Color textPrimary = Color.black;
    public Color textSecondary = new Color(0.2f, 0.2f, 0.2f);
    public Color accent = new Color(1f, 0.76f, 0.03f);
    public Color success = new Color(0.2f, 0.7f, 0.3f);
    public Color warning = new Color(0.95f, 0.6f, 0.1f);
    public Color danger = new Color(0.85f, 0.2f, 0.2f);

    [Header("Optional Fonts")]
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
    public TMP_FontAsset mainTMPFont;
    public TMP_FontAsset altTMPFont;
#endif
    public Font uiFont;

    [Header("Named Sprites (e.g., 'BtnPrimary', 'IconBack')")]
    public NamedSprite[] sprites;

    [Serializable]
    public struct NamedSprite
    {
        public string key;
        public Sprite sprite;
    }

    public bool TryGetSprite(string key, out Sprite sprite)
    {
        if (sprites != null)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                if (!string.IsNullOrEmpty(sprites[i].key) && sprites[i].key == key)
                {
                    sprite = sprites[i].sprite;
                    return sprite != null;
                }
            }
        }
        sprite = null;
        return false;
    }
}
