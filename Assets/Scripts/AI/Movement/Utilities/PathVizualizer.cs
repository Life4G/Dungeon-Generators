using UnityEngine;
using System.Collections.Generic;

public class PathVisualizer : MonoBehaviour
{
    private List<Vector3[]> paths = new List<Vector3[]>();

    public void AddPath(Vector3[] path)
    {
        paths.Add(path);
    }

    public void ClearPaths()
    {
        paths.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var path in paths)
        {
            for (int i = 0; i < path.Length - 1; i++)
            {
                Gizmos.DrawLine(path[i], path[i + 1]);
            }
        }
    }
}
