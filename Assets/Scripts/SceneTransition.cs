using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Image fadeImage;                          
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private static SceneTransition instance;
    public static SceneTransition Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (fadeImage == null)
            CreateFadeImage();

        if (fadeImage != null)
        {
            SetAlpha(1f);
            StartCoroutine(FadeIn());
        }
    }

    private void CreateFadeImage()
    {
        var canvasGO = new GameObject("TransitionCanvas");
        canvasGO.transform.SetParent(transform, false);

        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10000; 
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        var fadeGO = new GameObject("FadeImage");
        fadeGO.transform.SetParent(canvasGO.transform, false);

        fadeImage = fadeGO.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.raycastTarget = false;

        var rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private void SetAlpha(float a)
    {
        var c = fadeImage.color;
        c.a = Mathf.Clamp01(a);
        fadeImage.color = c;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithTransition(sceneName));
    }

    public void LoadScene(int buildIndex)
    {
        StartCoroutine(LoadSceneWithTransition(buildIndex));
    }

    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(sceneName);
        yield return null; 
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator LoadSceneWithTransition(int buildIndex)
    {
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(buildIndex);
        yield return null;
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        var startColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; 
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float a = fadeCurve.Evaluate(t);

            var c = startColor;
            c.a = a;
            fadeImage.color = c;

            yield return null;
        }

        var fc = startColor;
        fc.a = 1f;
        fadeImage.color = fc;
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;
        var startColor = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float a = fadeCurve.Evaluate(1f - t);

            var c = startColor;
            c.a = a;
            fadeImage.color = c;

            yield return null;
        }

        var fc = startColor;
        fc.a = 0f;
        fadeImage.color = fc;
    }

    public System.Collections.IEnumerator FadeOutRoutine()
    {
        yield return StartCoroutine(FadeOut());
    }

    public System.Collections.IEnumerator FadeInRoutine()
    {
        yield return StartCoroutine(FadeIn());
    }

}
