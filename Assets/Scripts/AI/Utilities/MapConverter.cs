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
        bool[,] boolArray = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                boolArray[x, y] = dungeonMap.tiles[x, y].isPassable;
            }
        }

        return boolArray;
    }
}

