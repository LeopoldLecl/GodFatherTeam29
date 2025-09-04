using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [SerializeField] private float gameOverDelay = 2f;

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool gameOver = false;

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
    }

    void Start()
    {
        // S'abonner aux événements de santé
        Health.OnHealthUpdated += OnHealthChanged;
    }

    void OnDestroy()
    {
        Health.OnHealthUpdated -= OnHealthChanged;
    }


    private void OnHealthChanged(int currentHealth)
    {
        Debug.Log($"Santé restante: {currentHealth}");

        if (currentHealth <= 0 && !gameOver)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        Debug.Log("Game Over!");

        // Attendre un peu avant de retourner au menu
        Invoke(nameof(ReturnToMenu), gameOverDelay);
    }

    public void ReturnToMenu()
    {
        gameOver = false;
        Time.timeScale = 1f; // S'assurer que le temps est normal

        // Charger la scène du menu
        SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        gameOver = false;
        Time.timeScale = 1f;

        // Charger la scène de jeu
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    // Méthodes utilitaires
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}