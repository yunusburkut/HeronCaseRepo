using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, ILevelController
{
    [Header("Settings")]
    [SerializeField] private GameSettings settings;

    private TubeView _selectedTube;
    private PourCoordinator _pourCoordinator;

    private void OnEnable() => EventBus<ShakeCompletedEvent>.Subscribe(OnShakeComplete);
    private void OnDisable() => EventBus<ShakeCompletedEvent>.Unsubscribe(OnShakeComplete);
    private void OnDestroy() => _pourCoordinator?.Dispose();

    public void Initialize(List<TubeView> tubes)
    {
        _selectedTube = null;
        _pourCoordinator?.Dispose();
        _pourCoordinator = new PourCoordinator(settings.QueuedPourSpeedMultiplier);
    }

    public void OnTubeClicked(TubeView tube)
    {
        if (_pourCoordinator.IsLocked(tube)) return;

        if (_selectedTube == null)
        {
            if (tube.IsEmpty || tube.IsSolved || _pourCoordinator.HasActivePour(tube)) return;

            _selectedTube = tube;
            tube.SetSelected(true);
            return;
        }

        if (_selectedTube == tube)
        {
            tube.SetSelected(false);
            _selectedTube = null;
            return;
        }

        if (_pourCoordinator.HasActivePour(tube))
        {
            _pourCoordinator.TryQueuePour(_selectedTube, tube);
            _selectedTube.SetSelected(false);
            _selectedTube = null;
            return;
        }

        if (!MoveValidator.CanPour(_selectedTube, tube))
        {
            var shakeTarget = _selectedTube;
            _selectedTube = null;
            _pourCoordinator.Lock(shakeTarget);
            shakeTarget.Shake();
            return;
        }

        var from = _selectedTube;
        _selectedTube = null;
        from.SetSelected(false);
        _pourCoordinator.StartPour(from, tube);
    }

    private void OnShakeComplete(ShakeCompletedEvent e)
    {
        _pourCoordinator.Unlock(e.Tube);
        e.Tube.SetSelected(false);
    }
}
