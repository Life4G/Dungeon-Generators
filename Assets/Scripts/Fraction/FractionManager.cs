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
    [ExecuteInEditMode]
    public class FractionManager : MonoBehaviour
    {
        public List<Fraction> fractions; 
        public List<FractionRelationship> relationships;
    }
}
