using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Генерация через рандомное хождение (идея? берем точку и идем из неё куда-то. Потом берем другую из имеющихся и идем опять куда-то и так неск. раз)
public class RandomWalk : DungeonGeneratorBase
{
    //Кол-во повторений алгоритма
    [SerializeField]
    private int iterations = 10;
    //Кол-во шагов которые сделает алгоритм
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
        //Цикл в котором мы вызываем рандомное хождение несколько раз
        for (int i = 0; i < iterations; i++)
        {
            //Создаем путь
            var path = GetPath(currentPosition, length);
            //Объединяем либо с пустым либо с уже имеющимся
            floorPositions.UnionWith(path);
            //Выбираем рандомную точку из которого будем повторять алгоритм
            currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }

    private static HashSet<Vector2Int> GetPath(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add(startPosition);
        var previousPosition = startPosition;
        //Ходим в рандомные стороны определенное кол-во шагов
        for (int i = 0; i < walkLength; i++)
        {   
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousPosition = newPosition;
        }
        return path;
    }

}