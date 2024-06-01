using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fraction
{
    /// <summary>
    /// Фракция.
    /// </summary>
    [System.Serializable]
    public class Fraction
    {
        /// <summary>
        /// Имя фракции.
        /// </summary>
        public string name;

        /// <summary>
        /// Коэффициент распределения территорий.
        /// </summary>
        public float territoryCoefficient;

        /// <summary>
        /// Цвет фракции.
        /// </summary>
        public Color color;
    }
}
