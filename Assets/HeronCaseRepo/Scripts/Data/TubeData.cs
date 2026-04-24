using System;
using System.Collections.Generic;

namespace HeronCaseRepo.Scripts.Data
{
    [Serializable]
    public class TubeData
    {
        public int capacity;
        public List<WaterColor> waterColors = new List<WaterColor>();
    }
}
