using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmokeTarget : MonoBehaviour
{
    public static event Action onTargetClicked;

    public void OnMouseDown()
    {
        onTargetClicked?.Invoke();
    }
}
