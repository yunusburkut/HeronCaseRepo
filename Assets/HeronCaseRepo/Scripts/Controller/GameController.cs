using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, ILevelController
{
    [Header("Settings")]
    [SerializeField] private GameSettings settings;

    public event Action<TubeView, TubeView> OnPourCompleted;

    private TubeView _selectedTube;
    private TubeView _pendingShakeTarget;
    private PourCoordinator _pourCoordinator;

    private Action _cachedOnShakeComplete;
    private Action<TubeView, TubeView> _cachedForwardPour;

    private void Awake()
    {
        _cachedOnShakeComplete = OnShakeComplete;
        _cachedForwardPour = (from, to) => OnPourCompleted?.Invoke(from, to);
    }

    public void Initialize(List<TubeView> tubes)
    {
        _selectedTube = null;
        _pendingShakeTarget = null;
        _pourCoordinator = new PourCoordinator(settings.QueuedPourSpeedMultiplier);
        _pourCoordinator.OnPourCompleted += _cachedForwardPour;
    }

    public void OnTubeClicked(TubeView tube)
    {
        if (_pourCoordinator.IsLocked(tube))
        {
            return;
        }

        if (_selectedTube == null)
        {
            if (tube.IsEmpty || tube.IsSolved || _pourCoordinator.HasActivePour(tube))
            {
                return;
            }

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
            _pendingShakeTarget = _selectedTube;
            _selectedTube = null;
            _pourCoordinator.Lock(_pendingShakeTarget);
            _pendingShakeTarget.Shake(_cachedOnShakeComplete);
            return;
        }

        var from = _selectedTube;
        _selectedTube = null;
        from.SetSelected(false);
        _pourCoordinator.StartPour(from, tube);
    }

    private void OnShakeComplete()
    {
        _pourCoordinator.Unlock(_pendingShakeTarget);
        _pendingShakeTarget.SetSelected(false);
        _pendingShakeTarget = null;
    }
}
