using UnityEngine;

public class WallsGenerator : GeneratorBase
{
    private static readonly Vector2Int[] AllDirections = {
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1)
    };

<<<<<<< Updated upstream
    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Генерирует набор позиций для стен на основе позиций пола.
    /// </summary>
    /// <param name="floorPositions">Набор позиций пола, вокруг которых необходимо создать стены.</param>
    /// <returns>Набор позиций стен, созданных вокруг каждой позиции пола.</returns>
    public HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions)
=======
    // генерирует двумерный массив позиций для стен на основе двумерного массива позиций пола
    public int[,] GenerateWallsFromFloor(int[,] floorArray)
>>>>>>> Stashed changes
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
}
