using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Hammer : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer hammerSpriteRenderer;

    [SerializeField]
    SpriteRenderer markSpriteRenderer;

    [SerializeField]
    GameObject hammerPivot;

    [SerializeField] Vector3 baseRotation;
    [SerializeField] Vector3 hitRotation;

    [SerializeField] InputActionReference IAClick;

    bool activated;
    Camera cam;

    private void Start()
    {
        cam = FindFirstObjectByType<Camera>();
        Reset();
        IAClick.action.started += (ctx) => { ShowClickMark(true); };
        IAClick.action.canceled += (ctx) => { ShowClickMark(false); };
    }

    public void Activate()
    {
        activated = true;
        hammerSpriteRenderer.enabled = true;
    }

    public void Reset()
    {
        activated = false;
        hammerSpriteRenderer.enabled = false;
    }

    public void ShowClickMark(bool show)
    {
        if (show)
        {
            markSpriteRenderer.enabled = true;
            transform.rotation = Quaternion.Euler(hitRotation);
        }
        else
        {
            markSpriteRenderer.enabled = false;
            transform.rotation = Quaternion.Euler(baseRotation);
        }
    }

    void Update()
    {
        if (activated)
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            hammerPivot.transform.position = pos;
        }
    }
}
