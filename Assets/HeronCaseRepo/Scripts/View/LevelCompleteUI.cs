using UnityEngine;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void OnEnable() => EventBus<GameStateChangedEvent>.Subscribe(OnStateChanged);
    private void OnDisable() => EventBus<GameStateChangedEvent>.Unsubscribe(OnStateChanged);

    private void OnStateChanged(GameStateChangedEvent e)
    {
        if (e.State == GameState.LevelComplete)
        {
            panel.SetActive(true);
        }
    }
}
