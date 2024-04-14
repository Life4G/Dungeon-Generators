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
        [SerializeField]
        public List<FractionRelation> relations;
    }
}
