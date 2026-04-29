using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class TubeView : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Transform waterContainer;
    [SerializeField] private Transform tubeHead;
    [SerializeField] private SpriteRenderer tubeRenderer;
    [SerializeField] private SpriteRenderer outlineRenderer;
    [SerializeField] private BoxCollider2D tubeCollider;
    [SerializeField] private RectTransform waterRect;
    [SerializeField] private SpriteRenderer lineRenderer;

    [Header("Settings")]
    [SerializeField] private GameSettings settings;
    [SerializeField] private int defaultCapacity = 3;

    private readonly List<WaterView> _waters = new List<WaterView>();
    private int _capacity;
    private bool _isSelected;
    private WaterView _waterPrefab;
    private Vector3 _restLocalPos;

    private TubeView _pourTarget;
    private Color _pourLineColor;
    private Action<TubeView, TubeView> _pourOnComplete;
    private Action _shakeOnComplete;
    private TweenCallback _cachedTransferWater;
    private TweenCallback _cachedInvokePourComplete;
    private TweenCallback _cachedInvokeShakeComplete;
    private TweenCallback _cachedShowPourLine;
    private TweenCallback _cachedHideTargetLine;
    private Sequence _pourSequence;

    public event Action<TubeView> OnClicked;

    public bool IsFull => _waters.Count >= _capacity;
    public bool IsEmpty => _waters.Count == 0;
    public bool IsSolved { get; private set; }

    public Color TopColor => _waters.Count > 0 ? _waters[_waters.Count - 1].Color : Color.clear;

    private int AvailableSlots => _capacity - _waters.Count;
    private Vector3 HeadWorldPos => tubeHead.position;

    private int TopColorCount
    {
        get
        {
            if (_waters.Count == 0) return 0;

            var top = _waters[_waters.Count - 1].Color;
            var count = 0;
            for (int i = _waters.Count - 1; i >= 0; i--)
            {
                if (_waters[i].Color != top) break;
                count++;
            }
            return count;
        }
    }

    private void Awake()
    {
        _cachedTransferWater = DoTransferWater;
        _cachedInvokePourComplete = InvokePourComplete;
        _cachedInvokeShakeComplete = InvokeShakeComplete;
        _cachedShowPourLine = ShowPourLineOnTarget;
        _cachedHideTargetLine = HideTargetLine;
        lineRenderer.color = Color.clear;
        outlineRenderer.color = Color.clear;
    }

    public void Init(TubeData data, WaterView waterPrefab, WaterColorPalette palette)
    {
        _waterPrefab = waterPrefab;
        _capacity = data.capacity;

        ResizeTube(_capacity);
        _restLocalPos = transform.localPosition;

        for (var i = 0; i < data.waters.Count; i++)
        {
            var entry = data.waters[i];
            SpawnWater(palette.Get(entry.color), i, entry.modifier == WaterModifier.Hidden);
        }
        RevealTopWater();
    }

    private void DoTransferWater() => TransferWater(_pourTarget);
    private void InvokePourComplete() => _pourOnComplete.Invoke(this, _pourTarget);
    private void InvokeShakeComplete() => _shakeOnComplete?.Invoke();

    public void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(this);

    private void AddWater(Color color, float delay = 0f)
    {
        SpawnWater(color, _waters.Count, animate: true, animDelay: delay);
    }

    public void AccelerateCurrentPour(float timeScale)
    {
        if (_pourSequence != null && _pourSequence.IsActive())
            _pourSequence.timeScale = timeScale;
    }

    // Anticipation: tube dips slightly before lifting (principle: Anticipation)
    // Landing: OutBounce simulates weight settling (principle: Follow Through)
    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        transform.DOKill();
        outlineRenderer.DOKill();
        outlineRenderer.DOColor(selected ? settings.OutlineColor : Color.clear, 0.1f);

        if (selected)
        {
            DOTween.Sequence()
                .SetTarget(transform)
                .SetRecyclable(true)
                .Append(transform.DOLocalMoveY(_restLocalPos.y - settings.AnticipationDip, settings.AnticipationDuration).SetEase(Ease.OutQuad))
                .Append(transform.DOLocalMoveY(_restLocalPos.y + settings.LiftAmount, settings.LiftDuration).SetEase(Ease.OutBack));
        }
        else
        {
            transform.DOLocalMoveY(_restLocalPos.y, settings.LiftDuration).SetEase(Ease.OutBounce);
        }
    }

    // Shake uses OutExpo for snappy initial impact, decays with InOutSine (principle: Exaggeration, Slow In/Out)
    public void Shake(Action onComplete = null)
    {
        _shakeOnComplete = onComplete;
        transform.DOKill();
        var snapY = _isSelected ? _restLocalPos.y + settings.LiftAmount : _restLocalPos.y;
        transform.localPosition = new Vector3(_restLocalPos.x, snapY, _restLocalPos.z);

        var x = _restLocalPos.x;
        var d = settings.ShakeMagnitude;
        var t = settings.ShakeDuration;

        DOTween.Sequence()
            .SetRecyclable(true)
            .Append(transform.DOLocalMoveX(x + d, t).SetEase(Ease.OutExpo))
            .Append(transform.DOLocalMoveX(x - d, t).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveX(x + d * settings.ShakeDecay1, t).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveX(x - d * settings.ShakeDecay2, t).SetEase(Ease.InOutSine))
            .Append(transform.DOLocalMoveX(x, t).SetEase(Ease.OutSine))
            .OnComplete(_cachedInvokeShakeComplete);
    }

    public bool IsSingleColor
    {
        get
        {
            if (_waters.Count == 0) return false;

            var first = _waters[0].Color;
            for (var i = 1; i < _waters.Count; i++)
            {
                if (_waters[i].Color != first) return false;
            }
            return true;
        }
    }

    public bool HasColor(Color color)
    {
        for (var i = 0; i < _waters.Count; i++)
        {
            if (_waters[i].Color == color) return true;
        }
        return false;
    }

    // Squash on impact then OutElastic bounce (principle: Squash & Stretch)
    public Tween MarkSolved()
    {
        IsSolved = true;
        tubeCollider.enabled = false;
        transform.DOKill();
        transform.localPosition = _restLocalPos;
        transform.localScale = Vector3.one;

        return DOTween.Sequence()
            .SetRecyclable(true)
            .Append(transform.DOScale(new Vector3(settings.SolvedSquashX, settings.SolvedSquashY, 1f), settings.SolvedSquashDuration).SetEase(Ease.OutQuad))
            .Append(transform.DOScale(Vector3.one, settings.SolvedBounceDuration).SetEase(Ease.OutElastic));
    }

    // Arc movement to pour position (principle: Arc)
    // Rotation overshoots pour angle via OutBack (principle: Follow Through)
    // Return rotation also overshoots via OutBack (principle: Follow Through)
    public void PourInto(TubeView target, Action<TubeView, TubeView> onComplete)
    {
        transform.DOKill();
        _pourTarget = target;
        _pourOnComplete = onComplete;

        _pourLineColor = TopColor;
        var isLeft = transform.position.x < target.transform.position.x;
        var signedOffsetX = isLeft ? -settings.PourOffsetX : settings.PourOffsetX;
        var signedAngle = isLeft ? -settings.PourAngle : settings.PourAngle;

        var pourWorldPos = new Vector3(
            target.HeadWorldPos.x + signedOffsetX,
            target.HeadWorldPos.y - tubeHead.localPosition.y + settings.PourHeightOffset,
            0f
        );
        var arcPeak = new Vector3(
            (transform.position.x + pourWorldPos.x) * 0.5f,
            Mathf.Max(transform.position.y, pourWorldPos.y) + settings.PourArcHeight,
            0f
        );
        var restWorldPos = transform.parent.TransformPoint(_restLocalPos);

        _pourSequence = DOTween.Sequence().SetRecyclable(true);
        _pourSequence.Append(transform.DOMove(arcPeak, settings.PourDuration * 0.45f).SetEase(Ease.OutSine));
        _pourSequence.Append(transform.DOMove(pourWorldPos, settings.PourDuration * 0.55f).SetEase(Ease.InSine));
        _pourSequence.Append(transform.DORotate(new Vector3(0f, 0f, signedAngle), settings.PourDuration).SetEase(Ease.OutBack));
        _pourSequence.AppendCallback(_cachedTransferWater);
        _pourSequence.AppendCallback(_cachedShowPourLine);
        _pourSequence.AppendInterval(settings.PourHoldDuration);
        _pourSequence.AppendCallback(_cachedHideTargetLine);
        _pourSequence.Append(transform.DORotate(Vector3.zero, settings.PourDuration).SetEase(Ease.OutBack));
        _pourSequence.Append(transform.DOMove(restWorldPos, settings.PourDuration).SetEase(Ease.OutBack));
        _pourSequence.OnComplete(_cachedInvokePourComplete);
    }

    public void ShowLine(Color color)
    {
        lineRenderer.DOKill();
        lineRenderer.color = new Color(color.r, color.g, color.b, 1f);
    }

    public void HideLine()
    {
        lineRenderer.DOKill();
        lineRenderer.DOFade(0f, settings.PourDuration);
    }

    private void ShowPourLineOnTarget() => _pourTarget.ShowLine(_pourLineColor);
    private void HideTargetLine() => _pourTarget.HideLine();

    private void TransferWater(TubeView target)
    {
        var color = TopColor;
        var toMove = Mathf.Min(TopColorCount, target.AvailableSlots);

        for (var i = 0; i < toMove; i++)
        {
            RemoveTopWater();
            target.AddWater(color, i * settings.WaterRevealDuration);
        }
    }

    private void RemoveTopWater()
    {
        var last = _waters.Count - 1;
        var water = _waters[last];
        _waters.RemoveAt(last);
        RevealTopWater();
        water.AnimateRevealTo(0f, settings.WaterRevealDuration).OnComplete(water.CachedDestroySelf);
    }

    private void RevealTopWater()
    {
        if (_waters.Count > 0)
            _waters[_waters.Count - 1].SetHidden(false);
    }

    private void SpawnWater(Color color, int slotIndex, bool isHidden = false, bool animate = false, float animDelay = 0f)
    {
        var water = Instantiate(_waterPrefab, waterContainer);
        water.Init(color);

        if (isHidden) water.SetHidden(true);

        if (animate)
        {
            water.SetReveal(0f);
            water.AnimateRevealTo(1f, settings.WaterRevealDuration).SetDelay(animDelay);
        }

        water.transform.localPosition = new Vector3(
            0f,
            (slotIndex + 0.5f) * settings.WaterSlotHeight - slotIndex * settings.WaterYStackOffset,
            0f
        );
        water.SetSortingOrder(slotIndex);
        water.name = $"Water_{slotIndex}";
        _waters.Add(water);
    }

    private void ResizeTube(int capacity)
    {
        var extraHeight = (capacity - defaultCapacity) * settings.WaterSlotHeight;

        var size = tubeRenderer.size;
        tubeRenderer.size = new Vector2(size.x, size.y + extraHeight);

        tubeCollider.size = new Vector2(tubeCollider.size.x, tubeCollider.size.y + extraHeight);
        tubeCollider.offset = new Vector2(tubeCollider.offset.x, tubeCollider.offset.y + extraHeight * 0.5f);

        waterRect.sizeDelta = new Vector2(waterRect.sizeDelta.x, waterRect.sizeDelta.y + extraHeight);
        waterContainer.localPosition -= new Vector3(0f, extraHeight * 0.5f, 0f);

        transform.localPosition -= new Vector3(0f, extraHeight * 0.5f, 0f);

        var lt = lineRenderer.transform;
        lt.localScale = new Vector3(lt.localScale.x, tubeRenderer.size.y, lt.localScale.z);

        var t = settings.OutlineThickness;
        outlineRenderer.size = new Vector2(tubeRenderer.size.x + t * 2f, tubeRenderer.size.y + t * 2f);
    }
}
