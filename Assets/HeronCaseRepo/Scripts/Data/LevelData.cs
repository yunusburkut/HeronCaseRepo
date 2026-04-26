using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "WaterSort/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Tubes")]
    public int tubeCapacity = 4;
    public int emptyTubeCount = 2;

    [Header("Colors")]
    public List<WaterColor> colors;

    [Header("Randomization")]
    public int seed;

    [HideInInspector]
    public List<TubeData> tubes = new List<TubeData>();
}
