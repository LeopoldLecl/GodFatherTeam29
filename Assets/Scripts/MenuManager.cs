using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Menu Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    void Start()
    {
        SetupUI();
        LoadBestScore();

    }

    private void SetupUI()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        if (titleText != null)
        {
            titleText.text = "Low Coast";
        }
    }

    private void LoadBestScore()
    {
        if (bestScoreText != null)
        {
            int bestScore = PlayerPrefs.GetInt("BestScore", 0);
            bestScoreText.text = $"Meilleur Score: {bestScore}";
        }
    }

    public void OnPlayClicked()
    {
        Debug.Log("Démarrage du jeu...");

        // Utiliser le GameManager si disponible, sinon charger directement
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OnQuitClicked()
    {
        Debug.Log("Fermeture du jeu...");

        // Utiliser le GameManager si disponible, sinon quitter directement
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }



    private System.Collections.IEnumerator ScaleAnimation(GameObject target, Vector3 targetScale, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 startScale = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            // Ease out back curve
            t = 1f - Mathf.Pow(1f - t, 3f);

            target.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        target.transform.localScale = targetScale;
    }

    void OnDestroy()
    {
        // Nettoyer les événements
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}