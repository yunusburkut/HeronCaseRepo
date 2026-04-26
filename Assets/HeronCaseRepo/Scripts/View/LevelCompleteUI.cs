using UnityEngine;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private GameObject panel;

    private ILevelController _levelController;

    private void Awake()
    {
        _levelController = gameController;
    }

    private void OnEnable()
    {
        _levelController.OnLevelCompleted += Show;
    }

    private void OnDisable()
    {
        _levelController.OnLevelCompleted -= Show;
    }

    private void Show()
    {
        panel.SetActive(true);
    }
}
