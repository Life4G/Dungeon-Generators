using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.UIElements;
using Unity.VisualScripting;

[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor
{
    private static int seed = 0;
    private string seedString;
    private bool ask = true;

    private void OnEnable()
    {
        GridManager manager = (GridManager)target;
        seed = manager.generator.GetSeed();
        seedString = seed.ToString();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GridManager manager = (GridManager)target;

        GUILayout.Label("Seed");
        if (GUILayout.Button("Randomize seed"))
        {
            if (ask)
            {

            }
            seed = manager.generator.GenerateSeed();
            seedString = seed.ToString();
            manager.generator.SetSeed(seed);
        }
        seedString = GUILayout.TextField(seedString);
        ask = GUILayout.Toggle(ask, "Don't ask");
     
        if (GUILayout.Button("Generate"))
        {
            manager.generator.SetSeed(seed);
            manager.Reload(seed);
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