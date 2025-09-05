using System;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private List<Minigame> minigames = new();
    [SerializeField] private float gameTimer;
    private float currentGameTimer;

    private Minigame currentMinigame;
    private int currentIndex = 0;
    private bool waitingForMinigameResult = false;
    private bool inTransition = false;

    public static event Action<bool> OnMinigameEnded;

    void Start()
    {
        Minigame.OnMinigameCompleted += OnMinigameCompleted;
        currentGameTimer = gameTimer;
        foreach (Minigame m in minigames) m.gameObject.SetActive(false);
        InitFirstMinigame();
    }

    void Update()
    {
        if (!health.IsGameRunning() || inTransition) return;
        currentGameTimer -= Time.deltaTime;
        if (currentGameTimer <= 0) FindFirstObjectByType<Health>().SetHealth(0);
    }

    void OnDestroy()
    {
        Minigame.OnMinigameCompleted -= OnMinigameCompleted;
    }

    void InitFirstMinigame()
    {
        currentMinigame = minigames[currentIndex];
        currentMinigame.Init();
        waitingForMinigameResult = true;
    }

    void LoadNextMinigameImmediate()
    {
        if (!health.IsGameRunning() || waitingForMinigameResult) return;
        if (++currentIndex > minigames.Count - 1) currentIndex = 0;
        currentMinigame = minigames[currentIndex];
        currentMinigame.Init();
        waitingForMinigameResult = true;
    }

    private void OnMinigameCompleted(bool success)
    {
        if (!waitingForMinigameResult || inTransition) return;
        waitingForMinigameResult = false;
        OnMinigameEnded?.Invoke(success);
        StartCoroutine(TransitionAndLoadNext());
    }

    private System.Collections.IEnumerator TransitionAndLoadNext()
    {
        inTransition = true;

        if (currentMinigame != null) currentMinigame.Clear();

        if (SceneTransition.Instance != null)
            yield return SceneTransition.Instance.FadeOutRoutine();

        yield return new WaitForSeconds(1f);

        if (!health.IsGameRunning())
        {
            inTransition = false;
            yield break;
        }

        LoadNextMinigameImmediate();

        if (SceneTransition.Instance != null)
            yield return SceneTransition.Instance.FadeInRoutine();

        inTransition = false;
    }
}
