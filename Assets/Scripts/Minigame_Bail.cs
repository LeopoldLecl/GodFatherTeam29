using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

#if UNITY_EDITOR
    public void Start()
    {
        Init();
    }
#endif

    public override void Init()
    {
        base.Init();

        IAClick.action.started += UpdateSpam;
        onGoodKeyPressed += OnGoodKeyPressed;
        currentDirection = Direction.RIGHT;
        ActivateIcon(currentDirection);
        actualCountdownTime = maxCountdownTime;
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
            Debug.Log("win");
            CompleteMinigame(false);
        }
    }

    private void UpdateSpam(InputAction.CallbackContext ctx)
    {
        float intDirection = ctx.ReadValue<float>();

        if (currentDirection != (Direction)intDirection)
        {
            if (Direction.LEFT == currentDirection)
            {
                currentDirection = Direction.RIGHT;
            }
            else
            {
                currentDirection = Direction.LEFT; 
            }

            onGoodKeyPressed.Invoke();
            ActivateIcon(currentDirection);
        }
    }

    private void ActivateIcon(Direction direction)
    {
        if (Direction.LEFT == direction)
        {
            rightIcon.transform.localScale = new Vector3(1.5f, 1.5f);
            leftIcon.transform.localScale = new Vector3(1f, 1f);
        }
        else
        {
            rightIcon.transform.localScale = new Vector3(1f, 1f);
            leftIcon.transform.localScale = new Vector3(1.5f, 1.5f);
        }
    }

    private void OnGoodKeyPressed()
    {
        actualPress++;
        pointSlider.value = (float)actualPress / (float)pressToWin;
        if (actualPress >= pressToWin)
        {
            Debug.Log("gg");
            CompleteMinigame(true);
        }
    }

    public override void Clear()
    {
        base.Clear();

        IAClick.action.RemoveAction();
    }
}
