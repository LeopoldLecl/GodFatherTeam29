using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HealthWidget : MonoBehaviour
{
    [SerializeField] private Vector3 basePosition;
    [SerializeField] private Vector3 finalPosition;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject pivot;

    void OnEnable()
    {
        Health.OnHealthUpdated += UpdateOverlayPosition;

        Sequence yTransformSequence = DOTween.Sequence();
        yTransformSequence
            .Append(transform.DOLocalMoveY(1f, 1f).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveY(-1f, 1f).SetEase(Ease.InOutSine))
            .SetLoops(-1);

        Sequence xTransformSequence = DOTween.Sequence();
        xTransformSequence
            .Append(transform.DOLocalMoveX(0.5f, 2.4f).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveX(-0.5f, 2.4f).SetEase(Ease.InOutSine))
            .SetLoops(-1);

        Sequence rotateSequence = DOTween.Sequence();
        rotateSequence
            .Append(transform.DORotate(new Vector3(0, 0, 3), 1.4f).SetEase(Ease.InOutSine))
            .Append(transform.DORotate(new Vector3(0, 0, -3), 1.4f).SetEase(Ease.InOutSine))
            .SetLoops(-1);

        yTransformSequence.Play();
        xTransformSequence.Play();
        rotateSequence.Play();

        UpdateOverlayPosition(maxHealth);
    }

    void OnDisable()
    {
        Health.OnHealthUpdated -= UpdateOverlayPosition;
        pivot.transform.DOLocalMove(basePosition);
    }

    private void UpdateOverlayPosition(int currentHealth)
    {
        float t = maxHealth > 0 ? Mathf.Clamp01((float)currentHealth / maxHealth) : 0f;
        pivot.transform.DOLocalMove(Vector3.Lerp(finalPosition, basePosition, (float)currentHealth / maxHealth), 1f);
    }
}
