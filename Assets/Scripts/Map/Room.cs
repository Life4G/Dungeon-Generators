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

public class Room
{
    public Vector2Int massRoomPos;
    private int sizeX, sizeY;
    private int[,] tiles;
    private bool valid;

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
    }
    public Vector2Int GetPos()
    {
        return massRoomPos;
    }

    public Vector2Int GetPosCenter()
    {
        return new Vector2Int(massRoomPos.x + sizeX / 2, massRoomPos.y + sizeY / 2); ;
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
    public int[,] GetTiles()
    {
        return tiles;
    }
    public void SetTiles(int[,] positions)
    {
        tiles = positions;
    }
    public bool GetValidation()
    {
        return valid;
    }
    public void SetValidation(bool validation)
    {
        valid = validation;
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
        if (this == null || other == null)
            return false;

        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);

        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        if (collisonX1 > collisonX2 || collisonY1 > collisonY2)
            return false;

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
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

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    tilesNew[y - massRoomPos.y, x - massRoomPos.x] = 1;
                else
                    tilesNew[y - massRoomPos.y, x - massRoomPos.x] = 0;
            }
        tiles = tilesNew;
        massRoomPos = new Vector2Int(collisonX1, collisonY1);
        sizeX = sizeXNew;
        sizeY = sizeYNew;
    }
    //Операция объединения с другой комнатой
    public void Union(Room other)
    {
        Vector2Int roomPosNew = Vector2Int.Min(massRoomPos, other.massRoomPos);
        int maxX = Mathf.Max(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int maxY = Mathf.Max(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeNewX = maxX - roomPosNew.x + 1;
        int sizeNewY = maxY - roomPosNew.y + 1;


        int[,] tilesNew = new int[sizeNewY, sizeNewX];

        for (int y = other.massRoomPos.y; y < other.massRoomPos.y + other.sizeY; y++)
            for (int x = other.massRoomPos.x; x < other.massRoomPos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 1;
                else
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 0;

            }
        for (int y = massRoomPos.y; y < massRoomPos.y + sizeY; y++)
            for (int x = massRoomPos.x; x < massRoomPos.x + sizeX; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] == 1)
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 1;
                else
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 0;

            }

        tiles = tilesNew;
        sizeX = sizeNewX;
        sizeY = sizeNewY;
        massRoomPos = roomPosNew;
    }
    //Операция разности с другой комнатой
    public void Difference(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    tiles[y - massRoomPos.y, x - massRoomPos.x] = 0;
            }

    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(Room other)
    {
        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);
        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                {
                    tiles[y - massRoomPos.y, x - massRoomPos.x] = 0;
                    other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] = 0;
                }
            }
    }

    public bool Validate()
    {
        if (this == null)
            return false;
        Vector2Int start = new Vector2Int();
        int tilesSum = 0;
        int[,] tilesToCheck = new int[sizeY, sizeX];
        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
                if (tiles[y, x] == 0)
                    tilesToCheck[y, x] = -1;
                else
                {
                    tilesToCheck[y, x] = 0;
                    start.y = y;
                    start.x = x;
                    tilesSum++;
                }
        if (start.x < 0 || start.y < 0)
            return false;


        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);
        int tilesCheckedSum = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;
            while (IsInside(lx - 1, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, lx - 1] = 1;
                lx--;
                tilesCheckedSum++;
            }
            while (IsInside(l.x, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, l.x] = 1;
                l.x++;
                tilesCheckedSum++;
            }
            FindPoints(lx, l.x - 1, l.y + 1, tilesToCheck, stack);
            FindPoints(lx, l.x - 1, l.y - 1, tilesToCheck, stack);

        }
        if (tilesSum == tilesCheckedSum)
            return true;
        return false;
    }

    private void FindPoints(int lx, int rx, int y, int[,] tilesToCheck, Stack<Vector2Int> stack)
    {
        bool isAdded = false;
        for (int x = lx; x < rx; x++)
        {
            if (!IsInside(lx - 1, y, tilesToCheck))
                isAdded = false;
            else if (!isAdded)
            {
                stack.Push(new Vector2Int(x, y));
                isAdded = true;
            }
        }
    }

    private bool IsInside(int x, int y, int[,] tilesToCheck)
    {
        if (x >= 0 && y >= 0 && x < sizeX && y < sizeY && tilesToCheck[y, x] == 0)
            return true;
        return false;
    }

    public bool CheckConnection(Room other)
    {
        if (this == null || other == null)
            return false;
        Vector2Int RoomPosNew = new Vector2Int(Mathf.Min(massRoomPos.x, other.massRoomPos.x), Mathf.Min(massRoomPos.y, other.massRoomPos.y));
        int maxX = Mathf.Max(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int maxY = Mathf.Max(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeNewX = maxX - RoomPosNew.x + 1;
        int sizeNewY = maxY - RoomPosNew.y + 1;

        int tilesSum = 0;
        int startX = 0; int startY = 0;


        int[,] tilesToCheck = new int[sizeNewY, sizeNewX];

        for (int y = other.massRoomPos.y; y < other.massRoomPos.y + other.sizeY; y++)
            for (int x = other.massRoomPos.x; x < other.massRoomPos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                {
                    tilesToCheck[y - other.massRoomPos.y, x - other.massRoomPos.x] = 1;
                    tilesSum++;
                }
                else
                    tilesToCheck[y - other.massRoomPos.y, x - other.massRoomPos.x] = 0;

            }
        for (int y = massRoomPos.y; y < massRoomPos.y + sizeY; y++)
            for (int x = massRoomPos.x; x < massRoomPos.x + sizeX; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] == 1)
                {
                    tilesToCheck[y - massRoomPos.y, x - massRoomPos.x] = 1;
                    tilesSum++;
                    startY = y - massRoomPos.y;
                    startX = x - massRoomPos.x;
                }
                else
                    tilesToCheck[y - massRoomPos.y, x - massRoomPos.x] = 0;

            }

        if (tilesSum == 0)
            return false;

        if (startX < 0 || startY < 0)
            return false;


        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(new Vector2Int(startX, startY));
        int tilesCheckedSum = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;
            while (IsInside(lx - 1, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, lx - 1] = 1;
                lx--;
                tilesCheckedSum++;
            }
            while (IsInside(l.x, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, l.x] = 1;
                l.x++;
                tilesCheckedSum++;
            }
            FindPoints(lx, l.x - 1, l.y + 1, tilesToCheck, stack);
            FindPoints(lx, l.x - 1, l.y - 1, tilesToCheck, stack);

        }
        if (tilesSum == tilesCheckedSum)
            return true;
        return false;
    }

}