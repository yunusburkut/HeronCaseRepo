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

    private TubeWaterSlots _waterSlots;
    private bool _isSelected;
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

    public bool IsFull => _waterSlots.IsFull;

    public bool IsEmpty => _waterSlots.IsEmpty;

    public bool IsSolved { get; private set; }

    public bool IsSingleColor => _waterSlots.IsSingleColor;

    public Color TopColor => _waterSlots.TopColor;

    public Vector3 HeadWorldPos => tubeHead.position;

    private int AvailableSlots => _waterSlots.AvailableSlots;

    private int TopColorCount => _waterSlots.TopColorCount;

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

    private void OnDestroy()
    {
        _scope.KillAll();
    }

    public void Init(TubeData data, WaterView waterPrefab, WaterColorPalette palette)
    {
        _waterSlots = new TubeWaterSlots(waterContainer, waterPrefab, settings, data.capacity);

        ResizeTube(data.capacity);
        _restLocalPos = transform.localPosition;
        _anim.SetRestLocalPos(_restLocalPos);

        for (var i = 0; i < data.waters.Count; i++)
        {
            var entry = data.waters[i];
            _waterSlots.SpawnWater(palette.Get(entry.color), i, entry.modifier == WaterModifier.Hidden);
        }

        _waterSlots.RevealTopWater();
    }

    private void DoTransferWater()
    {
        TransferWater(_pourTarget);
    }

    private void InvokePourComplete()
    {
        EventBus<PourAnimationCompletedEvent>.Publish(new PourAnimationCompletedEvent { From = this, To = _pourTarget });
    }

    private void InvokeShakeComplete()
    {
        EventBus<ShakeCompletedEvent>.Publish(new ShakeCompletedEvent { Tube = this });
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventBus<TubeClickedEvent>.Publish(new TubeClickedEvent { Tube = this });
    }

    public void AccelerateCurrentPour(float timeScale)
    {
        _anim.AcceleratePour(timeScale);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        _anim.PlaySelect(selected);
    }

    public void Shake()
    {
        float snapY;
        if (_isSelected)
        {
            snapY = _restLocalPos.y + settings.LiftAmount;
        }
        else
        {
            snapY = _restLocalPos.y;
        }

        transform.localPosition = new Vector3(_restLocalPos.x, snapY, _restLocalPos.z);
        _anim.PlayShake(_cachedInvokeShakeComplete);
    }

    public bool HasColor(Color color)
    {
        return _waterSlots.HasColor(color);
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

    private void ShowLine(Color color)
    {
        _anim.ShowLine(color);
    }

    private void HideLine()
    {
        _anim.HideLine();
    }

    private void ShowPourLineOnTarget()
    {
        _pourTarget.ShowLine(_pourLineColor);
    }

    private void HideTargetLine()
    {
        _pourTarget.HideLine();
    }

    private void TransferWater(TubeView target)
    {
        var color = TopColor;
        var toMove = Mathf.Min(TopColorCount, target.AvailableSlots);
        for (var i = 0; i < toMove; i++)
        {
            _waterSlots.RemoveTopWater();
            target._waterSlots.AddWater(color, i * settings.WaterRevealDuration);
        }
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
