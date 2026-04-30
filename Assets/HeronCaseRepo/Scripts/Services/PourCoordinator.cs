using System;
using System.Collections.Generic;

public class PourCoordinator : IDisposable
{
    private readonly HashSet<TubeView> _lockedTubes = new HashSet<TubeView>();
    private readonly Dictionary<TubeView, TubeView> _activeTargets = new Dictionary<TubeView, TubeView>();
    private readonly Dictionary<TubeView, TubeView> _pendingPours = new Dictionary<TubeView, TubeView>();
    private readonly float _queuedSpeedMultiplier;

    public PourCoordinator(float queuedSpeedMultiplier)
    {
        _queuedSpeedMultiplier = queuedSpeedMultiplier;
        EventBus<PourAnimationCompletedEvent>.Subscribe(OnPourAnimationCompleted);
    }

    public void Dispose()
    {
        EventBus<PourAnimationCompletedEvent>.Unsubscribe(OnPourAnimationCompleted);
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
        from.PourInto(to);
    }

    public void TryQueuePour(TubeView from, TubeView to)
    {
        if (!_activeTargets.TryGetValue(to, out var activeFrom) ||
            _pendingPours.ContainsKey(to) ||
            !MoveValidator.CanPour(from, to))
        {
            return;
        }

        _pendingPours[to] = from;
        _lockedTubes.Add(from);
        activeFrom.AccelerateCurrentPour(_queuedSpeedMultiplier);
    }

    private void OnPourAnimationCompleted(PourAnimationCompletedEvent e)
    {
        if (!_activeTargets.ContainsKey(e.To))
        {
            return;
        }

        _lockedTubes.Remove(e.From);
        _activeTargets.Remove(e.To);

        EventBus<PourCompletedEvent>.Publish(new PourCompletedEvent { From = e.From, To = e.To });

        if (!_pendingPours.Remove(e.To, out var pendingFrom))
        {
            return;
        }

        _lockedTubes.Remove(pendingFrom);

        if (MoveValidator.CanPour(pendingFrom, e.To))
        {
            StartPour(pendingFrom, e.To);
        }
    }
}
