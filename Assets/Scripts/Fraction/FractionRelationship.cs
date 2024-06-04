using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fraction
{
    /// <summary>
    /// Взаимоотношение двух фракций.
    /// </summary>
    [System.Serializable]
    public class FractionRelationship
    {
        /// <summary>
        /// Индекс первой фракции.
        /// </summary>
        public int fraction1Index;

        /// <summary>
        /// Индекс второй фракции.
        /// </summary>
        public int fraction2Index;

        /// <summary>
        /// Тип взаимоотношения между фракциями.
        /// </summary>
        public RelationshipType relationshipType;
    }
}
