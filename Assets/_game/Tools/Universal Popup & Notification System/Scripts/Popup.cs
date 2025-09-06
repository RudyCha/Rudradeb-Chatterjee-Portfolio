using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Popup : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text messageText;

    private PopupConfig config;
    private AudioSource audioSource;
    private System.Action<bool> confirmationCallback;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        transform.localScale = Vector3.zero;
    }

    public void Setup(PopupConfig cfg, string message, System.Action<bool> onConfirm = null)
    {
        config = cfg;
        confirmationCallback = onConfirm;

        if (background) background.color = cfg.backgroundColor;
        if (iconImage && cfg.icon) iconImage.sprite = cfg.icon;
        if (messageText)
        {
            messageText.text = message;
            messageText.color = cfg.textColor;
        }

        if (cfg.showSound) audioSource.PlayOneShot(cfg.showSound);

        StopAllCoroutines();
        StartCoroutine(AnimateScale(cfg.hideScale, cfg.showScale, cfg.showDuration, cfg.showEase));
    }

    public void Close(bool confirmed = false)
    {
        if (config.hideSound) audioSource.PlayOneShot(config.hideSound);

        StopAllCoroutines();
        StartCoroutine(AnimateScale(config.showScale, config.hideScale, config.hideDuration, config.hideEase, () =>
        {
            confirmationCallback?.Invoke(confirmed);
            Destroy(gameObject);
        }));
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration, AnimationCurve curve, System.Action onComplete = null)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = curve.Evaluate(Mathf.Clamp01(t / duration));
            transform.localScale = Vector3.LerpUnclamped(from, to, p);
            yield return null;
        }
        transform.localScale = to;
        onComplete?.Invoke();
    }

    // Example button hookups (for confirmation popups)
    public void OnConfirm() => Close(true);
    public void OnCancel() => Close(false);
}
