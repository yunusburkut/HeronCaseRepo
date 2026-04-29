using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
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

    private ILevelController _levelController;
    private WinConditionChecker _winChecker;
    private readonly List<TubeView> _tubeViews = new List<TubeView>();

    public void Initialize(ILevelController levelController, WinConditionChecker winChecker)
    {
        _levelController = levelController;
        _winChecker = winChecker;
    }

    public void StartLevel()
    {
        ClearLevel();
        SpawnLevel(LevelDataBuilder.Build(levelData, 0));
    }

    public void ClearLevel()
    {
        foreach (var tubeView in _tubeViews)
        {
            Destroy(tubeView.gameObject);
        }
        
        _tubeViews.Clear();
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
            _tubeViews.Add(tubeView);
        }

        _levelController.Initialize(_tubeViews);
        _winChecker.Initialize(_tubeViews);//todo change this 
    }
}
