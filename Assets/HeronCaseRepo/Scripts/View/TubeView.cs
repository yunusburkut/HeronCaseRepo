using System;
using System.Collections.Generic;
using HeronCaseRepo.Scripts.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class TubeView : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private Transform waterContainer;
    [SerializeField] private SpriteRenderer tubeRenderer;
    [SerializeField] private BoxCollider2D tubeCollider;
    [SerializeField] private RectTransform waterRect;

    [Header("Settings")]
    [SerializeField] private float waterSlotHeight = 0.5f;
    [SerializeField] private int defaultCapacity = 3; //TODO change
    [SerializeField] private Color selectedColor;

    private readonly List<WaterView> _waters = new List<WaterView>();
    private int _capacity;
    private Color _defaultColor;
    private bool _isSelected;
    private WaterView _waterPrefab;

    public void Init(TubeData data, WaterView waterPrefab)
    {
        _waterPrefab = waterPrefab;
        _capacity = data.capacity;
        _defaultColor = tubeRenderer.color;

        ResizeTube(_capacity);

        for (int i = 0; i < data.waterColors.Count; i++)
        {
            SpawnWater(data.waterColors[i], i);
        }
    }

    private void SpawnWater(Color color, int slotIndex)
    {
        var water = Instantiate(_waterPrefab, waterContainer);
        water.Init(color);
        water.transform.localPosition = new Vector3(0f, (slotIndex + 0.5f) * waterSlotHeight, 0f);
        water.name = $"Water_{slotIndex}";
        _waters.Add(water);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetSelected(!_isSelected);
    }

    private void SetSelected(bool selected)
    {
        _isSelected = selected;
        tubeRenderer.color = _isSelected ? selectedColor : _defaultColor;
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