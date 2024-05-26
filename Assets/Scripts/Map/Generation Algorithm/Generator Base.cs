using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Базовый класс для генераторов от которого наследуются все остальные
public abstract class DungeonGeneratorBase : MonoBehaviour
{
    //Стартовая позиция равная 0 (можно поменять из редактора)
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    //Сид
    [SerializeField]
    protected int seed = 0;
    [SerializeField]
    public WallsGenerator wallsGenerator;
    protected Graph graph;

    //Функция по генерации сида
    public int GenerateSeed()
    {
        string text = "";
        //Выбираем рандомно 10 букв и кидаем их в строку
        for (int i = 0; i < 10; i++)
        {
            text += "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789"[UnityEngine.Random.Range(0, "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789".Length)].ToString();
        }
        //строку преобразовываем в хешкод
        return ((text == "") ? 0 : text.GetHashCode());
    }
    //массив в котором валяются все плитки (а вернее их координаты)
    protected int[,] resultPositions;

    //Функция создания данжа которая вызывается по нажатию кнопки
    public int[,] CreateDungeon()
    {
        seed = GenerateSeed();
        //Эта штука нужно чтобы запомнить состояние рандома до вызова генерации
        //нужно чтобы в случае чего использовать одновременно несколько сидов под разные объекты и исключить возможность их взаимодействия между собой
        //писать полную идею не буду можешь просто погуглить про рандом unity либо потом спроси объясню
        Random.State state = Random.state;
        //Теперь задаем сиид
        Random.InitState(seed);
        //Вызываем генерацию данжей
        resultPositions = GenerateDungeon();
        //Возвращаем рандом до задания сида
        Random.state = state;
        return resultPositions;
    }

    //Функция создания данжа которая вызывается по нажатию кнопки (на этот раз есть сид)
    public int[,] CreateDungeon(int seed)
    {
        this.seed = seed;
        //Эта штука нужно чтобы запомнить состояние рандома до вызова генерации
        //нужно чтобы в случае чего использовать одновременно несколько сидов под разные объекты и исключить возможность их взаимодействия между собой
        //писать полную идею не буду можешь просто погуглить про рандом unity либо потом спроси объясню
        Random.State state = Random.state;
        //Теперь задаем сиид
        Random.InitState(seed);
        //Вызываем генерацию данжей
        resultPositions = GenerateDungeon();
        //Возвращаем рандом до задания сида
        Random.state = state;
        return resultPositions;
    }

    //Обстрактная функция которая создана другими объектами (наследники обязаны overridить эту функцию иначе жопа генерации ведь всё вкусное там)
    protected abstract int[,] GenerateDungeon();

    public virtual int GetRoomStyle(int id)
    {
        return 1;
    }

    public int GetSeed()
    {
        return seed;
    }
    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    public Graph GetGraph()
    { return graph; }

}

//Статический класс который возвращает рандомное движение (вверх вниз и т.д)
public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }
    public static List<Vector2Int> GetNewCardinalPosesFromPos(Vector2Int pos)
    {
        List<Vector2Int> newPoses = new List<Vector2Int>();
        foreach (Vector2Int direction in cardinalDirectionsList)
        {
            newPoses.Add(pos + direction);
        }
        return newPoses;
    }

}