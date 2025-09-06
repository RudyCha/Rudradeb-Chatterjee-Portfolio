using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ThemeToggleButton : MonoBehaviour
{
    [Header("Mode")]
    [Tooltip("Cycle through all registered themes in ThemeManager.")]
    public bool cycleAllThemes = true;

    [Tooltip("If not cycling, toggles between these two IDs (e.g., 'light' and 'dark').")]
    public string lightThemeId = "light";
    public string darkThemeId = "dark";

    private Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickToggle);
    }

    private void OnClickToggle()
    {
        var mgr = ThemeManager.Instance;
        if (mgr == null) { Debug.LogWarning("ThemeToggleButton: No ThemeManager in scene."); return; }

        if (cycleAllThemes)
        {
            mgr.NextTheme();
        }
        else
        {
            mgr.ToggleLightDark(lightThemeId, darkThemeId);
        }
    }
}
