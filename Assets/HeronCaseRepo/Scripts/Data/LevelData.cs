using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "LevelData", menuName = "WaterSort/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Tubes")]
    [SerializeField, FormerlySerializedAs("tubeCapacity")] private int _tubeCapacity = 4;
    [SerializeField, FormerlySerializedAs("emptyTubeCount")] private int _emptyTubeCount = 2;

    [Header("Colors")]
    [SerializeField, FormerlySerializedAs("colors")] private List<WaterColor> _colors;

    [Header("Randomization")]
    [SerializeField, FormerlySerializedAs("seed")] private int _seed;

    [HideInInspector]
    [SerializeField, FormerlySerializedAs("tubes")] private List<TubeData> _tubes = new List<TubeData>();

    public int TubeCapacity => _tubeCapacity;

    public int EmptyTubeCount => _emptyTubeCount;

    public List<WaterColor> Colors => _colors;

    public int Seed => _seed;

    public List<TubeData> Tubes => _tubes;

    public void SetTubes(List<TubeData> tubes)
    {
        _tubes = tubes;
    }
}
