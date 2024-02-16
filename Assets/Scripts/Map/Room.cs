using static SetOperations;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public static class SetOperations
{
    public enum Operations
    {
        None,
        Intersect,
        Union,
        DifferenceAB,
        DifferenceBA,
        SymmetricDifference,
    }
    public static Operations GetRandomOperation()
    {
        return (Operations)Random.Range(1, 4);
    }
    public static Operations GetRandomSubOperation()
    {
        return (Operations)Random.Range(2, 4);
    }
    public static List<Operations> GetOperationsList = new List<Operations>
    {
        Operations.Intersect,
        Operations.Union,
        Operations.DifferenceAB,
        Operations.DifferenceBA,
        Operations.SymmetricDifference

    };
    public static List<Operations> GetSubOperationsList = new List<Operations>
    {
        Operations.Union,
        Operations.DifferenceAB,
        Operations.DifferenceBA,
    };

}

//Возможные стили
public enum Styles
{
    None, Style1,
}

public class Room
{
    public Vector2Int massRoomPos;
    private int sizeX, sizeY;
    private int[,] tiles;
    private bool valid;
    private Styles style;

    public Room(Vector2Int massRoomPos, int sizeX, int sizeY, int[,] tiles)
    {
        this.massRoomPos = massRoomPos;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.tiles = tiles;
        this.valid = false;
    }

    public Room(Room room)
    {
        massRoomPos = new Vector2Int(massRoomPos.x, massRoomPos.y);
        sizeX = room.sizeX;
        sizeY = room.sizeY;
        tiles = room.tiles.Clone() as int[,];
        this.valid = false;
        this.style = Styles.Style1;
    }
    public Vector2Int GetPos()
    {
        return massRoomPos;
    }
    public Size GetSize()
    {
        return new Size(sizeX, sizeY);
    }
    public void SetPos(int x, int y)
    {
        massRoomPos = new Vector2Int(x, y);
    }
    public void SetPos(Vector2Int pos)
    {
        massRoomPos = pos;
    }
    public Styles GetStyle()
    {
        return style;
    }
    public void SetStyle(Styles style)
    {
        this.style = style;
    }
    public int[,] GetTiles()
    {
        return tiles;
    }
    public void SetTiles(int[,] positions)
    {
        tiles = positions;
    }
    public void SetSize(int width, int height)
    {
        sizeX = width;
        sizeY = height;
    }

    public void SetSize(int size)
    {
        sizeX = size;
        sizeY = size;
    }
    //Функция проверки пересечения
    public bool CheckIntersection(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);

        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        if (collisonX1 > collisonX2 || collisonY1 > collisonY2)
            return false;
        int y = 0;
        int x = 0;


        for (y = collisonY1; y < collisonY2; y++)
            for (x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    return true;
            }

