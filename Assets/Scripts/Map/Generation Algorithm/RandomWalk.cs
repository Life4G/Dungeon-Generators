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
    public static int mapMaxHeight = 512;
    public static int mapMaxWidth = 512;
    private int[,] map = new int[mapMaxHeight, mapMaxWidth];
    protected override int[,] GenerateDungeon()
    {
        return Run();
    }

    protected int[,] Run()
    {
        var currentPosition = new Vector2Int(mapMaxWidth / 2, mapMaxHeight / 2);
        int[,] floorPositions = new int[mapMaxHeight, mapMaxWidth];
        //Цикл в котором мы вызываем рандомное хождение несколько раз
        for (int i = 0; i < iterations; i++)
        {
            //Создаем путь
            var previousPosition = startPosition;
            //Ходим в рандомные стороны определенное кол-во шагов
            for (int j = 0; j < length; j++)
            {
                var newPosition = previousPosition;
                do
                {
                    newPosition += Direction2D.GetRandomCardinalDirection();
                } while (newPosition.x < 0 && newPosition.y < 0 && newPosition.x>mapMaxWidth && newPosition.y> mapMaxHeight);
                floorPositions[newPosition.y, newPosition.x] = 0;
                previousPosition = newPosition;
            }
            //Выбираем рандомную точку из которого будем повторять алгоритм
            currentPosition = new Vector2Int(Random.Range(0, mapMaxHeight), Random.Range(0, mapMaxWidth));
        }
        return floorPositions;
    }

}