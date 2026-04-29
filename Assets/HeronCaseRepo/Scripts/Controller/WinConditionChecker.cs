using System;
using System.Collections.Generic;
using DG.Tweening;

public class WinConditionChecker
{
    private readonly TweenCallback _cachedFireLevelCompleted;
    private List<TubeView> _allTubes;

    public event Action OnLevelCompleted;

    public WinConditionChecker()
    {
        _cachedFireLevelCompleted = FireLevelCompleted;
    }

    public void Initialize(List<TubeView> tubes)
    {
        _allTubes = new List<TubeView>(tubes);
        foreach (var tube in _allTubes)
        {
            TryMarkSolved(tube);
        }
    }

    public void OnPourCompleted(TubeView from, TubeView to)
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
            FireLevelCompleted();
        }
    }

    private void FireLevelCompleted()
    {
        OnLevelCompleted?.Invoke();
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
