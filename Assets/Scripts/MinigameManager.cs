using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private List<Minigame> minigames = new();
    [SerializeField] private List<Minigame> minigamePool = new();
    [SerializeField] private float gameTimer;
    private float currentGameTimer;

    private Minigame currentMinigame;
    private bool waitingForMinigameResult = false;
    private bool inTransition = false;

    [SerializeField] GameObject pivotPhare;
    [SerializeField] Vector3 endScale;
    [SerializeField] Vector3 startScale;

    [SerializeField] GameObject winPrefab;

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
        pivotPhare.transform.localScale = Vector3.Lerp(endScale, startScale, currentGameTimer / gameTimer);
        if (currentGameTimer <= 0)
        {
            FindFirstObjectByType<Health>().SetHealth(0);
            Instantiate(winPrefab);
        }
    }

    void OnDestroy()
    {
        Minigame.OnMinigameCompleted -= OnMinigameCompleted;
    }

    void InitFirstMinigame()
    {
        currentMinigame = minigames[0];
        minigamePool.Add(currentMinigame); // Add it to the pool and
        minigames.Remove(currentMinigame); // remove it from the main list

        currentMinigame.Init();
        waitingForMinigameResult = true;
    }

    void LoadNextMinigameImmediate()
    {
        if (!health.IsGameRunning() || waitingForMinigameResult) return;

        if (minigamePool.Count > 0) minigames.Add(minigamePool[0]); // If pool is not empty, add it to main list
        minigamePool.RemoveAt(0);
        currentMinigame = minigames[Random.Range(0, minigames.Count)]; // Choose a mg
        minigamePool.Add(currentMinigame); // Add it to the pool and
        minigames.Remove(currentMinigame); // remove it from the main list

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
