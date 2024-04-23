using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fraction
{
    public enum RelationshipType
    {
        Neutral,
        Ally,
        Enemy
    }

    public class FractionManager : MonoBehaviour
    {
        public List<Fraction> fractions;
        public List<FractionRelationship> relationships;

        /// <summary>
        /// Расчет количества комнат для фракции
        /// </summary>
        /// <param name="totalRooms"></param>
        /// <param name="fractionIndex"></param>
        /// <returns></returns>
        public int CalculateRoomsForFraction(int totalRooms, int fractionIndex)
        {
            float totalCoefficients = fractions.Sum(f => f.territoryCoefficient);
            Fraction targetFraction = fractions[fractionIndex];
            return (int)Math.Round((targetFraction.territoryCoefficient / totalCoefficients) * totalRooms);
        }
    }
}
