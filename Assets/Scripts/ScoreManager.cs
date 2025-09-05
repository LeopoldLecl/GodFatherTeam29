using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreText;

    [Header("Score Settings")]
    [SerializeField] private int pointsPerSuccess = 100;

    private int currentScore = 0;
    private int bestScore = 0;

    void Start()
    {
        bestScore = PlayerPrefs.GetInt("BestScore", 0);

        Health.OnHealthUpdated += OnHealthUpdated;
        MinigameManager.OnMinigameEnded += OnMinigameEnded;

        UpdateUI();
    }

    void OnDestroy()
    {
        Health.OnHealthUpdated -= OnHealthUpdated;
        MinigameManager.OnMinigameEnded -= OnMinigameEnded;
    }


    private void OnMinigameEnded(bool success)
    {
        if (success)
        {
            AddScore(pointsPerSuccess);
        }
    }

    private void OnHealthUpdated(int health, int maxHealth)
    {
        if (health <= 0)
        {
            SaveFinalScore();
        }
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateUI();

        Debug.Log($"Score actuel: {currentScore} (+{points})");
    }

    private void SaveFinalScore()
    {
        if (currentScore > bestScore)
        {
            bestScore = currentScore;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();

            Debug.Log($" NOUVEAU RECORD! Score: {currentScore}");

            if (bestScoreText != null)
            {
                StartCoroutine(NewRecordAnimation());
            }
        }
        else
        {
            Debug.Log($"Score final: {currentScore} (Record: {bestScore})");
        }
    }

    private void UpdateUI()
    {
        if (currentScoreText != null)
        {
            currentScoreText.text = $"Score: {currentScore}";
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = $"Record: {bestScore}";
        }
    }

    private System.Collections.IEnumerator NewRecordAnimation()
    {
        if (bestScoreText == null) yield break;

        Vector3 originalScale = bestScoreText.transform.localScale;
        Color originalColor = bestScoreText.color;

        for (int i = 0; i < 3; i++)
        {
            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                bestScoreText.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, t);
                bestScoreText.color = Color.Lerp(originalColor, Color.yellow, t);

                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                bestScoreText.transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t);
                bestScoreText.color = Color.Lerp(Color.yellow, originalColor, t);

                yield return null;
            }
        }

        bestScoreText.transform.localScale = originalScale;
        bestScoreText.color = originalColor;
    }

    public int GetCurrentScore() => currentScore;
    public int GetBestScore() => bestScore;

    public void ResetScore()
    {
        currentScore = 0;
        UpdateUI();
    }
}