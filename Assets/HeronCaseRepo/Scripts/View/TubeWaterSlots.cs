using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public sealed class TubeWaterSlots
{
    private readonly List<WaterView> _waters = new List<WaterView>();
    private readonly Transform _waterContainer;
    private readonly WaterView _waterPrefab;
    private readonly GameSettings _settings;
    private readonly int _capacity;

    public bool IsFull => _waters.Count >= _capacity;

    public bool IsEmpty => _waters.Count == 0;

    public int AvailableSlots => _capacity - _waters.Count;

    public Color TopColor
    {
        get
        {
            if (_waters.Count > 0)
            {
                return _waters[_waters.Count - 1].Color;
            }

            return Color.clear;
        }
    }

    public int TopColorCount
    {
        get
        {
            if (_waters.Count == 0)
            {
                return 0;
            }

            var top = _waters[_waters.Count - 1].Color;
            var count = 0;
            for (var i = _waters.Count - 1; i >= 0; i--)
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

    public TubeWaterSlots(Transform waterContainer, WaterView waterPrefab, GameSettings settings, int capacity)
    {
        _waterContainer = waterContainer;
        _waterPrefab = waterPrefab;
        _settings = settings;
        _capacity = capacity;
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

    public void SpawnWater(Color color, int slotIndex, bool isHidden = false, bool animate = false, float animDelay = 0f)
    {
        var water = Object.Instantiate(_waterPrefab, _waterContainer);
        water.Init(color);

        if (isHidden)
        {
            water.SetHidden(true);
        }

        if (animate)
        {
            water.SetReveal(0f);
            water.AnimateRevealTo(1f, _settings.WaterRevealDuration).SetDelay(animDelay);
        }

        water.transform.localPosition = new Vector3(
            0f,
            (slotIndex + 0.5f) * _settings.WaterSlotHeight - slotIndex * _settings.WaterYStackOffset,
            0f
        );

        water.SetSortingOrder(slotIndex);
        water.name = $"Water_{slotIndex}";
        _waters.Add(water);
    }

    public void AddWater(Color color, float delay = 0f)
    {
        SpawnWater(color, _waters.Count, animate: true, animDelay: delay);
    }

    public void RemoveTopWater()
    {
        var last = _waters.Count - 1;
        var water = _waters[last];
        _waters.RemoveAt(last);
        RevealTopWater();
        water.AnimateRevealTo(0f, _settings.WaterRevealDuration).OnComplete(water.CachedDestroySelf);
    }

    public void RevealTopWater()
    {
        if (_waters.Count > 0)
        {
            _waters[_waters.Count - 1].SetHidden(false);
        }
    }
}
