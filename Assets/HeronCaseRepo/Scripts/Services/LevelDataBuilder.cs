using System;
using System.Collections.Generic;
using Random = System.Random;

public static class LevelDataBuilder
{
    public static List<TubeData> Build(LevelData data, int seedOverride = -1)
    {
        var pool = BuildPool(data);
        Shuffle(pool, seedOverride >= 0 ? seedOverride : data.Seed);
        return DistributeIntoTubes(pool, data);
    }

    private static List<WaterEntry> BuildPool(LevelData data)
    {
        var pool = new List<WaterEntry>(data.Colors.Count * data.TubeCapacity);
        foreach (var color in data.Colors)
        {
            for (var i = 0; i < data.TubeCapacity; i++)
                pool.Add(new WaterEntry { color = color, modifier = WaterModifier.None });
        }
        return pool;
    }

    private static void Shuffle(List<WaterEntry> list, int seed)
    {
        var rng = new Random(seed != 0 ? seed : Environment.TickCount);
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static List<TubeData> DistributeIntoTubes(List<WaterEntry> pool, LevelData data)
    {
        var tubes = new List<TubeData>(data.Colors.Count + data.EmptyTubeCount);

        for (var i = 0; i < data.Colors.Count; i++)
        {
            var tube = new TubeData { capacity = data.TubeCapacity };
            for (var j = 0; j < data.TubeCapacity; j++)
                tube.waters.Add(pool[i * data.TubeCapacity + j]);
            tubes.Add(tube);
        }

        for (var i = 0; i < data.EmptyTubeCount; i++)
            tubes.Add(new TubeData { capacity = data.TubeCapacity });

        return tubes;
    }
}
