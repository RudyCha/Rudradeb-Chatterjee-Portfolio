using UnityEngine;
using UnityEngine.UI;

public class UniversalPanelButton : MonoBehaviour
{
    public enum ActionType { Enable, Disable, Toggle }

    [Header("Settings")]
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private ActionType action = ActionType.Toggle;

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(PerformAction);
        }
        else
        {
            Debug.LogWarning($"{name}: No Button component found.");
        }
    }

    private void PerformAction()
    {
        if (targetPanel == null)
        {
            Debug.LogWarning($"{name}: No target panel assigned.");
            return;
        }

        switch (action)
        {
            case ActionType.Enable:
                targetPanel.SetActive(true);
                break;
            case ActionType.Disable:
                targetPanel.SetActive(false);
                break;
            case ActionType.Toggle:
                targetPanel.SetActive(!targetPanel.activeSelf);
                break;
        }
    }
}
