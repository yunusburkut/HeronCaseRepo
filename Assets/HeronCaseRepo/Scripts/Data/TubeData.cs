using System;
using System.Collections.Generic;

[Serializable]
public class TubeData
{
    public int capacity;
    public TubeModifier modifier;
    public WaterColor cloakTriggerColor;
    public List<WaterEntry> waters = new List<WaterEntry>();
}
