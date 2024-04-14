using Assets.Scripts.Fraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CustomEditor(typeof(FractionManager))]
    public class FractionManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FractionManager manager = (FractionManager)target;

            if (GUILayout.Button("Output the relationship to the console"))
            {
                foreach (var relation in manager.relations)
                {
                    string message = $"{relation.faction1.name} has a {relation.relationshipType} relationship with {relation.faction2.name}";
                    Debug.Log(message);
                }
            }
        }
    }
}
