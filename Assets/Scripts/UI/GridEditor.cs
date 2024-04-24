using UnityEngine;
using UnityEditor;
using System.Text;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    int seed;
    string seedString;
    string roomIdString;
    int roomId;
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
        GUILayout.Label("RoomId");
        roomIdString = GUILayout.TextField(roomIdString);
        if (GUILayout.Button("Validate"))
        {
            if (int.TryParse(roomIdString, out roomId))
                Debug.Log(manager.generator.GetGraph().GetRoom(roomId).Validate().ToString());
        }

        if (GUILayout.Button("Output Graph"))
        {
            int[,] graph = manager.generator.GetGraph().GetGraphMap();

            StringBuilder output = new StringBuilder();
            int rows = graph.GetLength(0);
            int cols = graph.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    output.Append(graph[i, j] + "\t");
                }
                if (i < rows - 1)
                {
                    output.AppendLine();
                }
            }
            Debug.Log(output.ToString());
        }
    }
}