using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// Базовый класс для генераторов подземелий, от которого наследуются все остальные.
/// </summary>
public abstract class DungeonGeneratorBase : MonoBehaviour
{
    /// <summary>
    /// Стартовая позиция, равная 0 (меняется в редакторе).
    /// </summary>
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;

    /// <summary>
    /// Сид для генерации случайных чисел.
    /// </summary>
    [SerializeField]
    protected int seed = 0;

    /// <summary>
    /// Генератор стен.
    /// </summary>
    [SerializeField]
    public WallsGenerator wallsGenerator;

    /// <summary>
    /// Граф комнат подземелья.
    /// </summary>
    protected Graph graph;

    /// <summary>
    /// Массив, содержащий координаты всех плиток.
    /// </summary>
    protected int[,] resultPositions;

    public int roomShapesId;

    protected List<int> roomShapes;

    public void SetRoomShapes(int shapes)
    {
        roomShapesId = shapes;
        roomShapes = new List<int>();
        if (shapes != -1)
            for (int i = 0; i < sizeof(int) * 8; i++)
            {
                if ((shapes & (1 << i)) > 0)
                    roomShapes.Add(i);
            }
        else
        {
            for (int i = 0; i < 4; i++)
                roomShapes.Add(i);
        }
    }

    /// <summary>
    /// Генератор сида.
    /// </summary>
    /// <returns>Случайный сид.</returns>
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

    /// <summary>
    /// Генерирует подземелье.
    /// </summary>
    /// <returns>Массив плиток подземелья.</returns>
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

    /// <summary>
    /// Генерация подземелья по заданному сиду.
    /// </summary>
    /// <param name="seed">Сид для генерации подземелья.</param>
    /// <returns>Массив плиток подземелья.</returns>
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

    /// <summary>
    /// Абстрактная функция для генерации подземелья, которую должны реализовать наследники.
    /// </summary>
    /// <returns>Массив координат плиток подземелья.</returns>
    protected abstract int[,] GenerateDungeon();

    public virtual int GetRoomStyle(int id)
    {
        return 1;
    }

    /// <summary>
    /// Возвращает текущий сид.
    /// </summary>
    /// <returns>Текущий сид.</returns>
    public int GetSeed()
    {
        return seed;
    }

    /// <summary>
    /// Устанавливает сид для генерации.
    /// </summary>
    /// <param name="seed">Сид для генерации.</param>
    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    /// <summary>
    /// Возвращает граф комнат подземелья.
    /// </summary>
    /// <returns>Граф комнат подземелья.</returns>
    public Graph GetGraph()
    { return graph; }

}

/// <summary>
/// Статический класс, который возвращает случайное направление (вверх, вниз и т.д.).
/// </summary>
public static class Direction2D
{
    /// <summary>
    /// Список направлений.
    /// </summary>
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0)
    };

    /// <summary>
    /// Возвращает случайное направление.
    /// </summary>
    /// <returns>Случайное направление.</returns>
    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionsList[Random.Range(0, cardinalDirectionsList.Count)];
    }

    /// <summary>
    /// Возвращает новые позиции на основе текущей позиции и направлений.
    /// </summary>
    /// <param name="pos">Текущая позиция.</param>
    /// <returns>Список новых позиций.</returns>
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