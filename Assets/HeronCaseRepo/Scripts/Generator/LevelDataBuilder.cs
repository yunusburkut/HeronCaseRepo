using System;
using System.Collections.Generic;
using Random = System.Random;

public static class LevelDataBuilder
{
    public static List<TubeData> Build(LevelData data, int seedOverride = -1)
    {
        var pool = BuildPool(data);
        Shuffle(pool, seedOverride >= 0 ? seedOverride : data.seed);
        return DistributeIntoTubes(pool, data);
    }

    private static List<WaterEntry> BuildPool(LevelData data)
    {
        var pool = new List<WaterEntry>(data.colors.Count * data.tubeCapacity);
        foreach (var color in data.colors)
        {
            for (var i = 0; i < data.tubeCapacity; i++)
            {
                pool.Add(new WaterEntry { color = color, modifier = WaterModifier.None });
            }
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
        var tubes = new List<TubeData>(data.colors.Count + data.emptyTubeCount);

        for (var i = 0; i < data.colors.Count; i++)
        {
            var tube = new TubeData { capacity = data.tubeCapacity };
            for (var j = 0; j < data.tubeCapacity; j++)
            {
                tube.waters.Add(pool[i * data.tubeCapacity + j]);
            }
            
            tubes.Add(tube);
        }

        for (var i = 0; i < data.emptyTubeCount; i++)
        {
            tubes.Add(new TubeData { capacity = data.tubeCapacity });
        }

        return tubes;
    }
}
