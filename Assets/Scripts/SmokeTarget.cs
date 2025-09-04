using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SmokeTarget : MonoBehaviour, IPointerDownHandler
{
    public static event Action onTargetClicked;
    public void OnPointerDown(PointerEventData eventData)
    {
        onTargetClicked.Invoke();
    }
}
