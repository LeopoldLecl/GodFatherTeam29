using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float gameOverDelay = 2f;

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool gameOver = false;

    void Awake()
    {
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
        Health.OnHealthUpdated += OnHealthChanged;
    }

    void OnDestroy()
    {
        Health.OnHealthUpdated -= OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth)
    {
        if (currentHealth <= 0 && !gameOver)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        if (gameOver) return;

        gameOver = true;
        Invoke(nameof(ReturnToMenu), gameOverDelay);
    }

    public void ReturnToMenu()
    {
        gameOver = false;
        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void StartGame()
    {
        gameOver = false;
        Time.timeScale = 1f;

        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainScene");
        else
            SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}
