using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct WaterColorEntry
{
    public WaterColor colorId;
    public Color color;
}

[CreateAssetMenu(fileName = "WaterColorPalette", menuName = "WaterSort/Color Palette")]
public class WaterColorPalette : ScriptableObject
{
    [SerializeField, FormerlySerializedAs("entries")] private WaterColorEntry[] _entries;

    private Dictionary<WaterColor, Color> _lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    public Color Get(WaterColor colorId)
    {
        if (_lookup.TryGetValue(colorId, out var color))
        {
            return color;
        }

        return Color.magenta;
    }

    private void BuildLookup()
    {
        _lookup = new Dictionary<WaterColor, Color>(_entries.Length);
        foreach (var entry in _entries)
        {
            _lookup[entry.colorId] = entry.color;
        }
    }
}
