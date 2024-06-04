using Assets.Scripts.Fraction;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Assets.Scripts.Room;

[CustomEditor(typeof(SceneObjectManager))]
public class SceneObjectEditor : Editor
{
    private Object fractionManager;
    private Object roomStyleManager;
    private List<string> fractions;
    private ReorderableList sceneObjects;

    private void OnEnable()
    {
        SerializedProperty manager = serializedObject.FindProperty("fractionManager");
        fractionManager = manager.objectReferenceValue;
        roomStyleManager = serializedObject.FindProperty("roomStyleManager").objectReferenceValue;

        if (fractionManager != null && roomStyleManager != null && ((FractionManager)fractionManager).fractions.Count > 0)
        {
            fractions = new List<string>();
            for (int i = 0; i < ((FractionManager)fractionManager).fractions.Count; i++)
            {
                fractions.Add(((FractionManager)fractionManager).fractions[i].name);
            }

            sceneObjects = new ReorderableList(serializedObject,
    serializedObject.FindProperty("sceneObjects"),
    true, true, true, true);

            sceneObjects.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = sceneObjects.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                float halfWidth = rect.width * 0.3f;

                string newName = EditorGUI.TextField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),"Object Name",
                    element.FindPropertyRelative("name").stringValue
                );
                element.FindPropertyRelative("name").stringValue = newName;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                Object type = EditorGUI.ObjectField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Object Type",
                    element.FindPropertyRelative("type").objectReferenceValue, typeof(ScriptableObject), true
                );
                element.FindPropertyRelative("type").objectReferenceValue = type;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                Object prefab = EditorGUI.ObjectField(
                      new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Object prefab",
                     element.FindPropertyRelative("prefab").objectReferenceValue, typeof(GameObject), true
                );
                element.FindPropertyRelative("prefab").objectReferenceValue = prefab;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int min = EditorGUI.IntField(
                     new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Min spawn",
                    element.FindPropertyRelative("min").intValue
                );
                element.FindPropertyRelative("min").intValue = min;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int max = EditorGUI.IntField(
                     new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Max spawn",
                    element.FindPropertyRelative("max").intValue
                );
                element.FindPropertyRelative("max").intValue = max;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int fractions = 0;
                fractions = EditorGUI.MaskField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Fraction selector",
                    element.FindPropertyRelative("fractionsId").intValue, this.fractions.ToArray()
                );
                element.FindPropertyRelative("fractionsId").intValue = fractions;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

            };

            sceneObjects.elementHeight = EditorGUIUtility.singleLineHeight * 7;

            sceneObjects.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Scene Objects");
            };

        }


    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        fractionManager = EditorGUILayout.ObjectField(fractionManager, typeof(FractionManager), true);
        roomStyleManager = EditorGUILayout.ObjectField(roomStyleManager, typeof(RoomStyleManager), true);
        if (sceneObjects != null)
            sceneObjects.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            Update();
            Undo.RecordObject(target, "SAVE THIS");
            EditorUtility.SetDirty(target);
        }
        serializedObject.ApplyModifiedProperties();
    }

    public void Update()
    {
        if (fractionManager != null)
        {
            serializedObject.FindProperty("fractionManager").objectReferenceValue = fractionManager;
        }
        if (roomStyleManager != null)
        {
            serializedObject.FindProperty("roomStyleManager").objectReferenceValue = roomStyleManager;
        }

        if (GUI.changed)
        {
            // обновить инсппектор
            Repaint();
        }
    }
}