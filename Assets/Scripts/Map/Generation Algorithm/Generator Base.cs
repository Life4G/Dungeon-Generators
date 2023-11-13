using System.Collections.Generic;
using UnityEngine;

public abstract class GeneratorBase : MonoBehaviour 
{
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    protected int seed = 0;

    protected int GenerateSeed()
    {
        string text = "";
        for (int i = 0; i < 10; i++)
        {
            text += "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789"[UnityEngine.Random.Range(0, "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789".Length)].ToString();
        }
        return ((text == "") ? 0 : text.GetHashCode());
    }

    protected HashSet<Vector2Int> resultPositions;

    public HashSet<Vector2Int> CreateDungeon()
    {
        seed = GenerateSeed();
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(seed);
        resultPositions = GenerateDungeon();
        UnityEngine.Random.state = state;
        return resultPositions;
    }

    public HashSet<Vector2Int> CreateDungeon(int seed)
    {
        this.seed = seed;
        UnityEngine.Random.State state = UnityEngine.Random.state;
        UnityEngine.Random.InitState(seed);
        resultPositions = GenerateDungeon();
        UnityEngine.Random.state = state;
        return resultPositions;
    }

    protected abstract HashSet<Vector2Int> GenerateDungeon();

    public int GetSeed()
    {
        return seed;
    }

}
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
        return cardinalDirectionsList[UnityEngine.Random.Range(0, cardinalDirectionsList.Count)];
    }
}