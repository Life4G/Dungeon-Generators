using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts.Map;

public static class MapConverter
{
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

