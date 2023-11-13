using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalk : GeneratorBase
{

    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    private int length = 10;

    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        return Run();
    }

    protected HashSet<Vector2Int> Run()
    {
        var currentPosition = startPosition;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            var path = GetPath(currentPosition, length);
            floorPositions.UnionWith(path);
            currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

    public static HashSet<Vector2Int> GetPath(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

}