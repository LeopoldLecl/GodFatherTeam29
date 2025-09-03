using System;
using UnityEngine;
using UnityEngine.InputSystem;

enum Direction
{
    LEFT = -1,
    RIGHT = 1
}

public class BailMinigame : Minigame
{
    [SerializeField]
    InputActionReference IAClick;

    [SerializeField]
    GameObject leftIcon;

    [SerializeField]
    GameObject rightIcon;

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
        // do something
    }

    public override void Clear()
    {
        base.Clear();

        IAClick.action.RemoveAction();
    }
}
