using Assets.Scripts.Fraction;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.SceneManagement;
using Unity.VisualScripting;
using Assets.Scripts.Room;

namespace Assets.Scripts.UI
{
    [CustomEditor(typeof(FractionManager))]
    public class FractionManagerEditor : Editor
    {
        private ReorderableList relationshipList;
        private ReorderableList fractionList;


        private void OnEnable()
        {
            fractionList = new ReorderableList(serializedObject,
                    serializedObject.FindProperty("fractions"),
                    true, true, true, true);

            fractionList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = fractionList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float halfWidth = rect.width * 0.5f;

                // имя фракции
                string newName = EditorGUI.TextField(
                    new Rect(rect.x, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("name").stringValue
                );
                element.FindPropertyRelative("name").stringValue = newName;

                // коэффициент распределения территорий
                float newCoefficient = EditorGUI.FloatField(
                    new Rect(rect.x + halfWidth + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("territoryCoefficient").floatValue
                );
                element.FindPropertyRelative("territoryCoefficient").floatValue = newCoefficient;
            };

            fractionList.elementHeight = EditorGUIUtility.singleLineHeight + 4;

            // заголовок
            fractionList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Fractions");
            };

            //-------------------------------------------------------------------------------------------

            relationshipList = new ReorderableList(serializedObject,
                serializedObject.FindProperty("relationships"),
                true, true, true, true);

            relationshipList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = relationshipList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                SerializedProperty fractionsProperty = serializedObject.FindProperty("fractions");

                int fraction1Index = element.FindPropertyRelative("fraction1Index").intValue;
                int fraction2Index = element.FindPropertyRelative("fraction2Index").intValue;

                string fraction1Name = fraction1Index >= 0 && fraction1Index < fractionsProperty.arraySize
                                       ? fractionsProperty.GetArrayElementAtIndex(fraction1Index).FindPropertyRelative("name").stringValue
                                       : "None";
                string fraction2Name = fraction2Index >= 0 && fraction2Index < fractionsProperty.arraySize
                                       ? fractionsProperty.GetArrayElementAtIndex(fraction2Index).FindPropertyRelative("name").stringValue
                                       : "None";

                float halfWidth = rect.width * 0.25f;

                EditorGUI.LabelField(new Rect(rect.x, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight), fraction1Name);
                EditorGUI.LabelField(new Rect(rect.x + halfWidth + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight), "-");
                EditorGUI.LabelField(new Rect(rect.x + halfWidth * 2 + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight), fraction2Name);

                // поле для изменения типа взаимотношений
                EditorGUI.PropertyField(
                    new Rect(rect.x + halfWidth * 3 + 5, rect.y, halfWidth - 5, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("relationshipType"), GUIContent.none);
            };

            relationshipList.elementHeightCallback = (int indexer) =>
            {
                return EditorGUIUtility.singleLineHeight + 4;
            };

            // заголовок
            relationshipList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Fraction Relationships");
            };
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();
            fractionList.DoLayoutList();
            if (EditorGUI.EndChangeCheck())
            {
                // если были изменения в списке фракций, обновляем взаимоотношения
                UpdateRelationships((FractionManager)target);
                Undo.RecordObject(target, "SAVE THIS");
                EditorUtility.SetDirty(target);
            }

            relationshipList.DoLayoutList();

            if (GUILayout.Button("Output fractons"))
            {
                PrintFractionsToDebugger();
            }

            if (GUILayout.Button("Output relationships"))
            {
                OutputRelationshipsToDebugger((FractionManager)target);
            }

            serializedObject.ApplyModifiedProperties();
        }
        private void UpdateRelationships(FractionManager manager)
        {
            if (manager.fractions.Count < 2)
            {
                manager.relationships.Clear();
            }
            else
            {
                // сохранить старые
                var oldRelationships = new Dictionary<(int, int), RelationshipType>();
                foreach (var rel in manager.relationships)
                {
                    var key = (rel.fraction1Index, rel.fraction2Index);
                    if (!oldRelationships.ContainsKey(key))
                    {
                        oldRelationships.Add(key, rel.relationshipType);
                    }
                }

                // очистить текущие записи
                var newRelationships = new List<FractionRelationship>();

                // геннерация новых
                SerializedProperty factionChangedList = serializedObject.FindProperty("fractions");
                for (int i = 0; i < factionChangedList.arraySize; i++)
                {
                    for (int j = i + 1; j < factionChangedList.arraySize; j++)
                    {
                        var key = (i, j);
                        RelationshipType relationshipType;
                        if (oldRelationships.ContainsKey(key))
                        {
                            relationshipType = oldRelationships[key];
                        }
                        else
                        {
                            relationshipType = RelationshipType.Neutral; // по умолчаннию
                        }

                        newRelationships.Add(new FractionRelationship
                        {
                            fraction1Index = i,
                            fraction2Index = j,
                            relationshipType = relationshipType
                        });
                    }
                }
                SerializedProperty relationProperty = serializedObject.FindProperty("relationships");
                relationProperty.ClearArray();
                for (int i = 0; i < newRelationships.Count; i++)
                {
                    relationProperty.InsertArrayElementAtIndex(i);
                    relationProperty.GetArrayElementAtIndex(i).FindPropertyRelative("fraction1Index").intValue = newRelationships[i].fraction1Index;
                    relationProperty.GetArrayElementAtIndex(i).FindPropertyRelative("fraction2Index").intValue = newRelationships[i].fraction2Index;
                    relationProperty.GetArrayElementAtIndex(i).FindPropertyRelative("relationshipType").intValue = (int)newRelationships[i].relationshipType;
                }
            }

            if (GUI.changed)
            {
                // обновить инсппектор
                Repaint();
            }
        }
        private void PrintFractionsToDebugger()
        {
            FractionManager manager = (FractionManager)target;

            Debug.Log("Fractions:"); int i = 0;
            foreach (var fraction in manager.fractions)
            {
                Debug.Log($"Fraction name: {fraction.name}, Territory coefficient: {fraction.territoryCoefficient}, Rooms: {manager.CalculateRoomsForFraction(13, i)}");
                i++;
            }
        }
        private void OutputRelationshipsToDebugger(FractionManager manager)
        {
            foreach (var rel in manager.relationships)
            {
                string faction1Name = manager.fractions[rel.fraction1Index].name;
                string faction2Name = manager.fractions[rel.fraction2Index].name;
                Debug.Log($"Relationship {faction1Name} and {faction2Name}: {rel.relationshipType}");
            }

        }
    }
}