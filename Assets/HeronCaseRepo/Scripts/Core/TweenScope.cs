using System.Collections.Generic;
using DG.Tweening;

public sealed class TweenScope
{
    private readonly List<Tween> _active = new List<Tween>();

    public T Add<T>(T tween) where T : Tween
    {
        if (tween == null || !tween.IsActive())
        {
            return tween;
        }
        
        _active.Add(tween);
        tween.OnKill(() => _active.Remove(tween));
        return tween;
    }

    public void KillAll()
    {
        for (var i = _active.Count - 1; i >= 0; i--)
        {
            if (_active[i].IsActive())
            {
                _active[i].Kill(false);
            }
        }
        
        _active.Clear();
    }

    public void PauseAll()
    {
        for (var i = 0; i < _active.Count; i++)
        {
            if (_active[i].IsActive())
            {
                _active[i].Pause();
            }
        }
    }

    public void ResumeAll()
    {
        for (var i = 0; i < _active.Count; i++)
        {
            if (_active[i].IsActive())
            {
                _active[i].Play();
            }
        }
    }
}
