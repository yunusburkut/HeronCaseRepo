using UnityEngine;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject panel;

    private void OnEnable()
    {
        gameController.OnLevelCompleted += Show;
    }

    private void OnDisable()
    {
        gameController.OnLevelCompleted -= Show;
    }

    private void Show()
    {
        panel.SetActive(true);
    }
}
