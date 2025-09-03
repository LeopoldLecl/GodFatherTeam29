using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class MinigameManager : MonoBehaviour
{
    [SerializeField]
    private Health health;

    [SerializeField]
    private List<Minigame> minigames = new();

    [SerializeField]
    private InputActionReference IANextMinigame;

    [SerializeField]
    private float gameTimer;
    private float currentGameTimer;

    private Minigame currentMinigame;
    private int currentIndex = 0;
    private bool waitingForMinigameResult = false;

    public static event Action<bool> OnMinigameEnded;

    void Start()
    {
        IANextMinigame.action.started += LoadNextMinigame;
        Minigame.OnMinigameCompleted += OnMinigameCompleted;

        currentGameTimer = gameTimer;

        foreach (Minigame minigame in minigames)
        {
            minigame.gameObject.SetActive(false);
        }

        InitFirstMinigame();
    }

    private void Update()
    {
        if (!health.IsGameRunning())
        {
            return;
        }
 
        currentGameTimer -= Time.deltaTime;
        if (currentGameTimer <= 0)
        {
            FindFirstObjectByType<Health>().SetHealth(0);
        }
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

    void LoadNextMinigame(InputAction.CallbackContext ctx)
    {
        if (!health.IsGameRunning())
        {
            return;
        }

        if (waitingForMinigameResult)
        {
            return;
        }

        if (++currentIndex > minigames.Count - 1)
        {
            currentIndex = 0;
        }

        currentMinigame.Clear(); // Clear directly when minigame end? 
        /// <see cref="Minigame.CompleteMinigame(bool)"/>

        currentMinigame = minigames[currentIndex];

        currentMinigame.Init();
        waitingForMinigameResult = true;
    }

    private void OnMinigameCompleted(bool success)
    {
        if (waitingForMinigameResult)
        {
            waitingForMinigameResult = false;
            OnMinigameEnded?.Invoke(success);

            Invoke("AutoLoadNextMinigame", 2f);
        }
    }

    private void AutoLoadNextMinigame()
    {
        if (health.IsGameRunning())
        {
            LoadNextMinigame(new InputAction.CallbackContext());
        }
    }
}