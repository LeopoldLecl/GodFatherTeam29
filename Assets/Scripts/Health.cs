using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    int maxHealth = 3;

    [SerializeField, Tooltip("Read only")]
    int health = 3;

    public static event Action<int, int> OnHealthUpdated;

    void Start()
    {
        SetHealth(maxHealth);
        MinigameManager.OnMinigameEnded += OnMinigameEnded;
    }

    void OnMinigameEnded(bool success)
    {
        if (!success)
        {
            SetHealth(--health);
            if (health == 0)
            {
                Debug.Log("Game ended");
            }
            return;
        }
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        OnHealthUpdated?.Invoke(health, maxHealth);
    }

    public bool IsGameRunning() => health > 0;
}
