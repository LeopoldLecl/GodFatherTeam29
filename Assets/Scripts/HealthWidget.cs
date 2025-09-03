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

    private void UpdateOverlayPosition(int state)
    {
        gameObject.transform.position = Vector3.Lerp(basePosition, finalPosition, 1/(state + 1));
    }
}
