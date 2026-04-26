using System;
using System.Collections.Generic;

[Serializable]
public class TubeData
{
    public int capacity;
    public List<WaterEntry> waters = new List<WaterEntry>();
}
