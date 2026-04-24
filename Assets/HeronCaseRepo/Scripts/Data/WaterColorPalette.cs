using System;
using UnityEngine;

namespace HeronCaseRepo.Scripts.Data
{
    [Serializable]
    public struct WaterColorEntry
    {
        public WaterColor colorId;
        public Color color;
    }

    [CreateAssetMenu(fileName = "WaterColorPalette", menuName = "WaterSort/Color Palette")]
    public class WaterColorPalette : ScriptableObject
    {
        public WaterColorEntry[] entries;

        public Color Get(WaterColor colorId)
        {
            foreach (var entry in entries)
            {
                if (entry.colorId == colorId)
                {
                    return entry.color;
                }
            }
            
            return Color.magenta;
        }
    }
}
