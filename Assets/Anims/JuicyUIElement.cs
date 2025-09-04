using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class JuicyUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Scale Animation")]
    [SerializeField] private bool useScale = true;
    [SerializeField] private float scalePunch = 1.2f;
    [SerializeField] private float scaleDuration = 0.2f;

    [Header("Color Flash (Image/TMP)")]
    [SerializeField] private bool useColorFlash = false;
    [SerializeField] private Color flashColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.15f;

    [Header("Rotation Wiggle")]
    [SerializeField] private bool useWiggle = false;
    [SerializeField] private float wiggleAngle = 10f;
    [SerializeField] private float wiggleDuration = 0.15f;

    [Header("Idle Animation")]
    [SerializeField] private bool useIdle = false;
    [SerializeField] private bool idleIsScale = true;
    [SerializeField] private float idleStrength = 0.05f;
    [SerializeField] private float idleDuration = 1.2f;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Color originalColor;
    private Graphic graphic;
    private Tween idleTween;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalRotation = transform.rotation;

        graphic = GetComponent<Graphic>();
        if (graphic != null)
            originalColor = graphic.color;
    }

    private void OnEnable()
    {
        StartIdle();
    }

    private void OnDisable()
    {
        StopIdle();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (useScale) AnimateScale(scalePunch);
        if (useWiggle) AnimateWiggle();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (useScale) ResetScale();
        if (useColorFlash) ResetColor();
        if (useWiggle) ResetRotation();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (useColorFlash) AnimateColorFlash();
        if (useScale) AnimateScale(scalePunch * 1.1f);
    }

    private void AnimateScale(float target)
    {
        transform.DOKill(true);
        transform.DOPunchScale(originalScale * (target - 1f), scaleDuration, vibrato: 1, elasticity: 0.5f);
        RestartIdle();
    }

    private void ResetScale()
    {
        transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(RestartIdle);
    }

    private void AnimateColorFlash()
    {
        if (graphic == null) return;

        graphic.DOColor(flashColor, flashDuration)
            .OnComplete(() =>
            {
                graphic.DOColor(originalColor, flashDuration);
            });
    }

    private void AnimateWiggle()
    {
        transform.DORotate(new Vector3(0, 0, wiggleAngle), wiggleDuration, RotateMode.LocalAxisAdd)
                 .SetLoops(2, LoopType.Yoyo)
                 .SetEase(Ease.OutQuad);
    }

    private void ResetColor()
    {
        if (graphic != null)
            graphic.DOColor(originalColor, 0.1f);
    }

    private void ResetRotation()
    {
        transform.DORotateQuaternion(originalRotation, 0.1f).SetEase(Ease.OutQuad);
    }

    private void StartIdle()
    {
        if (!useIdle) return;

        if (idleIsScale)
        {
            idleTween = transform.DOScale(originalScale * (1f + idleStrength), idleDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else
        {
            idleTween = transform.DORotate(new Vector3(0, 0, idleStrength * 10f), idleDuration, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void StopIdle()
    {
        idleTween?.Kill();
        transform.localScale = originalScale;
        transform.rotation = originalRotation;
    }

    private void RestartIdle()
    {
        StopIdle();
        StartIdle();
    }
}
