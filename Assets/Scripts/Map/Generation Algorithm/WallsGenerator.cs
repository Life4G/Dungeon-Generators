using System.Collections.Generic;
using UnityEngine;

public class WallsGenerator : GeneratorBase
{
    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        throw new System.NotImplementedException();
    }

    // Создания стен
    public HashSet<Vector2Int> CreateWalls(HashSet<Vector2Int> floorPositions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (Vector2Int position in floorPositions)
        {
            // Проверяем каждое направление вокруг тайла
            foreach (Vector2Int direction in Direction2D.cardinalDirectionsList)
            {
                Vector2Int neighbourPosition = position + direction;
                // Если соседний тайл не является частью пола, здесь должна быть стена
                if (!floorPositions.Contains(neighbourPosition))
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
        }

        // Возвращаем позиции, где должны быть стены
        return wallPositions;
    }
    
}
