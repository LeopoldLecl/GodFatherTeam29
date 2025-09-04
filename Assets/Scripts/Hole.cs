using System;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particleSystem;

    [SerializeField]
    SpriteRenderer spriteRenderer;

    bool activated = false;

    public event Action<Hole> onHoleFixed;
    
    public void Activate()
    {
        activated = true;
        particleSystem.Play();
        spriteRenderer.enabled = true;
    }

    public void Reset()
    {
        activated = false;
        particleSystem.Stop();
        spriteRenderer.enabled = false;
    }

    private void OnMouseDown()
    {
        onHoleFixed?.Invoke(this);
        Reset();
    }
}
