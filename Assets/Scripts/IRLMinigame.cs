using UnityEngine;
using UnityEngine.InputSystem;

public class IRLMinigame : Minigame
{
    [SerializeField]
    InputActionReference IA_Win;

    [SerializeField]
    InputActionReference IA_Fail;

    public override void Init()
    {
        base.Init();

        IA_Win.action.started += (ctx) => {
            CompleteMinigame(true); 
        };

        IA_Fail.action.started += (ctx) => {
            CompleteMinigame(false);
        };
    }

    public override void Clear()
    {
        base.Clear();

        IA_Win.action.started -= (ctx) => {
            CompleteMinigame(true);
        };

        IA_Fail.action.started -= (ctx) => {
            CompleteMinigame(false);
        };
    }

    void OnDestroy()
    {
        IA_Win.action.started -= (ctx) => {
            CompleteMinigame(true);
        };

        IA_Fail.action.started -= (ctx) => {
            CompleteMinigame(false);
        };
    }
}
