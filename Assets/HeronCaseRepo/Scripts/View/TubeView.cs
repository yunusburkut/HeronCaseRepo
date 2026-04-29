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
    private TweenCallback _cachedTransferWater;
    private TweenCallback _cachedInvokePourComplete;
    private TweenCallback _cachedInvokeShakeComplete;
    private TweenCallback _cachedShowPourLine;
    private TweenCallback _cachedHideTargetLine;

    private TweenScope _scope;
    private TubeAnimController _anim;

    public bool IsFull => _waters.Count >= _capacity;
    public bool IsEmpty => _waters.Count == 0;
    public bool IsSolved { get; private set; }

    public Color TopColor => _waters.Count > 0 ? _waters[_waters.Count - 1].Color : Color.clear;
    public Vector3 HeadWorldPos => tubeHead.position;

    private int AvailableSlots => _capacity - _waters.Count;

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
        _scope = new TweenScope();
        _anim = new TubeAnimController(transform, tubeHead, outlineRenderer, lineRenderer, settings, _scope);

        _cachedTransferWater = DoTransferWater;
        _cachedInvokePourComplete = InvokePourComplete;
        _cachedInvokeShakeComplete = InvokeShakeComplete;
        _cachedShowPourLine = ShowPourLineOnTarget;
        _cachedHideTargetLine = HideTargetLine;

        lineRenderer.color = Color.clear;
        outlineRenderer.color = Color.clear;
    }

    private void OnDisable() => _scope.KillAll();
    private void OnDestroy() => _scope.KillAll();

    public void Init(TubeData data, WaterView waterPrefab, WaterColorPalette palette)
    {
        _waterPrefab = waterPrefab;
        _capacity = data.capacity;

        ResizeTube(_capacity);
        _restLocalPos = transform.localPosition;
        _anim.SetRestLocalPos(_restLocalPos);

        for (var i = 0; i < data.waters.Count; i++)
        {
            var entry = data.waters[i];
            SpawnWater(palette.Get(entry.color), i, entry.modifier == WaterModifier.Hidden);
        }
        
        RevealTopWater();
    }

    private void DoTransferWater() => TransferWater(_pourTarget);
    private void InvokePourComplete() => EventBus<PourAnimationCompletedEvent>.Publish(new PourAnimationCompletedEvent { From = this, To = _pourTarget });
    private void InvokeShakeComplete() => EventBus<ShakeCompletedEvent>.Publish(new ShakeCompletedEvent { Tube = this });

    public void OnPointerClick(PointerEventData eventData) =>
        EventBus<TubeClickedEvent>.Publish(new TubeClickedEvent { Tube = this });

    private void AddWater(Color color, float delay = 0f) =>
        SpawnWater(color, _waters.Count, animate: true, animDelay: delay);

    public void AccelerateCurrentPour(float timeScale) => _anim.AcceleratePour(timeScale);

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _anim.PlaySelect(selected);
    }

    public void Shake()
    {
        var snapY = _isSelected ? _restLocalPos.y + settings.LiftAmount : _restLocalPos.y;
        transform.localPosition = new Vector3(_restLocalPos.x, snapY, _restLocalPos.z);
        _anim.PlayShake(_cachedInvokeShakeComplete);
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

    public Tween MarkSolved()
    {
        IsSolved = true;
        tubeCollider.enabled = false;
        return _anim.PlayMarkSolved();
    }

    public void PourInto(TubeView target)
    {
        _pourTarget = target;
        _pourLineColor = TopColor;

        var restWorldPos = transform.parent.TransformPoint(_restLocalPos);
        _anim.PlayPourInto(target, restWorldPos, _cachedTransferWater, _cachedShowPourLine, _cachedHideTargetLine, _cachedInvokePourComplete);
    }

    public void ShowLine(Color color) => _anim.ShowLine(color);
    public void HideLine() => _anim.HideLine();

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
        {
            _waters[_waters.Count - 1].SetHidden(false);
        }
    }

    private void SpawnWater(Color color, int slotIndex, bool isHidden = false, bool animate = false, float animDelay = 0f)
    {
        var water = Instantiate(_waterPrefab, waterContainer);
        water.Init(color);

        if (isHidden)
        {
            water.SetHidden(true);
        }

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
