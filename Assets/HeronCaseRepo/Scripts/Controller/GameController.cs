using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float queuedPourSpeedMultiplier = 1.75f;

    public event Action OnLevelCompleted;

    private TubeView _selectedTube;
    private TubeView _pendingShakeTarget;
    private List<TubeView> _allTubes;

    private readonly HashSet<TubeView> _lockedTubes = new HashSet<TubeView>();
    private readonly Dictionary<TubeView, TubeView> _activeTargets = new Dictionary<TubeView, TubeView>();
    private readonly Dictionary<TubeView, TubeView> _pendingPours = new Dictionary<TubeView, TubeView>();

    private Action _cachedOnShakeComplete;

    private void Awake()
    {
        _cachedOnShakeComplete = OnShakeComplete;
    }

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = new List<TubeView>(tubes);

        foreach (var tube in _allTubes)
        {
            TryMarkSolved(tube);
        }
    }

    public void OnTubeClicked(TubeView tube)
    {
        if (_lockedTubes.Contains(tube))
        {
            return;
        }

        if (_selectedTube == null)
        {
            if (tube.IsEmpty || tube.IsSolved || _activeTargets.ContainsKey(tube))
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

        if (_activeTargets.TryGetValue(tube, out var target))
        {
            if (_pendingPours.ContainsKey(tube) || !MoveValidator.CanPour(_selectedTube, tube))
            {
                _selectedTube.SetSelected(false);
                _selectedTube = null;
                return;
            }

            _pendingPours[tube] = _selectedTube;
            _lockedTubes.Add(_selectedTube);
            _selectedTube.SetSelected(false);
            _selectedTube = null;
            target.AccelerateCurrentPour(queuedPourSpeedMultiplier);
            return;
        }

        if (!MoveValidator.CanPour(_selectedTube, tube))
        {
            _pendingShakeTarget = _selectedTube;
            _selectedTube = null;
            _lockedTubes.Add(_pendingShakeTarget);
            _pendingShakeTarget.Shake(_cachedOnShakeComplete);
            return;
        }

        StartPour(_selectedTube, tube);
        _selectedTube = null;
    }

    private void StartPour(TubeView from, TubeView to)
    {
        _lockedTubes.Add(from);
        _activeTargets[to] = from;
        from.SetSelected(false);
        from.PourInto(to, () => OnPourComplete(from, to));
    }

    private void OnPourComplete(TubeView from, TubeView to)
    {
        _lockedTubes.Remove(from);
        _activeTargets.Remove(to);

        CheckAfterPour(from, to);

        if (!_pendingPours.Remove(to, out var pendingFrom)) return;
        {
            _lockedTubes.Remove(pendingFrom);
        }

        if (MoveValidator.CanPour(pendingFrom, to))
        {
            StartPour(pendingFrom, to);
        }
    }

    private void OnShakeComplete()
    {
        _lockedTubes.Remove(_pendingShakeTarget);
        _pendingShakeTarget.SetSelected(false);
        _pendingShakeTarget = null;
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
            tween.OnComplete(() => OnLevelCompleted?.Invoke());
        }
        else
        {
            OnLevelCompleted?.Invoke();
        }
    }

    private Tween TryMarkSolved(TubeView tube)
    {
        if (tube == null || tube.IsSolved || tube.IsEmpty || !tube.IsFull || !tube.IsSingleColor)
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
