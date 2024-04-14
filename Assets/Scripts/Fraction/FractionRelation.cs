using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Fraction
{
    [System.Serializable]
    public class FractionRelation
    {
        public Fraction faction1;                   // Ссылка на фракцию
        public RelationshipType relationshipType;   // Тип отношения
        public Fraction faction2;                   // Ссылка на фракцию
    }
}
