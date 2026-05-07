using System;
using System.Collections.Generic;
using DG.Tweening;

public class WinConditionChecker : IDisposable
{
    private readonly TweenCallback _cachedFireLevelCompleted;
    private List<TubeView> _allTubes;

    public WinConditionChecker()
    {
        _cachedFireLevelCompleted = FireLevelCompleted;
        EventBus<PourCompletedEvent>.Subscribe(HandlePourCompleted);
    }

    public void Dispose()
    {
        EventBus<PourCompletedEvent>.Unsubscribe(HandlePourCompleted);
    }

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = tubes;
        for (var i = 0; i < _allTubes.Count; i++)
        {
            TryMarkSolved(_allTubes[i]);
        }
    }

    private void HandlePourCompleted(PourCompletedEvent e)
    {
        var tweenTo = TryMarkSolved(e.To);
        var tweenFrom = TryMarkSolved(e.From);

        Tween tween;
        if (tweenTo != null)
        {
            tween = tweenTo;
        }
        else
        {
            tween = tweenFrom;
        }

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
            FireLevelCompleted();
        }
    }

    private void FireLevelCompleted()
    {
        EventBus<LevelCompletedEvent>.Publish(new LevelCompletedEvent());
    }

    private Tween TryMarkSolved(TubeView tube)
    {
        if (tube.IsSolved || tube.IsEmpty || !tube.IsFull || !tube.IsSingleColor)
        {
            return null;
        }

        var color = tube.TopColor;
        for (var i = 0; i < _allTubes.Count; i++)
        {
            var t = _allTubes[i];
            if (t == tube || t.IsEmpty) continue;
            if (t.HasColor(color)) return null;
        }

        return tube.MarkSolved();
    }
}
