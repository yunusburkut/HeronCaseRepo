using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GameController gameController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private InputController inputController;

    private GameStateMachine _stateMachine;
    private WinConditionChecker _winChecker;

    private void Awake()
    {
        _stateMachine = new GameStateMachine();
        _winChecker = new WinConditionChecker();

        inputController.Initialize(_stateMachine, gameController);
        levelController.Initialize(_stateMachine, gameController, _winChecker);
    }

    private void Start()
    {
        levelController.StartLevel();
    }

    private void OnDestroy()
    {
        _winChecker.Dispose();
    }
}
