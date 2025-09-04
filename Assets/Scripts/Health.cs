using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    private int health;

    // Unifie l'événement pour être compatible avec ScoreManager et GameManager
    public static event Action<int> OnHealthUpdated;

    void Start()
    {
        ResetHealth();
        MinigameManager.OnMinigameEnded += OnMinigameEnded;
    }

    void OnDestroy()
    {
        MinigameManager.OnMinigameEnded -= OnMinigameEnded;
    }

    private void OnMinigameEnded(bool success)
    {
        if (!success)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        SetHealth(health - amount);
    }

    public void SetHealth(int newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
        OnHealthUpdated?.Invoke(health);

        if (health <= 0)
        {
            Debug.Log("Santé épuisée -> Fin du jeu");
        }
    }

    public void ResetHealth()
    {
        SetHealth(maxHealth);
    }

    public bool IsGameRunning() => health > 0;
}
