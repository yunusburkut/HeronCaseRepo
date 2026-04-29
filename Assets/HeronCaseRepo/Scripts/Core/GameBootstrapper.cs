using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GameController gameController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private InputController inputController;
    [SerializeField] private LevelCompleteUI levelCompleteUI;

    private GameStateMachine _stateMachine;
    private WinConditionChecker _winChecker;

    private void Awake()
    {
        _stateMachine = new GameStateMachine();
        _winChecker = new WinConditionChecker();

        gameController.OnPourCompleted += _winChecker.OnPourCompleted;
        inputController.Initialize(_stateMachine, gameController);
        levelController.Initialize(_stateMachine, gameController, _winChecker, inputController);
        levelCompleteUI.Initialize(_stateMachine);
    }

    private void Start()
    {
        levelController.StartLevel();
    }
}
