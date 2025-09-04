using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Minigame_Smoke : Minigame
{
    private Camera cam;

    [SerializeField]
    GameObject mask;

    [SerializeField]
    Slider timeSlider;

    [SerializeField]
    GameObject targetPrefab;
    private GameObject target;

    [SerializeField]
    List<GameObject> spawnpoints;

    [SerializeField]
    ParticleSystem particleSystem;

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

        target = Instantiate(targetPrefab, spawnpoints[Random.Range(0, spawnpoints.Count)].transform);

        actualCountdownTime = maxCountdownTime;
        timeSlider.value = 1;
        particleSystem.Play();
        SmokeTarget.onTargetClicked += () =>
        {
            Debug.Log("gg");
            CompleteMinigame(true);
        };
    }

    void Update()
    {
        if (isGameActive)
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

        if (!target)
        {
            Destroy(target);
        }

        particleSystem?.Pause();
        particleSystem?.Clear();
    }
}
