using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fraction
{
        [System.Serializable]
        public class Fraction
        {
            public string name;                // имя фракции
            public float territoryCoefficient; // коэффициент распределения территорий
            public Color color;
        }
}
