using System;
using UnityEngine;

public class Minigame : MonoBehaviour
{
    public static event Action<bool> OnMinigameCompleted;

    protected bool isGameActive = false;

    public virtual void Init()
    {
        // Init minigame
        isGameActive = true;
    }

    public virtual void Clear()
    {
        // Clear and stop minigame
        isGameActive = false;
    }

    protected void CompleteMinigame(bool success)
    {
        if (isGameActive)
        {
            isGameActive = false;
            OnMinigameCompleted?.Invoke(success);
        }
    }
}
