using System;
using System.Collections.Generic;

namespace HeronCaseRepo.Scripts.Data
{
    [Serializable]
    public class TubeData
    {
        public int capacity;
        public List<WaterEntry> waters = new List<WaterEntry>();
    }
}
