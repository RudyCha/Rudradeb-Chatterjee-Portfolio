using UnityEngine;
using UnityEngine.UI;
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
using TMPro;
#endif
using System.Collections.Generic;

public enum ThemeColorToken
{
    None,
    Background,
    Surface,
    Primary,
    Secondary,
    TextPrimary,
    TextSecondary,
    Accent,
    Success,
    Warning,
    Danger
}

[DisallowMultipleComponent]
public class UIThemeApplier : MonoBehaviour
{
    [Header("What to apply")]
    public bool applyGraphicColor = true;
    public ThemeColorToken colorToken = ThemeColorToken.TextPrimary;

    [Tooltip("If true, set Image.sprite from ThemeDefinition by a string key.")]
    public bool applySprite = false;
    public string spriteKey;

    [Tooltip("Apply fonts if present in theme.")]
    public bool applyFont = false;

    [Header("Optional overrides")]
    public bool multiplyColor = false;
    public Color colorMultiplier = Color.white;

    private Graphic graphic;
    private Image image;
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
    private TMP_Text tmpText;
#endif
    private Text legacyText;

    private static readonly List<UIThemeApplier> _all = new List<UIThemeApplier>();

    void Awake()
    {
        graphic = GetComponent<Graphic>();
        image   = GetComponent<Image>();
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
        tmpText = GetComponent<TMP_Text>();
#endif
        legacyText = GetComponent<Text>();
        Register();
    }

    void OnEnable()
    {
        Register();
        if (ThemeManager.Instance && ThemeManager.Instance.ActiveTheme != null)
            Apply(ThemeManager.Instance.ActiveTheme);

        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged += Apply;
    }

    void OnDisable()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged -= Apply;
        Deregister();
    }

    void OnDestroy()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged -= Apply;
        Deregister();
    }

    private void Register()
    {
        if (!_all.Contains(this)) _all.Add(this);
    }
    private void Deregister()
    {
        _all.Remove(this);
    }

    public static void ApplyAll(ThemeDefinition theme)
    {
        for (int i = 0; i < _all.Count; i++)
        {
            if (_all[i]) _all[i].Apply(theme);
        }
    }

    public void Apply(ThemeDefinition theme)
    {
        if (!theme) return;

        if (applyGraphicColor)
        {
            var c = GetColorFromToken(theme, colorToken);
            if (multiplyColor) c *= colorMultiplier;

            if (graphic) graphic.color = c;
        }

        if (applySprite && image)
        {
            if (!string.IsNullOrEmpty(spriteKey) && theme.TryGetSprite(spriteKey, out var spr))
                image.sprite = spr;
        }

        if (applyFont)
        {
#if TMP_PRESENT || TEXTMESHPRO || TMPRO
            if (tmpText && theme.mainTMPFont) tmpText.font = theme.mainTMPFont;
#endif
            if (legacyText && theme.uiFont) legacyText.font = theme.uiFont;
        }
    }

    private Color GetColorFromToken(ThemeDefinition t, ThemeColorToken token)
    {
        switch (token)
        {
            case ThemeColorToken.Background:    return t.background;
            case ThemeColorToken.Surface:       return t.surface;
            case ThemeColorToken.Primary:       return t.primary;
            case ThemeColorToken.Secondary:     return t.secondary;
            case ThemeColorToken.TextPrimary:   return t.textPrimary;
            case ThemeColorToken.TextSecondary: return t.textSecondary;
            case ThemeColorToken.Accent:        return t.accent;
            case ThemeColorToken.Success:       return t.success;
            case ThemeColorToken.Warning:       return t.warning;
            case ThemeColorToken.Danger:        return t.danger;
            default: return Color.white;
        }
    }
}
