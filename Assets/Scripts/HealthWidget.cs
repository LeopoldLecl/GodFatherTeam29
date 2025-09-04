using UnityEngine;
using UnityEngine.UI;

public class HealthWidget : MonoBehaviour
{
    [SerializeField] private Vector3 basePosition;
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private int maxHealth = 3;

    void OnEnable()
    {
        Health.OnHealthUpdated += UpdateOverlayPosition;
        UpdateOverlayPosition(maxHealth);
    }

    void OnDisable()
    {
        Health.OnHealthUpdated -= UpdateOverlayPosition;
    }

    private void UpdateOverlayPosition(int currentHealth)
    {
        float t = maxHealth > 0 ? Mathf.Clamp01((float)currentHealth / maxHealth) : 0f;
        transform.position = Vector3.Lerp(finalPosition, basePosition, t);
    }
}
