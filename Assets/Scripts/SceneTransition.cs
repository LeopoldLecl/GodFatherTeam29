using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static SceneTransition instance;
    public static SceneTransition Instance => instance;

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // S'assurer que l'image de fade existe
        if (fadeImage == null)
        {
            CreateFadeImage();
        }

        // Commencer avec un écran noir puis faire un fade in
        if (fadeImage != null)
        {
            fadeImage.color = Color.black;
            StartCoroutine(FadeIn());
        }
    }

    private void CreateFadeImage()
    {
        // Créer un Canvas pour le fade si nécessaire
        GameObject canvasGO = new GameObject("TransitionCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // Au-dessus de tout

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        // Créer l'image de fade
        GameObject fadeGO = new GameObject("FadeImage");
        fadeGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = fadeGO.AddComponent<Image>();
        fadeImage.color = Color.black;

        // Faire en sorte qu'elle couvre tout l'écran
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        DontDestroyOnLoad(canvasGO);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneWithTransition(sceneIndex));
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Charger la scène
        SceneManager.LoadScene(sceneName);

        // Attendre un frame
        yield return null;

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator LoadSceneWithTransition(int sceneIndex)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Charger la scène
        SceneManager.LoadScene(sceneIndex);

        // Attendre un frame
        yield return null;

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color startColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / fadeDuration;
            float curveValue = fadeCurve.Evaluate(normalizedTime);

            Color newColor = startColor;
            newColor.a = curveValue;
            fadeImage.color = newColor;

            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 1f;
        fadeImage.color = finalColor;
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        Color startColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedTime = elapsed / fadeDuration;
            float curveValue = fadeCurve.Evaluate(1f - normalizedTime);

            Color newColor = startColor;
            newColor.a = curveValue;
            fadeImage.color = newColor;

            yield return null;
        }

        Color finalColor = startColor;
        finalColor.a = 0f;
        fadeImage.color = finalColor;
    }
}