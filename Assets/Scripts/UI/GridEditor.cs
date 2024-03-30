using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    int seed;
    string seedString;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager manager = (GridManager)target;
        GUILayout.Label("Seed");
        seedString = GUILayout.TextField(seedString);
        if(GUILayout.Button("Regenerate"))
        {
            if(int.TryParse(seedString,out seed))
                manager.Reload(seed);
            else
            manager.Reload();
        }
    }
}