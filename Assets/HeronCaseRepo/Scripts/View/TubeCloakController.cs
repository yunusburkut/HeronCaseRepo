using System;
using DG.Tweening;
using UnityEngine;

public class TubeCloakController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer cloakRenderer;
    [SerializeField] private GameSettings settings;

    private Color _triggerColor;
    private bool _isActive;
    private Sequence _revealSeq;
    private Action _onLifted;

    public void Resize(float extraHeight)
    {
        var s = cloakRenderer.size;
        cloakRenderer.size = new Vector2(s.x, s.y + extraHeight);
    }

    public void Activate(Color triggerColor, Action onLifted)
    {
        _triggerColor = triggerColor;
        _onLifted = onLifted;
        _isActive = true;
        cloakRenderer.color = triggerColor;
        cloakRenderer.enabled = true;
        EventBus<TubeSolvedEvent>.Subscribe(OnTubeSolved);
    }

    private void OnDestroy()
    {
        _revealSeq?.Kill();
        if (_isActive)
        {
            EventBus<TubeSolvedEvent>.Unsubscribe(OnTubeSolved);
        }
    }

    private void OnTubeSolved(TubeSolvedEvent e)
    {
        if (e.SolvedColor != _triggerColor)
        {
            return;
        }
        _isActive = false;
        EventBus<TubeSolvedEvent>.Unsubscribe(OnTubeSolved);
        _onLifted?.Invoke();
        PlayRevealAnimation();
    }

    private void PlayRevealAnimation()
    {
        var t = cloakRenderer.transform;
        var startLocalPos = t.localPosition;

        _revealSeq = DOTween.Sequence();

        _revealSeq.Append(t.DOLocalMoveY(startLocalPos.y - settings.CloakAnticipationDip, settings.CloakAnticipationDuration).SetEase(Ease.OutQuad));
        _revealSeq.Join(t.DOScale(new Vector3(settings.CloakAnticipationSquashX, settings.CloakAnticipationSquashY, 1f), settings.CloakAnticipationDuration).SetEase(Ease.OutQuad));

        _revealSeq.Append(t.DOScale(new Vector3(settings.CloakStretchX, settings.CloakStretchY, 1f), settings.CloakStretchDuration).SetEase(Ease.InQuad));

        _revealSeq.Append(t.DOLocalMoveY(startLocalPos.y + settings.CloakLaunchHeight, settings.CloakLaunchDuration).SetEase(Ease.InCubic));
        _revealSeq.Join(t.DOScale(Vector3.one, settings.CloakScaleReturnDuration).SetEase(Ease.OutQuad));
        _revealSeq.Join(t.DORotate(new Vector3(0f, 0f, settings.CloakRotation), settings.CloakLaunchDuration).SetEase(Ease.InSine));

        _revealSeq.OnComplete(() =>
        {
            cloakRenderer.enabled = false;
            t.localPosition = startLocalPos;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;
        });
    }
}
