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

    private Minigame currentMinigame;
    private int currentIndex = 0;
    private bool waitingForMinigameResult = false;

    public static event Action<bool> OnMinigameEnded;

    void Start()
    {
        IANextMinigame.action.started += LoadNextMinigame;

        Minigame.OnMinigameCompleted += OnMinigameCompleted;

        foreach (Minigame minigame in minigames)
        {
            minigame.gameObject.SetActive(false);
        }

        InitFirstMinigame();
    }

    void OnDestroy()
    {
        Minigame.OnMinigameCompleted -= OnMinigameCompleted;
    }

    void InitFirstMinigame()
    {
        currentMinigame = minigames[currentIndex];
        currentMinigame.Init();
        currentMinigame.gameObject.SetActive(true);
        waitingForMinigameResult = true;
    }

    void LoadNextMinigame(InputAction.CallbackContext ctx)
    {
        if (health.health <= 0)
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

        currentMinigame.gameObject.SetActive(false);
        currentMinigame.Clear();

        currentMinigame = minigames[currentIndex];

        currentMinigame.Init();
        currentMinigame.gameObject.SetActive(true);
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
        if (health.health > 0)
        {
            LoadNextMinigame(new InputAction.CallbackContext());
        }
    }
}