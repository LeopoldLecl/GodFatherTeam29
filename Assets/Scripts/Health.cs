using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 3;

    public static event Action<int> OnHealthUpdated;

    void Start()
    {
        MinigameManager.OnMinigameEnded += OnMinigameEnded;
    }

    void OnMinigameEnded(bool success)
    {
        if (!success)
        {
            Debug.Log("Failed");
            health--;
            if (health == 0)
            {
                Debug.Log("womp womp");
            }
            OnHealthUpdated?.Invoke(health);
            return;
        }
        Debug.Log("Success");
    }
}
