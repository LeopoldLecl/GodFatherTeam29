using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthWidget : MonoBehaviour
{
    [SerializeField]
    Vector3 basePosition;

    [SerializeField]
    Vector3 finalPosition;

    void Start()
    {
        Health.OnHealthUpdated += UpdateOverlayPosition;
    }

    private void UpdateOverlayPosition(int currentHealth, int maxHealth)
    {
        gameObject.transform.position = Vector3.Lerp(finalPosition, basePosition, (float)currentHealth / maxHealth);
    }
}
