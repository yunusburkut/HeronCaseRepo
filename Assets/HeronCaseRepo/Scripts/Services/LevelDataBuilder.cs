using System;
using System.Collections.Generic;

public static class LevelDataBuilder
{
    private static readonly List<WaterEntry> _pool = new List<WaterEntry>(64);
    private static readonly List<TubeData> _tubePool = new List<TubeData>(16);
    private static uint _rngState;

    public static void Build(LevelData data, List<TubeData> output, int seedOverride = -1)
    {
        BuildPool(data);
        var seed = seedOverride >= 0 ? seedOverride : data.Seed;
        _rngState = (uint)(seed != 0 ? seed : Environment.TickCount);
        Shuffle();
        DistributeIntoTubes(data, output);
    }

    private static void BuildPool(LevelData data)
    {
        _pool.Clear();
        for (var c = 0; c < data.Colors.Count; c++)
        {
            var color = data.Colors[c];
            for (var i = 0; i < data.TubeCapacity; i++)
                _pool.Add(new WaterEntry { color = color, modifier = WaterModifier.None });
        }
    }

    private static void Shuffle()
    {
        for (var i = _pool.Count - 1; i > 0; i--)
        {
            var j = (int)(NextRng() % (uint)(i + 1));
            (_pool[i], _pool[j]) = (_pool[j], _pool[i]);
        }
    }

    // xorshift32 — allocation-free seeded PRNG
    private static uint NextRng()
    {
        _rngState ^= _rngState << 13;
        _rngState ^= _rngState >> 17;
        _rngState ^= _rngState << 5;
        return _rngState;
    }

    private static void DistributeIntoTubes(LevelData data, List<TubeData> output)
    {
        output.Clear();
        var totalTubes = data.Colors.Count + data.EmptyTubeCount;

        while (_tubePool.Count < totalTubes)
            _tubePool.Add(new TubeData());

        for (var i = 0; i < data.Colors.Count; i++)
        {
            var tube = _tubePool[i];
            tube.capacity = data.TubeCapacity;
            tube.modifier = TubeModifier.None;
            tube.waters.Clear();
            for (var j = 0; j < data.TubeCapacity; j++)
                tube.waters.Add(_pool[i * data.TubeCapacity + j]);
            output.Add(tube);
        }

        for (var i = 0; i < data.EmptyTubeCount; i++)
        {
            var tube = _tubePool[data.Colors.Count + i];
            tube.capacity = data.TubeCapacity;
            tube.modifier = TubeModifier.None;
            tube.waters.Clear();
            output.Add(tube);
        }
    }
}
