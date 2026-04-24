using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeronCaseRepo.Scripts.Data
{
    [Serializable]
    public class TubeData
    {
        public int capacity;
        public List<Color> waterColors = new List<Color>();
    }
}