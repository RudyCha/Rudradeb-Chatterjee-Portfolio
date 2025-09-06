using UnityEngine;
using UnityEngine.EventSystems;

public class UIUniversalAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("IDLE ANIMATIONS")]
    [SerializeField] private bool useIdleScale = false;
    [SerializeField] private bool useIdleRotate = false;
    [SerializeField] private bool useIdleMove = false;

    [Header("Idle Scale Settings")]
    [SerializeField] private float idleScaleAmount = 1.05f;
    [SerializeField] private float idleScaleDuration = 1f;

    [Header("Idle Rotation Settings")]
    [SerializeField] private float idleRotationSpeed = 45f;
    [SerializeField] private bool idleClockwise = true;

    [Header("Idle Move Settings")]
    [SerializeField] private Vector3 idleMoveOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float idleMoveDuration = 1f;
    [SerializeField] private bool idleMovePingPong = true;

    [Space(20)]
    [Header("HIGHLIGHT (HOVER) ANIMATIONS")]
    [SerializeField] private bool useHighlightScale = false;
    [SerializeField] private float highlightScale = 1.1f;
    [SerializeField] private float highlightScaleDuration = 0.2f;

    [SerializeField] private bool useHighlightMove = false;
    [SerializeField] private Vector3 highlightMoveOffset = new Vector3(0f, 10f, 0f);
    [SerializeField] private float highlightMoveDuration = 0.2f;
    [SerializeField] private bool highlightMoveReturn = true;

    [SerializeField] private bool useHighlightRotate = false;
    [SerializeField] private float highlightRotationAngle = 15f;
    [SerializeField] private float highlightRotationDuration = 0.25f;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private bool isHovered = false;
    private float scaleTimer = 0f;
    private float moveTimer = 0f;

    void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
    }

    void OnEnable()
    {
        ResetState();
    }

    void OnDisable()
    {
        ResetState();
    }

    void ResetState()
    {
        transform.localScale = originalScale;
        transform.localRotation = originalRotation;
        transform.localPosition = originalPosition;
        scaleTimer = 0f;
        moveTimer = 0f;
        isHovered = false;
    }

    void Update()
    {
        if (!isHovered)
        {
            // IDLE SCALE
            if (useIdleScale)
            {
                scaleTimer += Time.deltaTime;
                float t = (Mathf.Sin((scaleTimer / idleScaleDuration) * Mathf.PI * 2f) + 1f) / 2f;
                transform.localScale = Vector3.Lerp(originalScale, originalScale * idleScaleAmount, t);
            }

            // IDLE ROTATION
            if (useIdleRotate)
            {
                float direction = idleClockwise ? -1f : 1f;
                transform.Rotate(Vector3.forward, direction * idleRotationSpeed * Time.deltaTime);
            }

            // IDLE MOVE
            if (useIdleMove)
            {
                moveTimer += Time.deltaTime;
                float t = Mathf.PingPong(moveTimer / idleMoveDuration, 1f);
                if (!idleMovePingPong) t = Mathf.Clamp01(moveTimer / idleMoveDuration);
                transform.localPosition = Vector3.Lerp(originalPosition, originalPosition + idleMoveOffset, t);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        StopAllCoroutines();

        if (useHighlightScale)
            StartCoroutine(AnimateScale(transform.localScale, originalScale * highlightScale, highlightScaleDuration));

        if (useHighlightMove)
            StartCoroutine(AnimateMove(transform.localPosition, originalPosition + highlightMoveOffset, highlightMoveDuration));

        if (useHighlightRotate)
            StartCoroutine(AnimateRotate(transform.localRotation, Quaternion.Euler(0f, 0f, highlightRotationAngle), highlightRotationDuration));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();

        if (useHighlightScale)
            StartCoroutine(AnimateScale(transform.localScale, originalScale, highlightScaleDuration, () =>
            {
                if (useIdleScale) scaleTimer = 0f;
            }));

        if (useHighlightMove && highlightMoveReturn)
            StartCoroutine(AnimateMove(transform.localPosition, originalPosition, highlightMoveDuration, () =>
            {
                if (useIdleMove) moveTimer = 0f;
            }));

        if (useHighlightRotate)
            StartCoroutine(AnimateRotate(transform.localRotation, originalRotation, highlightRotationDuration, () =>
            {
                if (useIdleRotate) transform.localRotation = originalRotation;
            }));

        isHovered = false;
    }

    // ---------------- ANIMATION HELPERS ----------------
    private System.Collections.IEnumerator AnimateScale(Vector3 from, Vector3 to, float duration, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localScale = to;
        onComplete?.Invoke();
    }

    private System.Collections.IEnumerator AnimateMove(Vector3 from, Vector3 to, float duration, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localPosition = to;
        onComplete?.Invoke();
    }

    private System.Collections.IEnumerator AnimateRotate(Quaternion from, Quaternion to, float duration, System.Action onComplete = null)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            transform.localRotation = Quaternion.Lerp(from, to, t);
            yield return null;
        }
        transform.localRotation = to;
        onComplete?.Invoke();
    }
}
