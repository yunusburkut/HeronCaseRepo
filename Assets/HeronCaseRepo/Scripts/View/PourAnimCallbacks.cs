using DG.Tweening;

public readonly struct PourAnimCallbacks
{
    public readonly TweenCallback OnTransfer;
    public readonly TweenCallback OnShowLine;
    public readonly TweenCallback OnHideLine;
    public readonly TweenCallback OnComplete;

    public PourAnimCallbacks(TweenCallback onTransfer, TweenCallback onShowLine, TweenCallback onHideLine, TweenCallback onComplete)
    {
        OnTransfer = onTransfer;
        OnShowLine = onShowLine;
        OnHideLine = onHideLine;
        OnComplete = onComplete;
    }
}
