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

    private GameStateMachine _stateMachine;
    private ILevelController _levelController;
    private WinConditionChecker _winChecker;
    private InputController _inputController;
    private readonly List<TubeView> _tubeViews = new List<TubeView>();

    public void Initialize(GameStateMachine stateMachine, ILevelController levelController,
        WinConditionChecker winChecker, InputController inputController)
    {
        _stateMachine = stateMachine;
        _levelController = levelController;
        _winChecker = winChecker;
        _inputController = inputController;
        _winChecker.OnLevelCompleted += OnLevelCompleted;
    }

    public void StartLevel()
    {
        SpawnLevel(levelData.tubes.Count > 0 ? levelData.tubes : LevelDataBuilder.Build(levelData));
        _stateMachine.Enter(GameState.Playing);
    }

    private void OnLevelCompleted()
    {
        _stateMachine.Enter(GameState.LevelComplete);
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        _inputController.UnregisterTubes();
        foreach (var tubeView in _tubeViews)
        {
            Destroy(tubeView.gameObject);
        }
        
        _tubeViews.Clear();

        SpawnLevel(LevelDataBuilder.Build(levelData, 0));
        _stateMachine.Enter(GameState.Playing);
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
        _winChecker.Initialize(_tubeViews);
        _inputController.RegisterTubes(_tubeViews);
    }

    private void OnDestroy()
    {
        if (_winChecker != null)
        {
            _winChecker.OnLevelCompleted -= OnLevelCompleted;
        }
        _inputController?.UnregisterTubes();
    }
}
