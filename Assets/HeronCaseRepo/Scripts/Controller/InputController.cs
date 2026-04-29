using UnityEngine;

public class InputController : MonoBehaviour
{
    private GameStateMachine _stateMachine;
    private ILevelController _levelController;

    public void Initialize(GameStateMachine stateMachine, ILevelController levelController)
    {
        _stateMachine = stateMachine;
        _levelController = levelController;
        EventBus<TubeClickedEvent>.Subscribe(OnTubeClicked);
    }

    private void OnDestroy()
    {
        EventBus<TubeClickedEvent>.Unsubscribe(OnTubeClicked);
    }

    private void OnTubeClicked(TubeClickedEvent e)
    {
        if (_stateMachine.Current != GameState.Playing)
        {
            return;
        }
        
        _levelController.OnTubeClicked(e.Tube);
    }
}
