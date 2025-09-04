using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Minigame_Smoke : Minigame
{
    [SerializeField]
    Camera cam;

    [SerializeField]
    GameObject mask;

    [SerializeField]
    Slider timeSlider;

    [SerializeField]
    SmokeTarget toFind;

    [SerializeField]
    float maxCountdownTime = 5f;
    float actualCountdownTime;

    void Start()
    {
        cam = FindFirstObjectByType<Camera>();
    }

    public override void Init()
    {
        base.Init();

        actualCountdownTime = maxCountdownTime;
        SmokeTarget.onTargetClicked += () =>
        {
            Debug.Log("gg");
            CompleteMinigame(true);
        };
    }

    void Update()
    {
        if (true)
        {
            Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            mask.transform.position = pos;

            UpdateTimer();
        }
    }

    private void UpdateTimer()
    {
        actualCountdownTime -= Time.deltaTime;
        timeSlider.value = actualCountdownTime / maxCountdownTime;
        if (actualCountdownTime <= 0)
        {
            Debug.Log("failed");
            CompleteMinigame(false);
        }
    }

    public override void Clear()
    {
        base.Clear();

    }
}
