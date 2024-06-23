using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using UnityEditor.EditorTools;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    private static int seedGeometry = 0;
    private string seedGeometryString;
    private static int seedFraction = 0;
    private string seedFractionString;

    private bool ask = true;

    private void OnEnable()
    {
        GridManager manager = (GridManager)target;
        seedGeometry = manager.generator.GetSeed();
        seedGeometryString = seedGeometry.ToString();
        seedFraction = manager.roomManager.GenerateSeed();
        seedFractionString = seedFraction.ToString();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridManager manager = (GridManager)target;

        GUILayout.Label("Geometry seed");
        if (GUILayout.Button("Randomize Geometry seed"))
        {
            if (ask)
            {

            }
            seedGeometry = manager.generator.GenerateSeed();
            seedGeometryString = seedGeometry.ToString();
            manager.generator.SetSeed(seedGeometry);
        }
        seedGeometryString = GUILayout.TextField(seedGeometryString);
        seedGeometry = int.Parse(seedGeometryString);
        ask = GUILayout.Toggle(ask, "Don't ask");
     
        if (GUILayout.Button("Generate Geometry"))
        {
            manager.generator.SetSeed(seedGeometry);
            manager.Reload(seedGeometry);
        }

        GUILayout.Label("Seed");
        if (GUILayout.Button("Randomize Faction seed"))
        {
            if (ask)
            {

            }
            seedFraction = manager.roomManager.GenerateSeed();
            seedFractionString = seedFraction.ToString();
            manager.roomManager.SetSeed(seedFraction);
        }
        seedFractionString = GUILayout.TextField(seedFractionString);
        seedFraction = int.Parse(seedFractionString);

        if (GUILayout.Button("Generate Factions"))
        {
            manager.roomManager.AssignFractions(seedFraction);
            manager.roomManager.PrintRoomsInfo();
            manager.roomManager.ClearRoomsInfoFromMap();
            manager.roomManager.DisplayRoomsInfoOnMap();
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

public class WarningWindow : MonoBehaviour
{
    private Rect windowRect;

    WarningWindow(float x, float y, float width, float heigth)
    {
        windowRect = new Rect(x, y, width, heigth);
    }

    void OnGUI()
    {
        // Register the window. Notice the 3rd parameter
        windowRect = GUI.ModalWindow(0, windowRect, DoMyWindow, "Warning");

    }

    // Make the contents of the window
    void DoMyWindow(int windowID)
    {
        if (GUI.Button(new Rect(10, 20, 100, 20), "Hello World"))
        {
            print("Got a click");
        }
    }
}