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
        public List<TubeData> tubes = new List<TubeData>();
    }
}