        return false;
    }
    public bool IsProperSubsetOf(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        if (collisonX1 > collisonX2 || collisonY1 > collisonY2)
            return false;
        return true;
    }
    //Операция пересечения с другой комнатой
    public void Intersect(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeXNew = collisonX2 - collisonX1 + 1;
        int sizeYNew = collisonY2 - collisonY1 + 1;
        int[,] tilesNew = new int[sizeYNew, sizeXNew];

        Vector2Int offset = Vector2Int.Min(massRoomPos, other.massRoomPos);
        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - offset.y, x - offset.x] == 1)
                    tilesNew[y, x] = 1;
                else
                    tilesNew[y, x] = 0;
            }
        tiles = tilesNew;
        massRoomPos = new Vector2Int(collisonX1, collisonY1);
        sizeX = sizeXNew;
        sizeY = sizeYNew;
    }
    //Операция объединения с другой комнатой
    public void Union(Room other)
    {
        Vector2Int RoomPosNew = Vector2Int.Min(massRoomPos, other.massRoomPos);
        int maxX = Mathf.Max(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int maxY = Mathf.Max(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeNewX = maxX - RoomPosNew.x + 1;
        int sizeNewY = maxY - RoomPosNew.y + 1;


        int[,] tilesNew = new int[sizeNewY, sizeNewX];

        for (int y = other.massRoomPos.y; y < other.massRoomPos.y + other.sizeY; y++)
            for (int x = other.massRoomPos.x; x < other.massRoomPos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    tilesNew[y - RoomPosNew.y, x - RoomPosNew.x] = 1;
                else
                    tilesNew[y - RoomPosNew.y, x - RoomPosNew.x] = 0;

            }
        for (int y = massRoomPos.y; y < massRoomPos.y + sizeY; y++)
            for (int x = massRoomPos.x; x < massRoomPos.x + sizeX; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] == 1)
                    tilesNew[y - RoomPosNew.y, x - RoomPosNew.x] = 1;
                else
                    tilesNew[y - RoomPosNew.y, x - RoomPosNew.x] = 0;

            }

        tiles = tilesNew;
        sizeX = sizeNewX;
        sizeY = sizeNewY;
        massRoomPos = RoomPosNew;
    }
    //Операция разности с другой комнатой
    public void DifferenceAB(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        Vector2Int offset = Vector2Int.Min(massRoomPos, other.massRoomPos);
        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - offset.y, x - offset.x] == 1)
                    tiles[y, x] = 0;
            }

    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        Vector2Int offset = Vector2Int.Min(massRoomPos, other.massRoomPos);
        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - offset.y, x - offset.x] == 1)
                {
                    tiles[y - massRoomPos.y, x - massRoomPos.x] = 0;
                    other.tiles[y - offset.y, x - offset.x] = 0;
                }
            }
    }

    public bool Validate()
    {
        int[,] tilesToValidate = new int[sizeY, sizeX];
        bool run = true;
        int startY = 0, startX = 0;
        int step = 0;
        int tilesTotalNum = 0;

        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                if (tiles[y, x] == 0)
                    tilesToValidate[y, x] = -2;
                else
                {
                    tilesToValidate[y, x] = -1;
                    startY = y; startX = x;
                    tilesTotalNum++;
                }
            }

        if (tilesTotalNum == 0)
            return false;
        tilesToValidate[startY, startX] = 0;

        int tilesNum = 0;
        while (run == true)
        {
            run = false;
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {
                    if (tilesToValidate[y, x] == step)
                    {
                        if (y - 1 >= 0 && tilesToValidate[y - 1, x] == -1)
                        {
                            tilesToValidate[y - 1, x] = step + 1;
                            tilesNum++; run = true;
                        }
                        if (x - 1 >= 0 && tilesToValidate[y, x - 1] == -1)
                        {
                            tilesToValidate[y, x - 1] = step + 1;
                            tilesNum++; run = true;
                        }
                        if (y + 1 < sizeY && tilesToValidate[y + 1, x] == -1)
                        {
                            tilesToValidate[y + 1, x] = step + 1;
                            tilesNum++; run = true;
                        }

                        if (x + 1 < sizeX && tilesToValidate[y, x + 1] == -1)
                        {
                            tilesToValidate[y, x + 1] = step + 1;
                            tilesNum++; run = true;
                        }
                    }
                }

            if (tilesNum == tilesTotalNum)
                return true;

        }
        return false;
    }

    public bool CheckConnection(Room other)
    {
        Vector2Int RoomPosNew = new Vector2Int(Mathf.Min(massRoomPos.x, other.massRoomPos.x), Mathf.Min(massRoomPos.y, other.massRoomPos.y));
        int maxX = Mathf.Max(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int maxY = Mathf.Max(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeNewX = maxX - RoomPosNew.x + 1;
        int sizeNewY = maxY - RoomPosNew.y + 1;

        int tilesTotalNum = 0;
        int startX = 0; int startY = 0;


        int[,] tilesToValidate = new int[sizeNewY, sizeNewX];

        Vector2Int offsetOther = Vector2Int.Min(RoomPosNew, other.massRoomPos);
        for (int y = other.massRoomPos.y; y < other.massRoomPos.y + other.sizeY; y++)
            for (int x = other.massRoomPos.x; x < other.massRoomPos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                {
                    tilesToValidate[y - offsetOther.y, x - offsetOther.x] = 1;
                    tilesTotalNum++;
                }
                else
                    tilesToValidate[y - offsetOther.y, x - offsetOther.x] = 0;

            }
        Vector2Int offset = Vector2Int.Min(RoomPosNew, massRoomPos);
        for (int y = massRoomPos.y; y < massRoomPos.y + sizeY; y++)
            for (int x = massRoomPos.x; x < massRoomPos.x + sizeX; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] == 1)
                {
                    tilesToValidate[y - offset.y, x - offset.x] = 1;
                    tilesTotalNum++;
                    startY = y - offset.y;
                    startX = x - offset.x;
                }
                else
                    tilesToValidate[y - offset.y, x - offset.x] = 0;

            }

        if (tilesTotalNum == 0)
            return false;

        tilesToValidate[startY, startX] = 0;
        int tilesNum = 0;
        bool run = true;
        int step = 0;
        while (run == true)
        {
            run = false;
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {
                    if (tilesToValidate[y, x] == step)
                    {

                        if (y - 1 >= 0 && tilesToValidate[y - 1, x] == -1)
                        {
                            tilesToValidate[y - 1, x] = step + 1;
                            tilesNum++; run = true;
                        }
                        if (x - 1 >= 0 && tilesToValidate[y, x - 1] == -1)
                        {
                            tilesToValidate[y, x - 1] = step + 1;
                            tilesNum++; run = true;
                        }
                        if (y + 1 < sizeY && tilesToValidate[y + 1, x] == -1)
                        {
                            tilesToValidate[y + 1, x] = step + 1;
                            tilesNum++; run = true;
                        }

                        if (x + 1 < sizeX && tilesToValidate[y, x + 1] == -1)
                        {
                            tilesToValidate[y, x + 1] = step + 1;
                            tilesNum++; run = true;
                        }
                    }
                }
        }
        if (tilesNum == tilesTotalNum)
            return true;
        return false;
    }

    public void DoOperation(Room other, SetOperations.Operations operation)
    {
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                Intersect(other);
                break;
            case SetOperations.Operations.Union:
                Union(other);
                break;
            case SetOperations.Operations.DifferenceAB:
                DifferenceAB(other);
                break;
                case SetOperations.Operations.SymmetricDifference:
                    SymmetricDifference(other);
                    break;
        }
    }
    public bool TryOperation(Room other, SetOperations.Operations operation)
    {
        Room roomTest = new Room(other);
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                roomTest.Intersect(other);
                break;
            case SetOperations.Operations.Union:
                roomTest.Union(other);
                break;
            case SetOperations.Operations.DifferenceAB:
                roomTest.DifferenceAB(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                roomTest.SymmetricDifference(other);
                break;
        }
        return roomTest.Validate();
    }
    public SetOperations.Operations TryAllOperations(Room other)
    {
        foreach (SetOperations.Operations op in GetOperationsList)
        {
            if (TryOperation(other, op))
            {
                return op;
            }
        }
        return Operations.None;
    }
    public SetOperations.Operations TryAllSubOperations(Room other)
    {
        foreach (SetOperations.Operations op in GetSubOperationsList)
        {
            if (TryOperation(other, op))
            {
                return op;
            }
        }
        return Operations.None;
    }
}