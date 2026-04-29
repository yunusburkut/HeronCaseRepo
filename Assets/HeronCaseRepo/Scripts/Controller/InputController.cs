using HeronCaseRepo.Scripts.Controller;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private FlowController _flowController;
    private ILevelController _levelController;

    public void Initialize(FlowController flowController, ILevelController levelController)
    {
        _flowController = flowController;
        _levelController = levelController;
        EventBus<TubeClickedEvent>.Subscribe(OnTubeClicked);
    }

    private void OnDestroy()
    {
        EventBus<TubeClickedEvent>.Unsubscribe(OnTubeClicked);
    }

    private void OnTubeClicked(TubeClickedEvent e)
    {
        if (_flowController.Current != FlowState.Gameplay)
        {
            return;
        }

        _levelController.OnTubeClicked(e.Tube);
    }
}
