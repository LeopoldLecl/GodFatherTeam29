using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_ScreenClean : Minigame
{
    [Header("UI References")]
    [SerializeField] private Button targetImage;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private ParticleSystem clickParticles;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Slider timerSlider;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 10f;
    [SerializeField] private int requiredClicks = 15;

    [Header("Juice Effects")]
    [SerializeField] private AnimationCurve punchCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private Color[] clickColors = { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta };
    [SerializeField] private AudioClip[] clickSounds;
    [SerializeField] private float maxShakeIntensity = 15f;
    [SerializeField] private float flashIntensity = 2f;

    private float timer;
    private int currentClicks;
    private bool hasCompleted;

    private Vector3 imgOrigScale;
    private Quaternion imgOrigRotation;
    private Color imgOrigColor = Color.white;
    private Image targetImageComponent;

    private RectTransform canvasRect;
    private Vector3 canvasOrigLocalPos;

    private Color timerOrigColor = Color.white;
    private Vector3 timerOrigScale = Vector3.one;

    public override void Init()
    {
        base.Init();

        if (canvasRect == null)
        {
            var canvas = GetComponentInParent<Canvas>();
            canvasRect = canvas ? canvas.GetComponent<RectTransform>() : null;
            if (canvasRect != null) canvasOrigLocalPos = canvasRect.localPosition;
        }

        if (targetImage != null)
        {
            targetImageComponent = targetImage.GetComponent<Image>();
            imgOrigScale = targetImage.transform.localScale;
            imgOrigRotation = targetImage.transform.localRotation;
            if (targetImageComponent != null) imgOrigColor = targetImageComponent.color;

            targetImage.onClick.RemoveListener(OnImageClicked);
            targetImage.onClick.AddListener(OnImageClicked);
        }

        if (timerText != null)
        {
            timerOrigColor = timerText.color;
            timerOrigScale = timerText.transform.localScale;
        }

        ResetGameplayState();
        ResetVisuals();

        UpdateUI();
        StartCoroutine(AppearanceEffect());
    }

    public override void Clear()
    {
        StopAllCoroutines();

        if (targetImage != null)
        {
            targetImage.onClick.RemoveListener(OnImageClicked);
            targetImage.interactable = false;
        }

        ResetVisuals();

        base.Clear();
    }

    private void ResetGameplayState()
    {
        timer = timeLimit;
        currentClicks = 0;
        hasCompleted = false;
    }

    private void ResetVisuals()
    {
        if (canvasRect != null)
            canvasRect.localPosition = canvasOrigLocalPos;

        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(true);
            targetImage.interactable = true;
            targetImage.transform.localScale = imgOrigScale;
            targetImage.transform.localRotation = imgOrigRotation;
            if (targetImageComponent != null)
            {
                var c = imgOrigColor;
                c.a = 1f;
                targetImageComponent.color = c;
            }
        }

        if (timerText != null)
        {
            timerText.color = timerOrigColor;
            timerText.transform.localScale = timerOrigScale;
        }
    }

    private void Update()
    {
        if (!isGameActive || hasCompleted) return;

        timer -= Time.deltaTime;
        UpdateUI();

        if (timer <= 3f && timer > 0f && timerText != null)
        {
            float t = Mathf.PingPong(Time.unscaledTime * 8f, 1f);
            timerText.color = Color.Lerp(timerOrigColor, Color.red, t);
            timerText.transform.localScale = Vector3.Lerp(timerOrigScale, timerOrigScale * 1.1f, t);
        }

        if (timer <= 0f)
        {
            hasCompleted = true;
            StartCoroutine(FailureEffect());
        }
    }

    private void UpdateUI()
    {
        if (timerText != null)
            timerText.text = $"Temps: {Mathf.Max(0f, timer):F1}s";
        if (timerSlider != null)
            timerSlider.value = timer / timeLimit;
    }

    private void OnImageClicked()
    {
        if (!isGameActive || hasCompleted) return;

        currentClicks++;
        StartCoroutine(ClickEffects());

        if (currentClicks >= requiredClicks)
        {
            hasCompleted = true;
            StartCoroutine(SuccessEffect());
        }
    }

    private IEnumerator ClickEffects()
    {
        PlayRandomClickSound();
        SpawnClickParticles();
        StartCoroutine(PunchScale());
        StartCoroutine(ColorFlash());
        StartCoroutine(ScreenShake());
        StartCoroutine(RotationWobble());
        yield return null;
    }

    private IEnumerator PunchScale()
    {
        if (targetImage == null) yield break;

        float duration = 0.3f;
        float elapsed = 0f;
        float punchStrength = 1.2f - ((float)currentClicks / requiredClicks * 0.5f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scaleMul = 1f + (punchCurve.Evaluate(t) * (punchStrength - 1f));
            float progressive = 1f - ((float)currentClicks / requiredClicks * 0.8f);
            targetImage.transform.localScale = imgOrigScale * progressive * scaleMul;

            elapsed += Time.deltaTime;
            yield return null;
        }

        float finalScale = 1f - ((float)currentClicks / requiredClicks * 0.8f);
        targetImage.transform.localScale = imgOrigScale * finalScale;
    }

    private IEnumerator ColorFlash()
    {
        if (targetImageComponent == null) yield break;

        Color flashColor = clickColors[Random.Range(0, clickColors.Length)];
        Color startColor = targetImageComponent.color;

        float upDur = 0.1f;
        float downDur = 0.2f;
        float elapsed = 0f;

        while (elapsed < upDur)
        {
            float t = elapsed / upDur;
            targetImageComponent.color = Color.Lerp(startColor, flashColor * flashIntensity, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        Color targetColor = imgOrigColor; targetColor.a = 1f - ((float)currentClicks / requiredClicks * 0.7f);
        while (elapsed < downDur)
        {
            float t = elapsed / downDur;
            targetImageComponent.color = Color.Lerp(flashColor * flashIntensity, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImageComponent.color = targetColor;
    }

    private IEnumerator ScreenShake()
    {
        if (canvasRect == null) yield break;

        Vector3 start = canvasRect.localPosition;
        float intensity = maxShakeIntensity * (1f - (float)currentClicks / requiredClicks * 0.7f);
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);
            canvasRect.localPosition = start + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            intensity *= 0.96f;
            yield return null;
        }

        canvasRect.localPosition = start;
    }

    private IEnumerator RotationWobble()
    {
        if (targetImage == null) yield break;

        float wobbleAngle = Random.Range(-15f, 15f);
        float duration = 0.2f;
        float elapsed = 0f;
        Quaternion startRot = targetImage.transform.localRotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angle = wobbleAngle * Mathf.Sin(t * Mathf.PI);
            targetImage.transform.localRotation = startRot * Quaternion.Euler(0, 0, angle);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.transform.localRotation = startRot;
    }

    private void SpawnClickParticles()
    {
        if (clickParticles == null || targetImage == null) return;

        if (!clickParticles.gameObject.activeInHierarchy)
            clickParticles.gameObject.SetActive(true);

        var emission = clickParticles.emission;
        if (!emission.enabled) emission.enabled = true;

        var main = clickParticles.main;
        if (main.startLifetime.constantMax <= 0f) main.startLifetime = 0.6f;
        if (main.startSpeed.constantMax <= 0f) main.startSpeed = 2.5f;
        if (main.startSize.constantMax <= 0f) main.startSize = 0.15f;
        if (main.maxParticles <= 0) main.maxParticles = 1000;

        if (clickColors != null && clickColors.Length > 0)
            main.startColor = clickColors[Random.Range(0, clickColors.Length)];

        Vector3 worldPos = targetImage.transform.position;
        var canvas = targetImage.GetComponentInParent<Canvas>();
        var rect = targetImage.transform as RectTransform;

        if (canvas != null && rect != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                Camera uiCam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
                if (uiCam != null &&
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, Input.mousePosition, uiCam, out var wp))
                {
                    worldPos = wp;
                }
            }
            else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    Vector3 sp = Input.mousePosition;
                    sp.z = 1f;
                    worldPos = cam.ScreenToWorldPoint(sp);
                }
            }
            else
            {
                worldPos = rect.position;
            }
        }

        clickParticles.transform.position = worldPos;
        clickParticles.Emit(15);
    }

    private void PlayRandomClickSound()
    {
        if (audioSource != null && clickSounds != null && clickSounds.Length > 0)
        {
            AudioClip randomClip = clickSounds[Random.Range(0, clickSounds.Length)];
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(randomClip);
        }
    }

    private IEnumerator AppearanceEffect()
    {
        if (targetImage == null) yield break;

        targetImage.transform.localScale = Vector3.zero;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float easeOut = 1f - Mathf.Pow(1f - t, 3f);
            targetImage.transform.localScale = imgOrigScale * easeOut;
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.transform.localScale = imgOrigScale;
    }

    private IEnumerator SuccessEffect()
    {
        if (targetImage == null) yield break;

        if (audioSource != null)
        {
            audioSource.pitch = 1.5f;
            if (clickSounds != null && clickSounds.Length > 0)
                audioSource.PlayOneShot(clickSounds[0]);
        }

        if (clickParticles != null)
        {
            var main = clickParticles.main;
            main.startColor = Color.yellow;
            var emission = clickParticles.emission;
            emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 50) });
            clickParticles.Play();
        }

        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.5f;
            targetImage.transform.localScale = imgOrigScale * scale * (1f - t);
            targetImage.transform.localRotation = Quaternion.Euler(0, 0, t * 720f);

            if (targetImageComponent != null)
            {
                Color color = targetImageComponent.color;
                color.a = 1f - t;
                targetImageComponent.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.gameObject.SetActive(false);
        CompleteMinigame(true);
    }

    private IEnumerator FailureEffect()
    {
        if (targetImage == null) yield break;

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetImage.transform.localScale = imgOrigScale * (1f - t);

            if (targetImageComponent != null)
            {
                Color color = Color.Lerp(imgOrigColor, Color.black, t);
                color.a = 1f - t;
                targetImageComponent.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        CompleteMinigame(false);
    }

    private void OnDestroy()
    {
        if (targetImage != null)
            targetImage.onClick.RemoveListener(OnImageClicked);
    }
}
