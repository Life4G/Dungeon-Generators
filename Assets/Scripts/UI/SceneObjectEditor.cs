using Assets.Scripts.Fraction;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using Assets.Scripts.Room;
using Unity.VisualScripting;

[CustomEditor(typeof(SceneObjectManager))]
public class SceneObjectEditor : Editor
{
    private Object fractionManager;
    private Object roomStyleManager;
    private Object roomManager;
    private List<string> fractionNames;
    private List<string> styleNames;
    private ReorderableList sceneObjects;

    private void OnEnable()
    {
        fractionManager = serializedObject.FindProperty("fractionManager").objectReferenceValue;
        roomStyleManager = serializedObject.FindProperty("roomStyleManager").objectReferenceValue;
        roomManager = serializedObject.FindProperty("roomManager").objectReferenceValue;

        if (fractionManager != null && roomStyleManager != null && ((FractionManager)fractionManager).fractions.Count > 0)
        {
            fractionNames = new List<string>();
            for (int i = 0; i < ((FractionManager)fractionManager).fractions.Count; i++)
            {
                fractionNames.Add(((FractionManager)fractionManager).fractions[i].name);
            }

            styleNames = new List<string>();
            for (int i = 0; i < ((RoomStyleManager)roomStyleManager).roomStyles.Count; i++)
            {
                styleNames.Add(((RoomStyleManager)roomStyleManager).roomStyles[i].styleName);
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
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Object Name",
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
                     new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Min spawn on map",
                    element.FindPropertyRelative("min").intValue
                );
                element.FindPropertyRelative("min").intValue = min;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int max = EditorGUI.IntField(
                     new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Max spawn on map",
                    element.FindPropertyRelative("max").intValue
                );
                element.FindPropertyRelative("max").intValue = max;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int maxRoom = EditorGUI.IntField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Max spawn in room",
                    element.FindPropertyRelative("maxRoom").intValue
                );
                element.FindPropertyRelative("maxRoom").intValue = maxRoom;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int fractions = 0;
                fractions = EditorGUI.MaskField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Fraction selector",
                    element.FindPropertyRelative("fractionIds").intValue, fractionNames.ToArray()
                );
                element.FindPropertyRelative("fractionIds").intValue = fractions;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

                int styles = 0;
                styles = EditorGUI.MaskField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Style selector",
                    element.FindPropertyRelative("styleIds").intValue, styleNames.ToArray()
                );
                element.FindPropertyRelative("styleIds").intValue = styles;
                rect.y += EditorGUIUtility.singleLineHeight + 2;

            };

            sceneObjects.elementHeight = EditorGUIUtility.singleLineHeight * 9;

            sceneObjects.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Scene Objects");
            };

        }


    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        roomManager = EditorGUILayout.ObjectField(roomManager, typeof(DungeonRoomManager), true);
        fractionManager = EditorGUILayout.ObjectField(fractionManager, typeof(FractionManager), true);
        roomStyleManager = EditorGUILayout.ObjectField(roomStyleManager, typeof(RoomStyleManager), true);
        if(roomManager!= null && GUILayout.Button("Generate Objects"))
        {
            ((SceneObjectManager)target).CalculateObjectsForRooms();
        }
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
        if (roomManager != null)
        {
            serializedObject.FindProperty("roomManager").objectReferenceValue = roomManager;
        }

        if (GUI.changed)
        {
            // обновить инсппектор
            Repaint();
        }
    }
}