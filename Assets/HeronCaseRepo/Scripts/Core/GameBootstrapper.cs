using DG.Tweening;
using HeronCaseRepo.Scripts.Services;
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
    private DeadlockChecker _deadlockChecker;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        
        Debug.Assert(gameController != null, "[GameBootstrapper] gameController is not assigned.");
        Debug.Assert(levelController != null, "[GameBootstrapper] levelController is not assigned.");
        Debug.Assert(inputController != null, "[GameBootstrapper] inputController is not assigned.");
        Debug.Assert(flowController != null, "[GameBootstrapper] flowController is not assigned.");

        _winChecker = new WinConditionChecker();
        _deadlockChecker = new DeadlockChecker();
        
        levelController.Initialize(gameController, _winChecker, _deadlockChecker);
        flowController.Initialize(levelController);
        inputController.Initialize(flowController, gameController);
    }

    private void OnDestroy()
    {
        _winChecker.Dispose();
    }
}
