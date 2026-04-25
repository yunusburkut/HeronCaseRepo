using System;
using System.Collections.Generic;
using DG.Tweening;
using HeronCaseRepo.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class TubeView : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Transform waterContainer;
    [SerializeField] private Transform tubeHead;
    [SerializeField] private SpriteRenderer tubeRenderer;
    [SerializeField] private BoxCollider2D tubeCollider;
    [SerializeField] private RectTransform waterRect;

    [Header("Settings")]
    [SerializeField] private float waterSlotHeight = 0.5f;
    [SerializeField] private float waterYStackOffset = 0.25f;
    [SerializeField] private int defaultCapacity = 3;

    [Header("Animation")]
    [SerializeField] private float liftAmount = 0.3f;
    [SerializeField] private float liftDuration = 0.2f;
    [SerializeField] private float pourOffsetX = 0.8f;
    [SerializeField] private float pourAngle = 120f;
    [SerializeField] private float pourDuration = 0.25f;
    [SerializeField] private float pourHeightOffset = 0.5f;

    private readonly List<WaterView> _waters = new List<WaterView>();
    private int _capacity;
    private bool _isSelected;
    private WaterView _waterPrefab;
    private Vector3 _restLocalPos;

    private TubeView _pourTarget;
    private Action _pendingOnComplete;
    private TweenCallback _cachedTransferWater;
    private TweenCallback _cachedOnComplete;
    private Sequence _pourSequence;

    public event Action<TubeView> OnClicked;

    public bool IsFull
    {
        get
        {
            return _waters.Count >= _capacity;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return _waters.Count == 0;
        }
    }

    public bool IsSolved { get; private set; }

    public Color TopColor
    {
        get
        {
            return _waters.Count > 0 ? _waters[_waters.Count - 1].Color : Color.clear;
        }
    }

    private int AvailableSlots
    {
        get
        {
            return _capacity - _waters.Count;
        }
    }

    private Vector3 HeadWorldPos
    {
        get
        {
            return tubeHead.position;
        }
    }

    private int TopColorCount
    {
        get
        {
            if (_waters.Count == 0)
            {
                return 0;
            }

            var top = _waters[_waters.Count - 1].Color;
            var count = 0;
            for (int i = _waters.Count - 1; i >= 0; i--)
            {
                if (_waters[i].Color != top)
                {
                    break;
                }
                count++;
            }

            return count;
        }
    }

    private void Awake()
    {
        _cachedTransferWater = DoTransferWater;
        _cachedOnComplete = InvokeOnComplete;
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

    private void DoTransferWater()
    {
        TransferWater(_pourTarget);
    }

    private void InvokeOnComplete()
    {
        _pendingOnComplete?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(this);
    }

    private void AddWater(Color color)
    {
        SpawnWater(color, _waters.Count);
    }

    public void AccelerateCurrentPour(float timeScale)
    {
        if (_pourSequence != null && _pourSequence.IsActive())
        {
            _pourSequence.timeScale = timeScale;
        }
    }

    public void Shake(Action onComplete = null)
    {
        _pendingOnComplete = onComplete;
        transform.DOKill();
        var snapY = _isSelected ? _restLocalPos.y + liftAmount : _restLocalPos.y;
        transform.localPosition = new Vector3(_restLocalPos.x, snapY, _restLocalPos.z);

        var x = _restLocalPos.x;
        float d = 0.08f;//todo check magic numbers
        float t = 0.04f;

        DOTween.Sequence()
            .Append(transform.DOLocalMoveX(x + d, t).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalMoveX(x - d, t).SetEase(Ease.InOutQuad))
            .Append(transform.DOLocalMoveX(x + d * 0.6f, t).SetEase(Ease.InOutQuad))
            .Append(transform.DOLocalMoveX(x - d * 0.4f, t).SetEase(Ease.InOutQuad))
            .Append(transform.DOLocalMoveX(x, t).SetEase(Ease.InOutQuad))
            .OnComplete(_cachedOnComplete);
    }

    public bool IsSingleColor
    {
        get
        {
            if (_waters.Count == 0)
            {
                return false;
            }

            var first = _waters[0].Color;
            for (var i = 1; i < _waters.Count; i++)
            {
                if (_waters[i].Color != first)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public bool HasColor(Color color)
    {
        for (var i = 0; i < _waters.Count; i++)
        {
            if (_waters[i].Color == color)
            {
                return true;
            }
        }

        return false;
    }

    public Tween MarkSolved()
    {
        IsSolved = true;
        tubeCollider.enabled = false;
        transform.DOKill();
        transform.localPosition = _restLocalPos;
        return transform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 5, 0.5f);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        //todo add outline hover
        transform.DOKill();
        transform.DOLocalMoveY(
            _isSelected ? _restLocalPos.y + liftAmount : _restLocalPos.y,
            liftDuration
        ).SetEase(_isSelected ? Ease.OutBack : Ease.InOutSine);
    }

    public void PourInto(TubeView target, Action onComplete)
    {
        _pourTarget = target;
        _pendingOnComplete = onComplete;

        var isLeft = transform.position.x < target.transform.position.x;
        var signedOffsetX = isLeft ? -pourOffsetX : pourOffsetX;
        var signedAngle = isLeft ? -pourAngle : pourAngle;

        var pourWorldPos = new Vector3(
            target.HeadWorldPos.x + signedOffsetX,
            target.HeadWorldPos.y - tubeHead.localPosition.y + pourHeightOffset,
            0f
        );
        var restWorldPos = transform.parent.TransformPoint(_restLocalPos);

        _pourSequence = DOTween.Sequence();
        _pourSequence.Append(transform.DOMove(pourWorldPos, pourDuration).SetEase(Ease.OutQuad));
        _pourSequence.Append(transform.DORotate(new Vector3(0f, 0f, signedAngle), pourDuration).SetEase(Ease.OutQuad));
        _pourSequence.AppendCallback(_cachedTransferWater);
        _pourSequence.AppendInterval(0.4f);
        _pourSequence.Append(transform.DORotate(Vector3.zero, pourDuration).SetEase(Ease.InOutQuad));
        _pourSequence.Append(transform.DOMove(restWorldPos, pourDuration).SetEase(Ease.OutBack));
        _pourSequence.OnComplete(_cachedOnComplete);
    }

    private void TransferWater(TubeView target)
    {
        var color = TopColor;
        var toMove = Mathf.Min(TopColorCount, target.AvailableSlots);

        for (var i = 0; i < toMove; i++)
        {
            RemoveTopWater();
            target.AddWater(color);
        }
    }

    private void RemoveTopWater()
    {
        var last = _waters.Count - 1;
        Destroy(_waters[last].gameObject);
        _waters.RemoveAt(last);
        RevealTopWater();
    }

    private void RevealTopWater()
    {
        if (_waters.Count > 0)
        {
            _waters[_waters.Count - 1].SetHidden(false);
        }
    }

    private void SpawnWater(Color color, int slotIndex, bool isHidden = false)
    {
        var water = Instantiate(_waterPrefab, waterContainer);
        water.Init(color);

        if (isHidden)
        {
            water.SetHidden(true);
        }

        water.transform.localPosition = new Vector3(0f, (slotIndex + 0.5f) * waterSlotHeight - slotIndex * waterYStackOffset, 0f);
        water.SetSortingOrder(slotIndex);
        water.name = $"Water_{slotIndex}";
        _waters.Add(water);
    }

    private void ResizeTube(int capacity)
    {
        var extraHeight = (capacity - defaultCapacity) * waterSlotHeight;

        var size = tubeRenderer.size;
        tubeRenderer.size = new Vector2(size.x, size.y + extraHeight);

        tubeCollider.size = new Vector2(tubeCollider.size.x, tubeCollider.size.y + extraHeight);
        tubeCollider.offset = new Vector2(tubeCollider.offset.x, tubeCollider.offset.y + extraHeight * 0.5f);

        waterRect.sizeDelta = new Vector2(waterRect.sizeDelta.x, waterRect.sizeDelta.y + extraHeight);
        waterContainer.localPosition -= new Vector3(0f, extraHeight * 0.5f, 0f);

        transform.localPosition -= new Vector3(0f, extraHeight * 0.5f, 0f);
    }
}
