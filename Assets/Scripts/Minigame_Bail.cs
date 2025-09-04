using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

enum Direction
{
    LEFT = -1,
    RIGHT = 1
}

public class Minigame_Bail : Minigame
{
    [SerializeField]
    InputActionReference IAClick;

    [SerializeField]
    GameObject leftIcon;
    [SerializeField]
    GameObject rightIcon;
    [SerializeField]
    Slider timeSlider;
    [SerializeField]
    Slider pointSlider;

    [SerializeField]
    int pressToWin = 10;
    int actualPress;

    [SerializeField]
    float maxCountdownTime = 5f;
    float actualCountdownTime;

    public static event Action onGoodKeyPressed;

    Direction currentDirection = Direction.RIGHT;

    public override void Init()
    {
        base.Init();

        IAClick.action.started += OnKeyPressed;
        onGoodKeyPressed += OnGoodKeyPressed;
        currentDirection = Direction.RIGHT;
        ActivateIcon(currentDirection);
        actualCountdownTime = maxCountdownTime;
        pointSlider.value = 0;
        timeSlider.value = 1;
    }

    private void Update()
    {
        if (isGameActive)
        {
            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        actualCountdownTime -= Time.deltaTime;
        timeSlider.value = actualCountdownTime / maxCountdownTime;
        if (actualCountdownTime <= 0)
        {
            IAClick.action.started -= OnKeyPressed; // Should be cleared when parent minigame end
            Debug.Log("failed");
            CompleteMinigame(false);
        }
    }

    private void OnKeyPressed(InputAction.CallbackContext ctx)
    {
        float intDirection = ctx.ReadValue<float>();
        if (currentDirection != (Direction)intDirection)
        {
            onGoodKeyPressed.Invoke();
            ActivateIcon(currentDirection);
        }
    }

    private void ActivateIcon(Direction direction)
    {
        if (Direction.LEFT == direction)
        {
            rightIcon.transform.DOScale(1.5f, 1f).SetEase(Ease.OutElastic);
            leftIcon.transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
        }
        else
        {
            rightIcon.transform.DOScale(1f, 1f).SetEase(Ease.OutBounce);
            leftIcon.transform.DOScale(1.5f, 1f).SetEase(Ease.OutElastic);
        }
    }

    private void OnGoodKeyPressed()
    {
        actualPress++;

        if (Direction.LEFT == currentDirection)
        {
            currentDirection = Direction.RIGHT;
        }
        else
        {
            currentDirection = Direction.LEFT;
        }
        pointSlider.value = (float)actualPress / (float)pressToWin;

        if (actualPress >= pressToWin)
        {
            Debug.Log("gg");
            IAClick.action.started -= OnKeyPressed;
            CompleteMinigame(true);
        }
    }

    public override void Clear()
    {
        base.Clear();

        IAClick.action.started -= OnKeyPressed;
    }
}
