using System.Collections.Generic;
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

    private ILevelController _levelController;
    private readonly List<TubeView> _tubeViews = new List<TubeView>();

    private void Awake()
    {
        _levelController = gameController;
    }

    private void Start()
    {
        _levelController.OnLevelCompleted += LoadNextLevel;
        SpawnLevel(levelData.tubes.Count > 0 ? levelData.tubes : LevelDataBuilder.Build(levelData));
    }

    private void OnDestroy()
    {
        _levelController.OnLevelCompleted -= LoadNextLevel;
        foreach (var tubeView in _tubeViews)
        {
            tubeView.OnClicked -= _levelController.OnTubeClicked;
        }
    }

    private void LoadNextLevel()
    {
        foreach (var tubeView in _tubeViews)
        {
            tubeView.OnClicked -= _levelController.OnTubeClicked;
            Destroy(tubeView.gameObject);
        }
        _tubeViews.Clear();

        SpawnLevel(LevelDataBuilder.Build(levelData, 0));
    }

    private void SpawnLevel(List<TubeData> tubeDataList)
    {
        var count = tubeDataList.Count;
        var startX = -(count - 1) * tubeSpacing * 0.5f;

        for (var i = 0; i < count; i++)
        {
            var pos = new Vector3(startX + i * tubeSpacing, 0f, 0f);
            var tubeView = Instantiate(tubePrefab, pos, Quaternion.identity, tubesContainer);
            tubeView.name = $"Tube_{i}";
            tubeView.Init(tubeDataList[i], waterPrefab, colorPalette);
            tubeView.OnClicked += _levelController.OnTubeClicked;
            _tubeViews.Add(tubeView);
        }

        _levelController.Initialize(_tubeViews);
    }
}
