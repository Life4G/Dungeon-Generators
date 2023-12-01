using System.Collections.Generic;
using UnityEngine;

public class WallsGenerator : MonoBehaviour
{
    // Направления, в т.ч. по диагоналям
    private static readonly Vector2Int[] AllDirections = {
        new Vector2Int(0, 1),   // Вверх
        new Vector2Int(1, 1),   // Вверх-вправо
        new Vector2Int(1, 0),   // Вправо
        new Vector2Int(1, -1),  // Вниз-вправо
        new Vector2Int(0, -1),  // Вниз
        new Vector2Int(-1, -1), // Вниз-влево
        new Vector2Int(-1, 0),  // Влево
        new Vector2Int(-1, 1)   // Вверх-влево
    };

    /// <summary>
    /// Генерирует набор позиций для стен на основе позиций пола.
    /// </summary>
    /// <param name="floorPositions">Набор позиций пола, вокруг которых необходимо создать стены.</param>
    /// <returns>Набор позиций стен, созданных вокруг каждой позиции пола.</returns>
    public HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (Vector2Int position in floorPositions)
        {
            // Проверяем каждое направление вокруг тайла
            foreach (Vector2Int direction in AllDirections)
            {
                Vector2Int neighbourPosition = position + direction;

                // Если соседний тайл не является частью пола, здесь должна быть стена
                // Проверка на добавление только новых стен
                if (!floorPositions.Contains(neighbourPosition) && !wallPositions.Contains(neighbourPosition))
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        // Возвращаем позиции, где должны быть стены
        return wallPositions;
    }
}
