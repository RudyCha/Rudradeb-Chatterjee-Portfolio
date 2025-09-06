using UnityEngine;

public class PopupTester : MonoBehaviour
{
    private void Start()
    {
        // Trigger popup after 2 seconds
        Invoke(nameof(ShowTestPopup), 2f);
    }

    private void ShowTestPopup()
    {
        // Example: Info popup
        PopupManager.Instance.ShowPopup(PopupType.Info, "Welcome to the game!");

        // You could also try other types:
        // PopupManager.Instance.ShowPopup(PopupType.Warning, "Energy is low!");
        // PopupManager.Instance.ShowPopup(PopupType.Error, "Connection failed!");
        // PopupManager.Instance.ShowPopup(PopupType.Reward, "You got 100 coins!");
    }
}
