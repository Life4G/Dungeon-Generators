using Assets.Scripts.Map;
using UnityEngine;

public class WallsGenerator
{
    /// <summary>
    /// Все возможные направления для проверки соседних плиток.
    /// </summary>
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

    /// <summary>
    /// Генерирует двумерный массив позиций для стен на основе двумерного массива позиций пола.
    /// </summary>
    /// <param name="floorArray">Двумерный массив, представляющий карту пола.</param>
    /// <returns>Двумерный массив, представляющий карту стен.</returns>
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
                wallArray[y, x] = -1;
            }
        }

        // стены вокруг пола
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (floorArray[y, x] != -1) // если на этой позиции есть пол
                {
                    foreach (Vector2Int direction in AllDirections)
                    {
                        int neighbourX = x + direction.x;
                        int neighbourY = y + direction.y;

                        // проверка на выход за границы массива
                        if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                        {
                            // если рядом нет пола
                            if (floorArray[neighbourY, neighbourX] == -1 && wallArray[neighbourY, neighbourX] == -1)
                            {
                                wallArray[neighbourY, neighbourX] = floorArray[y, x]; // присвоить стиль стены, как у пола
                            }
                        }
                    }
                }
            }
        }

        return wallArray;
    }

    /// <summary>
    /// Генерирует двумерный массив позиций для стен, используя карту подземелья.
    /// </summary>
    /// <param name="dungeonMap">Карта подземелья, содержащая информацию о поле.</param>
    /// <returns>Двумерный массив, представляющий карту стен.</returns>
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
                wallArray[y, x] = -1;
            }
        }

        // стены вокруг пола
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (dungeonMap.tiles[y, x].roomIndex != -1) // если на этой позиции есть пол
                {
                    foreach (Vector2Int direction in AllDirections)
                    {
                        int neighbourX = x + direction.x;
                        int neighbourY = y + direction.y;

                        // проверка на выход за границы массива
                        if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                        {
                            // если рядом нет пола
                            if (dungeonMap.tiles[neighbourY, neighbourX].roomIndex == -1 && wallArray[neighbourY, neighbourX] == -1)
                            {
                                wallArray[neighbourY, neighbourX] = dungeonMap.tiles[y, x].roomIndex; // присвоить стиль стены, как у пола
                            }
                        }
                    }
                }
            }
        }

        return wallArray;
    }
}
