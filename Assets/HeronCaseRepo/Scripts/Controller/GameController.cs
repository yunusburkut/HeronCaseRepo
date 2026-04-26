using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour, ILevelController
{
    [Header("Settings")]
    [SerializeField] private GameSettings settings;

    public event Action OnLevelCompleted;

    private TubeView _selectedTube;
    private TubeView _pendingShakeTarget;
    private List<TubeView> _allTubes;
    private PourCoordinator _pourCoordinator;

    private Action _cachedOnShakeComplete;
    private TweenCallback _cachedFireLevelCompleted;

    private void Awake()
    {
        _cachedOnShakeComplete = OnShakeComplete;
        _cachedFireLevelCompleted = FireLevelCompleted;
    }

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = new List<TubeView>(tubes);
        _pourCoordinator = new PourCoordinator(settings.queuedPourSpeedMultiplier);
        _pourCoordinator.OnPourCompleted += CheckAfterPour;

        foreach (var tube in _allTubes)
        {
            TryMarkSolved(tube);
        }
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

    private void FireLevelCompleted()
    {
        OnLevelCompleted?.Invoke();
    }

    private void CheckAfterPour(TubeView from, TubeView to)
    {
        var tweenTo = TryMarkSolved(to);
        var tweenFrom = TryMarkSolved(from);
        var tween = tweenTo ?? tweenFrom;

        if (!MoveValidator.IsLevelComplete(_allTubes))
        {
            return;
        }

        if (tween != null)
        {
            tween.OnComplete(_cachedFireLevelCompleted);
        }
        else
        {
            OnLevelCompleted?.Invoke();
        }
    }

    private Tween TryMarkSolved(TubeView tube)
    {
        if (tube.IsSolved || tube.IsEmpty || !tube.IsFull || !tube.IsSingleColor)
        {
            return null;
        }

        var color = tube.TopColor;
        foreach (var t in _allTubes)
        {
            if (t == tube || t.IsEmpty)
            {
                continue;
            }

            if (t.HasColor(color))
            {
                return null;
            }
        }

        return tube.MarkSolved();
    }
}
