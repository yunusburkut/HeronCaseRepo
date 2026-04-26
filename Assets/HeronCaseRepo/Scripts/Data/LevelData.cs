using System.Collections.Generic;
using UnityEngine;

namespace HeronCaseRepo.Scripts.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "WaterSort/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Level Settings")]
        public int levelIndex;

        [Header("Tubes")]
        public int tubeCapacity = 4;
        public int emptyTubeCount = 2;

        [Header("Colors")]
        public List<WaterColor> colors;

        [Header("Randomization")]
        [Tooltip("0 = her oyunda farkli, >0 = sabit duzen")]
        public int seed;

        [HideInInspector]
        public List<TubeData> tubes = new List<TubeData>();
    }
}