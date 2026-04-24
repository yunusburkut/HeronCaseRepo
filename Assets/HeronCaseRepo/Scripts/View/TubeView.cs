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
    [SerializeField] private int defaultCapacity = 3;
    [SerializeField] private Color selectedColor;

    [Header("Animation")]
    [SerializeField] private float liftAmount = 0.3f;
    [SerializeField] private float liftDuration = 0.2f;
    [SerializeField] private float pourOffsetX = 0.8f;
    [SerializeField] private float pourAngle = 120f;
    [SerializeField] private float pourDuration = 0.25f;

    private readonly List<WaterView> _waters = new List<WaterView>();
    private int _capacity;
    private Color _defaultColor;
    private bool _isSelected;
    private WaterView _waterPrefab;
    private Vector3 _restLocalPos;

    public event Action<TubeView> OnClicked;

    public bool IsFull => _waters.Count >= _capacity;
    public bool IsEmpty => _waters.Count == 0;
    public Color TopColor => _waters.Count > 0 ? _waters[_waters.Count - 1].Color : Color.clear;
    public Vector3 HeadWorldPos => tubeHead.position;

    public void Init(TubeData data, WaterView waterPrefab)
    {
        _waterPrefab = waterPrefab;
        _capacity = data.capacity;
        _defaultColor = tubeRenderer.color;

        ResizeTube(_capacity);
        _restLocalPos = transform.localPosition;

        for (int i = 0; i < data.waterColors.Count; i++)
        {
            SpawnWater(data.waterColors[i], i);
        }
    }

    public void OnPointerClick(PointerEventData eventData) => OnClicked?.Invoke(this);
    public void AddWater(Color color) => SpawnWater(color, _waters.Count);

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        tubeRenderer.color = _isSelected ? selectedColor : _defaultColor;

        transform.DOKill();
        transform.DOLocalMoveY(
            _isSelected ? _restLocalPos.y + liftAmount : _restLocalPos.y,
            liftDuration
        ).SetEase(_isSelected ? Ease.OutBack : Ease.InOutSine);
    }

    public void PourInto(TubeView target, Action onComplete)
    {
        bool isLeft = transform.position.x < target.transform.position.x;
        float signedOffsetX = isLeft ? -pourOffsetX : pourOffsetX;
        float signedAngle = isLeft ? -pourAngle : pourAngle;

        float pourX = target.HeadWorldPos.x + signedOffsetX;
        float pourY = target.HeadWorldPos.y - tubeHead.localPosition.y;
        Vector3 pourWorldPos = new Vector3(pourX, pourY, 0f);
        Vector3 restWorldPos = transform.parent != null
            ? transform.parent.TransformPoint(_restLocalPos)
            : _restLocalPos;

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOMove(pourWorldPos, pourDuration).SetEase(Ease.OutQuad));
        seq.Append(transform.DORotate(new Vector3(0f, 0f, signedAngle), pourDuration).SetEase(Ease.OutQuad));
        seq.AppendCallback(() => TransferWater(target));
        seq.AppendInterval(0.4f);
        seq.Append(transform.DORotate(Vector3.zero, pourDuration).SetEase(Ease.InOutQuad));
        seq.Append(transform.DOMove(restWorldPos, pourDuration).SetEase(Ease.OutBack));
        seq.OnComplete(() => onComplete?.Invoke());
    }

    
    private void TransferWater(TubeView target)
    {
        if (IsEmpty || target.IsFull)
        {
            return;
        }
        
        Color color = TopColor;
        RemoveTopWater();
        target.AddWater(color);
    }

    private void RemoveTopWater()
    {
        if (_waters.Count == 0)
        {
            return;
        }
        
        var top = _waters[_waters.Count - 1];
        _waters.RemoveAt(_waters.Count - 1);
        Destroy(top.gameObject);
    }

    private void SpawnWater(Color color, int slotIndex)
    {
        var water = Instantiate(_waterPrefab, waterContainer);
        water.Init(color);
        water.transform.localPosition = new Vector3(0f, (slotIndex + 0.5f) * waterSlotHeight, 0f);
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
