using System;
using System.Collections.Generic;
using UnityEngine;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Themes (drag all available ThemeDefinition assets)")]
    [SerializeField] private List<ThemeDefinition> themes = new List<ThemeDefinition>();

    [Header("Startup")]
    [Tooltip("If true, restore last selected theme from PlayerPrefs.")]
    [SerializeField] private bool restoreLastTheme = true;

    [Tooltip("Fallback themeId if nothing saved.")]
    [SerializeField] private string defaultThemeId = "light";

    [Header("Debug")]
    [SerializeField] private bool applyOnAwake = true;

    public ThemeDefinition ActiveTheme { get; private set; }

    public event Action<ThemeDefinition> OnThemeChanged;

    const string PrefKey = "THEME_MANAGER_ACTIVE_THEME_ID";

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (applyOnAwake)
        {
            if (restoreLastTheme && PlayerPrefs.HasKey(PrefKey))
            {
                string saved = PlayerPrefs.GetString(PrefKey, defaultThemeId);
                SwitchThemeById(saved, true);
            }
            else
            {
                SwitchThemeById(defaultThemeId, true);
            }
        }
    }

    public IReadOnlyList<ThemeDefinition> GetThemes() => themes;

    public bool HasTheme(string themeId) => themes.Exists(t => t && t.themeId == themeId);

    public void SwitchThemeById(string themeId, bool silent = false)
    {
        ThemeDefinition found = themes.Find(t => t && t.themeId == themeId);
        if (!found)
        {
            Debug.LogWarning($"ThemeManager: Theme '{themeId}' not found. Using default '{defaultThemeId}'.");
            found = themes.Find(t => t && t.themeId == defaultThemeId);
            if (!found && themes.Count > 0) found = themes[0];
        }

        if (!found) { Debug.LogError("ThemeManager: No themes configured."); return; }

        ActiveTheme = found;
        PlayerPrefs.SetString(PrefKey, ActiveTheme.themeId);
        PlayerPrefs.Save();

        if (!silent) OnThemeChanged?.Invoke(ActiveTheme);

        // Force apply to already-placed UIThemeApplier components
        UIThemeApplier.ApplyAll(ActiveTheme);
    }

    public void SwitchThemeByIndex(int index)
    {
        if (themes.Count == 0) return;
        int idx = Mathf.Clamp(index, 0, themes.Count - 1);
        SwitchThemeById(themes[idx].themeId);
    }

    public void NextTheme()
    {
        if (themes.Count == 0) return;
        int current = Mathf.Max(0, themes.FindIndex(t => t == ActiveTheme));
        int next = (current + 1) % themes.Count;
        SwitchThemeByIndex(next);
    }

    public void PreviousTheme()
    {
        if (themes.Count == 0) return;
        int current = Mathf.Max(0, themes.FindIndex(t => t == ActiveTheme));
        int prev = (current - 1 + themes.Count) % themes.Count;
        SwitchThemeByIndex(prev);
    }

    // Convenience helpers
    public void ToggleLightDark(string lightId = "light", string darkId = "dark")
    {
        if (ActiveTheme != null && ActiveTheme.themeId == lightId && HasTheme(darkId))
            SwitchThemeById(darkId);
        else if (HasTheme(lightId))
            SwitchThemeById(lightId);
    }

    public void SwitchToTeam(string teamId)
    {
        var found = themes.Find(t => t && t.teamId == teamId);
        if (found) SwitchThemeById(found.themeId);
        else Debug.LogWarning($"ThemeManager: No theme with teamId '{teamId}' found.");
    }
}
