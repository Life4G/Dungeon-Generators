using Assets.Scripts.Map;
using UnityEngine;

public class WallsGenerator : MonoBehaviour
{
    private static readonly Vector2Int[] AllDirections = {
        new Vector2Int(0, 1),   // вверх
        new Vector2Int(1, 1),   // вверх-вправо
        new Vector2Int(1, 0),   // вправо
        new Vector2Int(1, -1),  // вниз-вправо
        new Vector2Int(0, -1),  // вниз
        new Vector2Int(-1, -1), // вниз-влево
        new Vector2Int(-1, 0),  // влево
        new Vector2Int(-1, 1)   // вверх-влево 
    };

    // генерирует двумерный массив позиций для стен на основе двумерного массива позиций пола
    public int[,] GenerateWallsFromFloor(int[,] floorArray)
    {
        int width = floorArray.GetLength(0);
        int height = floorArray.GetLength(1);
        int[,] wallArray = new int[width, height];

        // инициализация массива стен значениями -1 (стен нет)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                wallArray[x, y] = -1;
            }
        }

        // стены вокруг пола
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (floorArray[x, y] != -1) // если на этой позиции есть пол
                {
                    foreach (Vector2Int direction in AllDirections)
                    {
                        int neighbourX = x + direction.x;
                        int neighbourY = y + direction.y;

                        // проверка на выход за границы массива
                        if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                        {
                            // если рядом нет пола
                            if (floorArray[neighbourX, neighbourY] == -1 && wallArray[neighbourX, neighbourY] == -1)
                            {
                                wallArray[neighbourX, neighbourY] = floorArray[x, y]; // присвоить стиль стены, как у пола
                            }
                        }
                    }
                }
            }
        }

        return wallArray;
    }

    public int[,] GenerateWallsFromDungeonMap(DungeonMap dungeonMap)
    {
        int width = dungeonMap.GetWidth();
        int height = dungeonMap.GetHeight();
        int[,] wallArray = new int[width, height];

        // инициализация массива стен значениями -1 (стен нет)
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                wallArray[x, y] = -1;
            }
        }

        // стены вокруг пола
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (dungeonMap.tiles[x, y].roomIndex != -1) // если на этой позиции есть пол
                {
                    foreach (Vector2Int direction in AllDirections)
                    {
                        int neighbourX = x + direction.x;
                        int neighbourY = y + direction.y;

                        // проверка на выход за границы массива
                        if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                        {
                            // если рядом нет пола
                            if (dungeonMap.tiles[neighbourX, neighbourY].roomIndex == -1 && wallArray[neighbourX, neighbourY] == -1)
                            {
                                wallArray[neighbourX, neighbourY] = dungeonMap.tiles[x, y].roomIndex; // присвоить стиль стены, как у пола
                            }
                        }
                    }
                }
            }
        }

        return wallArray;
    }
}
