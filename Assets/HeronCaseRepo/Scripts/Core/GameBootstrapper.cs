using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private GameController gameController;
    [SerializeField] private LevelController levelController;
    [SerializeField] private InputController inputController;
    [SerializeField] private FlowController flowController;

    private WinConditionChecker _winChecker;

    private void Awake()
    {
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
