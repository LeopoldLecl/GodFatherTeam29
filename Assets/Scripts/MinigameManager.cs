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

    public static event Action<bool> OnMinigameEnded;

    void Start()
    {
        IANextMinigame.action.started += LoadNextMinigame;
        foreach(Minigame minigame in minigames)
        {
            minigame.gameObject.SetActive(false);
        }

        InitFirstMinigame();
    }

    void InitFirstMinigame()
    {
        currentMinigame = minigames[currentIndex];
        currentMinigame.Init();
        currentMinigame.gameObject.SetActive(true);
    }

    void LoadNextMinigame(InputAction.CallbackContext ctx)
    {
        if (health.health <= 0)
        {
            return;
        }

        if (++currentIndex > minigames.Count - 1) {
            currentIndex = 0;
        }
        currentMinigame.gameObject.SetActive(false);
        currentMinigame.Clear();

        currentMinigame = minigames[currentIndex];

        currentMinigame.Init();
        currentMinigame.gameObject.SetActive(true);

        OnMinigameEnded?.Invoke(Random.value < 0.5);
    }
}
