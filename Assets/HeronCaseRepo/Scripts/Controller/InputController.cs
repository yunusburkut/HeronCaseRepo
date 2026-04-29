using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private GameStateMachine _stateMachine;
    private ILevelController _levelController;
    private List<TubeView> _registeredTubes;

    public void Initialize(GameStateMachine stateMachine, ILevelController levelController)
    {
        _stateMachine = stateMachine;
        _levelController = levelController;
    }

    public void RegisterTubes(List<TubeView> tubes)
    {
        UnregisterTubes();
        _registeredTubes = new List<TubeView>(tubes);
        foreach (var tube in _registeredTubes)
        {
            tube.OnClicked += HandleTubeClicked;
        }
    }

    public void UnregisterTubes()
    {
        if (_registeredTubes == null) return;
        
        foreach (var tube in _registeredTubes)
        {
            tube.OnClicked -= HandleTubeClicked;
        }
        _registeredTubes = null;
    }

    private void HandleTubeClicked(TubeView tube)
    {
        if (_stateMachine.Current != GameState.Playing) return;
        {
            _levelController.OnTubeClicked(tube);
        }
    }
}
