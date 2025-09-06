using UnityEngine;

[CreateAssetMenu(fileName = "PopupConfig", menuName = "UI/Popup Config")]
public class PopupConfig : ScriptableObject
{
    public PopupType popupType;

    [Header("Appearance")]
    public Sprite icon;
    public Color backgroundColor = Color.white;
    public Color textColor = Color.black;

    [Header("Animation Settings")]
    public float showDuration = 0.3f;
    public float hideDuration = 0.25f;
    public Vector3 showScale = Vector3.one;
    public Vector3 hideScale = Vector3.zero;
    public AnimationCurve showEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve hideEase = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Audio")]
    public AudioClip showSound;
    public AudioClip hideSound;
}
