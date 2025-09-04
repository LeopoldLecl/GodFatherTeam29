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
    private int currentClicks = 0;
    private bool hasCompleted = false;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Color originalColor;
    private Image targetImageComponent;
    private RectTransform canvasRect;

    public override void Init()
    {
        base.Init();

        timer = timeLimit;
        currentClicks = 0;
        hasCompleted = false;

        if (targetImage != null)
        {
            originalScale = targetImage.transform.localScale;
            originalPosition = targetImage.transform.localPosition;
            targetImageComponent = targetImage.GetComponent<Image>();
            if (targetImageComponent != null)
                originalColor = targetImageComponent.color;
        }

        canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();

        SetupUI();

        StartCoroutine(AppearanceEffect());

    }

    public override void Clear()
    {
        base.Clear();

        if (targetImage != null)
        {
            targetImage.interactable = false;
            targetImage.onClick.RemoveListener(OnImageClicked); 
        }

        StopAllCoroutines();
    }

    private void SetupUI()
    {
        if (targetImage != null)
        {
            targetImage.interactable = true;

            targetImage.onClick.RemoveListener(OnImageClicked);
            targetImage.onClick.AddListener(OnImageClicked);

            targetImage.gameObject.SetActive(true);
            targetImage.transform.localScale = originalScale;

            if (targetImageComponent != null)
                targetImageComponent.color = originalColor;
        }

        UpdateUI();
    }

    void Update()
    {
        if (!isGameActive || hasCompleted) return;

        timer -= Time.deltaTime;

        UpdateUI();

        if (timer <= 3f && timer > 0)
        {
            float intensity = Mathf.PingPong(Time.time * 8f, 1f);
            if (timerText != null)
            {
                timerText.color = Color.Lerp(Color.white, Color.red, intensity);
                timerText.transform.localScale = Vector3.one * (1f + intensity * 0.1f);
            }
        }

        if (timer <= 0)
        {
            hasCompleted = true;
            StartCoroutine(FailureEffect());
        }
    }

    private void UpdateUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Temps: {timer:F1}s";
        }
    }

    private void OnImageClicked()
    {
        if (!isGameActive || hasCompleted)
        {
            return;
        }

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
            float normalizedTime = elapsed / duration;
            float scaleMultiplier = 1f + (punchCurve.Evaluate(normalizedTime) * (punchStrength - 1f));

            float progressiveScale = 1f - ((float)currentClicks / requiredClicks * 0.8f);
            targetImage.transform.localScale = originalScale * progressiveScale * scaleMultiplier;

            elapsed += Time.deltaTime;
            yield return null;
        }

        float finalScale = 1f - ((float)currentClicks / requiredClicks * 0.8f);
        targetImage.transform.localScale = originalScale * finalScale;
    }

    private IEnumerator ColorFlash()
    {
        if (targetImageComponent == null) yield break;

        Color flashColor = clickColors[Random.Range(0, clickColors.Length)];
        Color startColor = targetImageComponent.color;

        float duration = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetImageComponent.color = Color.Lerp(startColor, flashColor * flashIntensity, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        duration = 0.2f;
        Color targetColor = originalColor;
        targetColor.a = 1f - ((float)currentClicks / requiredClicks * 0.7f);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            targetImageComponent.color = Color.Lerp(flashColor * flashIntensity, targetColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImageComponent.color = targetColor;
    }

    private IEnumerator ScreenShake()
    {
        if (canvasRect == null) yield break;

        Vector3 originalPos = canvasRect.localPosition;
        float intensity = maxShakeIntensity * (1f - (float)currentClicks / requiredClicks * 0.7f);
        float duration = 0.15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-intensity, intensity);
            float y = Random.Range(-intensity, intensity);

            canvasRect.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            intensity *= 0.96f; 
            yield return null;
        }

        canvasRect.localPosition = originalPos;
    }

    private IEnumerator RotationWobble()
    {
        if (targetImage == null) yield break;

        float wobbleAngle = Random.Range(-15f, 15f);
        float duration = 0.2f;
        float elapsed = 0f;
        Quaternion originalRotation = targetImage.transform.localRotation;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float angle = wobbleAngle * Mathf.Sin(t * Mathf.PI);
            targetImage.transform.localRotation = originalRotation * Quaternion.Euler(0, 0, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.transform.localRotation = originalRotation;
    }

    private void SpawnClickParticles()
    {
        if (clickParticles == null || targetImage == null) return;

        if (!clickParticles.gameObject.activeInHierarchy)
            clickParticles.gameObject.SetActive(true);

        var emission = clickParticles.emission;
        if (!emission.enabled)
            emission.enabled = true;

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
        else
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                Vector3 sp = Input.mousePosition;
                sp.z = 1f;
                worldPos = cam.ScreenToWorldPoint(sp);
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
            targetImage.transform.localScale = originalScale * easeOut;

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.transform.localScale = originalScale;
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
            emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0.0f, 50)
            });
            clickParticles.Play();
        }

        float duration = 0.8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.5f;
            targetImage.transform.localScale = originalScale * scale * (1f - t);

            
            targetImage.transform.localRotation = Quaternion.Euler(0, 0, t * 360f * 2f);

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

            targetImage.transform.localScale = originalScale * (1f - t);

            if (targetImageComponent != null)
            {
                Color color = Color.Lerp(originalColor, Color.black, t);
                color.a = 1f - t;
                targetImageComponent.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        CompleteMinigame(false);
    }

    void OnDestroy()
    {
        if (targetImage != null)
        {
            targetImage.onClick.RemoveListener(OnImageClicked);
        }
    }
}
