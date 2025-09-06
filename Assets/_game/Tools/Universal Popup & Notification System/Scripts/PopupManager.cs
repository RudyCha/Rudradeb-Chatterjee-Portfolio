using UnityEngine;
using System.Collections.Generic;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    [Header("Popup Prefab")]
    [SerializeField] private GameObject popupPrefab;

    [Header("Popup Configs")]
    [SerializeField] private List<PopupConfig> popupConfigs;

    [Header("Canvas")]
    [SerializeField] private Canvas popupCanvas;

    private Dictionary<PopupType, PopupConfig> configLookup;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        configLookup = new Dictionary<PopupType, PopupConfig>();
        foreach (var cfg in popupConfigs)
            if (cfg) configLookup[cfg.popupType] = cfg;
    }

    public void ShowPopup(PopupType type, string message, System.Action<bool> onConfirm = null)
    {
        if (!popupPrefab || !popupCanvas)
        {
            Debug.LogError("PopupManager: Prefab or Canvas not assigned.");
            return;
        }

        if (!configLookup.TryGetValue(type, out var cfg))
        {
            Debug.LogError($"PopupManager: No config for {type}.");
            return;
        }

        var instance = Instantiate(popupPrefab, popupCanvas.transform);
        var popup = instance.GetComponent<Popup>();
        popup.Setup(cfg, message, onConfirm);
    }
}
