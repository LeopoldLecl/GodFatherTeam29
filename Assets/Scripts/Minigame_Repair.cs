using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minigame_Repair : Minigame
{
    [SerializeField]
    Slider pointSlider;

    [SerializeField]
    int fixToWin = 10;
    int holeFixed = 0;

    [SerializeField]
    float minHoleTimer = 1.5f;
    [SerializeField]
    float maxHoleTimer = 2.5f;
    private float actualHoleTimer = 0;

    [SerializeField]
    List<Hole> holeSpawnpoints = new();
    List<Hole> availableHoleSpawnpoints;

    private int holeCount;

    [SerializeField] Hammer hammer;

    public override void Init()
    {
        base.Init();

        holeFixed = 0;
        pointSlider.value = 0;
        actualHoleTimer = 0;
        holeCount = 0;
        availableHoleSpawnpoints = new(holeSpawnpoints);
        hammer.Activate();
    }

    private void Update()
    {
        if (isGameActive)
        {
            actualHoleTimer -= Time.deltaTime;
            if (actualHoleTimer <= 0)
            {
                actualHoleTimer = Random.Range(minHoleTimer, maxHoleTimer);
                NewHole();
            }
        }
        
    }

    private void NewHole()
    {
        holeCount++;
        if (availableHoleSpawnpoints.Count <= 0)
        {
            CompleteMinigame(false);
            return;
        }

        Hole hole = availableHoleSpawnpoints[Random.Range(0, availableHoleSpawnpoints.Count)];
        hole.Activate();
        hole.onHoleFixed += OnHoleFixed;
        availableHoleSpawnpoints.Remove(hole);
    }

    private void OnHoleFixed(Hole hole)
    {
        availableHoleSpawnpoints.Add(hole);
        holeFixed++;
        pointSlider.value = (float)holeFixed / fixToWin;
        if (holeFixed >= fixToWin)
        {
            CompleteMinigame(true);   
        }
        hole.onHoleFixed -= OnHoleFixed;
    }

    public override void Clear()
    {
        base.Clear();

        foreach (Hole holeInst in holeSpawnpoints)
        {
            holeInst.onHoleFixed -= OnHoleFixed;
            holeInst.Reset();
        }
        availableHoleSpawnpoints.Clear();
        hammer.Reset();
    }
}
