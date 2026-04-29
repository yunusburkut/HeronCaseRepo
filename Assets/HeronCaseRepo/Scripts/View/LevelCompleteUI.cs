using UnityEngine;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private GameStateMachine _stateMachine;

    public void Initialize(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
        _stateMachine.OnStateChanged += OnStateChanged;
    }

    private void OnDestroy()
    {
        if (_stateMachine != null)
        {
            _stateMachine.OnStateChanged -= OnStateChanged;
        }
    }

    private void OnStateChanged(GameState state)
    {
        if (state == GameState.LevelComplete)
        {
            panel.SetActive(true);
        }
    }
}
