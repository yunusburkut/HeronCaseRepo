using DG.Tweening;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GameController gameController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private InputController inputController;
    [SerializeField] private FlowController flowController;

    [Header("Performance")]
    [SerializeField] private int targetFrameRate = 60;

    private WinConditionChecker _winChecker;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        
        Debug.Assert(gameController != null, "[GameBootstrapper] gameController is not assigned.");
        Debug.Assert(levelController != null, "[GameBootstrapper] levelController is not assigned.");
        Debug.Assert(inputController != null, "[GameBootstrapper] inputController is not assigned.");
        Debug.Assert(flowController != null, "[GameBootstrapper] flowController is not assigned.");

        _winChecker = new WinConditionChecker();

        levelController.Initialize(gameController, _winChecker);
        flowController.Initialize(levelController);
        inputController.Initialize(flowController, gameController);
    }

    private void OnDestroy()
    {
        _winChecker.Dispose();
    }
}
