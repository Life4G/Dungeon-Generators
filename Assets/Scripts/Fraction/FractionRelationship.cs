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
        public int fraction1Index;                   // ссылка на фракцию
        public int fraction2Index;                   // ссылка на фракцию
        public RelationshipType relationshipType;    // тип отношения

    }
}
