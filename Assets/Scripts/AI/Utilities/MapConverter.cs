using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Map;

public class MapConverter : MonoBehaviour
{
    private bool[,] passabilityMap;
    public bool[,] Map => passabilityMap;

    private void Start()
    {
        ConvertMap();
    }

    private void ConvertMap()
    {
        var gridManager = GameObject.FindObjectOfType<GridManager>();

        if (gridManager != null)
        {
            var dungeonMap = gridManager.GetDungeonMap();

            if (dungeonMap != null)
            {
                passabilityMap = ConvertToBoolArray(dungeonMap);
                Debug.Log("Карта успешно конвертирована в MapConverter");
            }
            else
            {
                Debug.LogError("Карта подземелья не найдена в GridManager!");
            }
        }
        else
        {
            Debug.LogError("GridManager не найден!");
        }
    }

    public static bool[,] ConvertToBoolArray(DungeonMap dungeonMap)
    {
        int width = dungeonMap.GetWidth();
        int height = dungeonMap.GetHeight();
        Debug.Log($"Width ={width}, height = {height}");
        bool[,] boolArray = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                boolArray[y, x] = dungeonMap.tiles[y, x].isPassable;
            }
        }

        return boolArray;
    }

    //private void OnDrawGizmos()
    //{
    //    if (passabilityMap == null) return;

    //    for (int x = 0; x < passabilityMap.GetLength(0); x++)
    //    {
    //        for (int y = 0; y < passabilityMap.GetLength(1); y++)
    //        {
    //            Gizmos.color = passabilityMap[x, y] ? Color.green : Color.red;
    //            Gizmos.DrawCube(new Vector3(x, y, 0), Vector3.one * 0.9f);
    //        }
    //    }
    //}
}

