using System;
using System.Collections.Generic;
using HeronCaseRepo.Scripts.Data;
using UnityEngine;
using Random = System.Random;

public class LevelGenerator : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelData levelData;
    [SerializeField] private WaterColorPalette colorPalette;

    [Header("Prefabs")]
    [SerializeField] private TubeView tubePrefab;
    [SerializeField] private WaterView waterPrefab;

    [Header("Layout")]
    [SerializeField] private float tubeSpacing = 1.5f;
    [SerializeField] private Transform tubesContainer;

    [Header("Controller")]
    [SerializeField] private GameController gameController;

    private readonly List<TubeView> _tubeViews = new List<TubeView>();

    private void Start()
    {
        GenerateLevel(levelData);
    }

    private void GenerateLevel(LevelData data)
    {
        var tubeDataList = data.tubes.Count > 0 ? data.tubes : BuildTubeData(data);
        var count = tubeDataList.Count;
        var startX = -(count - 1) * tubeSpacing * 0.5f;

        for (var i = 0; i < count; i++)
        {
            var pos = new Vector3(startX + i * tubeSpacing, 0f, 0f);
            var tubeView = Instantiate(tubePrefab, pos, Quaternion.identity, tubesContainer);
            tubeView.name = $"Tube_{i}";
            tubeView.Init(tubeDataList[i], waterPrefab, colorPalette);
            tubeView.OnClicked += gameController.OnTubeClicked;
            _tubeViews.Add(tubeView);
        }

        gameController.Initialize(_tubeViews);
    }

    public static List<TubeData> BuildTubeData(LevelData data)
    {
        var pool = new List<WaterEntry>(data.colors.Count * data.tubeCapacity);
        foreach (var color in data.colors)
        {
            for (var i = 0; i < data.tubeCapacity; i++)
            {
                pool.Add(new WaterEntry { color = color, modifier = WaterModifier.None });
            }
        }

        var rng = new Random(data.seed != 0 ? data.seed : Environment.TickCount);
        for (var i = pool.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        var tubes = new List<TubeData>(data.colors.Count + data.emptyTubeCount);
        for (var i = 0; i < data.colors.Count; i++)
        {
            var tube = new TubeData { capacity = data.tubeCapacity };
            for (var j = 0; j < data.tubeCapacity; j++)
            {
                tube.waters.Add(pool[i * data.tubeCapacity + j]);
            }
            tubes.Add(tube);
        }

        for (var i = 0; i < data.emptyTubeCount; i++)
        {
            tubes.Add(new TubeData { capacity = data.tubeCapacity });
        }

        return tubes;
    }
}
