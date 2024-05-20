using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ECS
{
    class PathfindingAStar
    {
        private static readonly Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(-1, 0),
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 1), new Vector2Int(-1, -1),
        new Vector2Int(1, -1), new Vector2Int(-1, 1)
    };

        public static List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, bool[,] map)
        {
            var openSet = new PriorityQueue<Vector2Int, float>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScore = new Dictionary<Vector2Int, float>() { [start] = 0 };
            var fScore = new Dictionary<Vector2Int, float>() { [start] = Heuristic(start, goal) };

            openSet.Enqueue(start, fScore[start]);

            int iterations = 0;

            while (openSet.Count > 0)
            {
                iterations++;
                var current = openSet.Dequeue();

                if (current == goal)
                {
                    Console.WriteLine($"Количество итераций: {iterations}");
                    return ReconstructPath(cameFrom, current);
                }

                foreach (var neighbor in GetNeighbors(current, map))
                {
                    float tentativeGScore = gScore[current] + Distance(current, neighbor);

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);

                        if (!openSet.UnorderedItems.Any(item => item.Element.Equals(neighbor)))
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            return null; // Путь не найден
        }

        private static float Distance(Vector2Int a, Vector2Int b)
        {
            // Расчет расстояния между соседними точками
            int dx = Math.Abs(a.x - b.x);
            int dy = Math.Abs(a.y - b.y);
            if (dx == 1 && dy == 1)
            {
                return (float)Math.Sqrt(2); // Диагональное расстояние
            }
            else
            {
                return 1; // Горизонтальное или вертикальное расстояние
            }
        }

        static IEnumerable<Vector2Int> GetNeighbors(Vector2Int point, bool[,] map)
        {
            foreach (var dir in directions)
            {
                var next = new Vector2Int(point.x + dir.x, point.y + dir.y);
                if (next.x >= 0 && next.x < map.GetLength(0) && next.y >= 0 && next.y < map.GetLength(1) && map[next.x, next.y])
                {
                    if (dir.x != 0 && dir.y != 0) // Диагональное направление
                    {
                        if (map[point.x + dir.x, point.y] && map[point.x, point.y + dir.y])
                        {
                            yield return next;
                        }
                    }
                    else // Прямое направление
                    {
                        yield return next;
                    }
                }
            }
        }

        static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var totalPath = new List<Vector2Int> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }

        static float Heuristic(Vector2Int a, Vector2Int b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

}
