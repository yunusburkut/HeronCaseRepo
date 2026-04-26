using System;
using System.Collections.Generic;

public class PourCoordinator
{
    private readonly HashSet<TubeView> _lockedTubes = new HashSet<TubeView>();
    private readonly Dictionary<TubeView, TubeView> _activeTargets = new Dictionary<TubeView, TubeView>();
    private readonly Dictionary<TubeView, TubeView> _pendingPours = new Dictionary<TubeView, TubeView>();
    private readonly float _queuedSpeedMultiplier;
    private readonly Action<TubeView, TubeView> _cachedOnPourComplete;

    public event Action<TubeView, TubeView> OnPourCompleted;

    public PourCoordinator(float queuedSpeedMultiplier)
    {
        _queuedSpeedMultiplier = queuedSpeedMultiplier;
        _cachedOnPourComplete = OnPourComplete;
    }

    public bool IsLocked(TubeView tube)
    {
        return _lockedTubes.Contains(tube);
    }

    public bool HasActivePour(TubeView to)
    {
        return _activeTargets.ContainsKey(to);
    }

    public void Lock(TubeView tube)
    {
        _lockedTubes.Add(tube);
    }

    public void Unlock(TubeView tube)
    {
        _lockedTubes.Remove(tube);
    }

    public void StartPour(TubeView from, TubeView to)
    {
        _lockedTubes.Add(from);
        _activeTargets[to] = from;
        from.PourInto(to, _cachedOnPourComplete);
    }

    public bool TryQueuePour(TubeView from, TubeView to)
    {
        if (!_activeTargets.TryGetValue(to, out var activeFrom) ||
            _pendingPours.ContainsKey(to) ||
            !MoveValidator.CanPour(from, to))
        {
            return false;
        }

        _pendingPours[to] = from;
        _lockedTubes.Add(from);
        activeFrom.AccelerateCurrentPour(_queuedSpeedMultiplier);
        return true;
    }

    private void OnPourComplete(TubeView from, TubeView to)
    {
        _lockedTubes.Remove(from);
        _activeTargets.Remove(to);

        OnPourCompleted?.Invoke(from, to);

        if (!_pendingPours.Remove(to, out var pendingFrom))
        {
            return;
        }

        _lockedTubes.Remove(pendingFrom);

        if (MoveValidator.CanPour(pendingFrom, to))
        {
            StartPour(pendingFrom, to);
        }
    }
}
