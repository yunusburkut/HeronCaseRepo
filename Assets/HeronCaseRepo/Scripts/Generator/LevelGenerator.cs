using System.Collections.Generic;
using HeronCaseRepo.Scripts.Data;
using UnityEngine;

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
        var tubeDataList = data.tubes.Count > 0 ? data.tubes : LevelDataBuilder.Build(data);
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
}
