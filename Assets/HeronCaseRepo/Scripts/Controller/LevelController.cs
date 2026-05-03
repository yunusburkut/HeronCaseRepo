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
    [SerializeField] private float rowSpacing = 10f;
    [SerializeField] private int tubesPerRow = 4;
    [SerializeField] private Transform tubesContainer;
    [SerializeField] private GameSettings settings;

    private ITubeInteractionController _levelController;
    private WinConditionChecker _winChecker;
    private readonly List<TubeView> _tubeViews = new List<TubeView>();

    public void Initialize(ITubeInteractionController levelController, WinConditionChecker winChecker)
    {
        _levelController = levelController;
        _winChecker = winChecker;
    }

    public void StartLevel()
    {
        ClearLevel();
        SpawnLevel(levelData.Tubes.Count > 0 ? levelData.Tubes : LevelDataBuilder.Build(levelData, 0));
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
        var rowCount = Mathf.CeilToInt((float)count / tubesPerRow);
        var animIndex = 0;

        var cam = Camera.main;
        var camPos = cam.transform.position;
        var layoutOriginY = camPos.y + (rowCount - 1) * rowSpacing * 0.5f;

        for (var row = 0; row < rowCount; row++)
        {
            var rowStart = row * tubesPerRow;
            var rowSize = Mathf.Min(tubesPerRow, count - rowStart);
            var startX = camPos.x - (rowSize - 1) * tubeSpacing * 0.5f;
            var y = layoutOriginY - row * rowSpacing;

            for (var col = 0; col < rowSize; col++)
            {
                var i = rowStart + col;
                var pos = new Vector3(startX + col * tubeSpacing, y, 0);
                var tubeView = Instantiate(tubePrefab, pos, Quaternion.identity, tubesContainer);
                tubeView.name = $"Tube_{i}";
                tubeView.Init(tubeDataList[i], waterPrefab, colorPalette);
                tubeView.PlayEnterAnimation(animIndex * settings.EnterStaggerDelay);
                _tubeViews.Add(tubeView);
                animIndex++;
            }
        }

        _levelController.Initialize(_tubeViews);
        _winChecker.Initialize(_tubeViews);
    }
}
