using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Room
{
    /// <summary>
    /// Позиция комнаты на карте.
    /// </summary>
    public Vector2Int massRoomPos;

    /// <summary>
    /// Ширина комнаты.
    /// </summary>
    private int sizeX;

    /// <summary>
    /// Высота комнаты.
    /// </summary>
    private int sizeY;

    /// <summary>
    /// Двумерный массив, представляющий плитки комнаты.
    /// </summary>
    private int[,] tiles;

    /// <summary>
    /// Флаг, указывающий, является ли комната валидной (допустимой).
    /// </summary>
    private bool valid;

    /// <summary>
    /// Минимальное количество плиток для валидной комнаты.
    /// </summary>
    private static readonly int tilesMin = 8;

    /// <summary>
    /// Конструктор комнаты.
    /// </summary>
    /// <param name="massRoomPos">Позиция комнаты.</param>
    /// <param name="sizeX">Ширина комнаты.</param>
    /// <param name="sizeY">Высота комнаты.</param>
    /// <param name="tiles">Плитки комнаты.</param>
    public Room(Vector2Int massRoomPos, int sizeX, int sizeY, int[,] tiles)
    {
        this.massRoomPos = massRoomPos;
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.tiles = tiles;
        this.valid = false;
    }

    /// <summary>
    /// Конструктор копирования комнаты.
    /// </summary>
    /// <param name="room">Комната для копирования.</param>
    public Room(Room room)
    {
        massRoomPos = new Vector2Int(massRoomPos.x, massRoomPos.y);
        sizeX = room.sizeX;
        sizeY = room.sizeY;
        tiles = room.tiles.Clone() as int[,];
        valid = false;
    }

    /// <summary>
    /// Возвращает позицию комнаты.
    /// </summary>
    /// <returns>Позиция комнаты.</returns>
    public Vector2Int GetPos()
    {
        return massRoomPos;
    }

    /// <summary>
    /// Возвращает позицию центра комнаты.
    /// </summary>
    /// <returns>Позиция центра комнаты.</returns>
    public Vector2Int GetPosCenter()
    {
        return new Vector2Int(massRoomPos.x + sizeX / 2, massRoomPos.y + sizeY / 2); ;
    }

    /// <summary>
    /// Возвращает размер комнаты.
    /// </summary>
    /// <returns>Размер комнаты.</returns>
    public Size GetSize()
    {
        return new Size(sizeX, sizeY);
    }

    /// <summary>
    /// Устанавливает позицию комнаты.
    /// </summary>
    /// <param name="x">X координата.</param>
    /// <param name="y">Y координата.</param>
    public void SetPos(int x, int y)
    {
        massRoomPos = new Vector2Int(x, y);
    }

    /// <summary>
    /// Устанавливает позицию комнаты.
    /// </summary>
    /// <param name="pos">Позиция комнаты.</param>
    public void SetPos(Vector2Int pos)
    {
        massRoomPos = pos;
    }

    /// <summary>
    /// Возвращает плитки комнаты.
    /// </summary>
    /// <returns>Плитки комнаты.</returns>
    public int[,] GetTiles()
    {
        return tiles;
    }

    /// <summary>
    /// Устанавливает плитки комнаты.
    /// </summary>
    /// <param name="positions">Плитки комнаты.</param>
    public void SetTiles(int[,] positions)
    {
        tiles = positions;
    }

    /// <summary>
    /// Возвращает валидность комнаты.
    /// </summary>
    /// <returns>Валидность комнаты.</returns>
    public bool GetValidation()
    {
        return valid;
    }

    /// <summary>
    /// Устанавливает валидность комнаты.
    /// </summary>
    /// <param name="validation">Флаг валидности.</param>
    public void SetValidation(bool validation)
    {
        valid = validation;
    }

    /// <summary>
    /// Устанавливает размер комнаты.
    /// </summary>
    /// <param name="width">Ширина комнаты.</param>
    /// <param name="height">Высота комнаты.</param>
    public void SetSize(int width, int height)
    {
        sizeX = width;
        sizeY = height;
    }

    /// <summary>
    /// Устанавливает размер комнаты.
    /// </summary>
    /// <param name="size">Размер комнаты.</param>
    public void SetSize(int size)
    {
        sizeX = size;
        sizeY = size;
    }

    /// <summary>
    /// Проверяет пересечение с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
    /// <returns>True - комнаты пересекаются, иначе - False.</returns>
    public bool CheckIntersection(Room other)
    {
        if (this == null || other == null)
            return false;

        int collisonX1 = Mathf.Max(massRoomPos.x, other.massRoomPos.x);
        int collisonY1 = Mathf.Max(massRoomPos.y, other.massRoomPos.y);

        int collisonX2 = Mathf.Min(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int collisonY2 = Mathf.Min(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        if (collisonX1 >= collisonX2 || collisonY1 >= collisonY2)
            return false;

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] * other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] == 1)
                    return true;
            }

        return false;
    }

    /// <summary>
    /// Проверяет, является ли данная комната подмножеством другой комнаты.
    /// </summary>
    /// <param name="other">Другая комната.</param>
    /// <returns>True - данная комната является подмножеством, иначе - False.</returns>
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

    /// <summary>
    /// Операция пересечения с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
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

    /// <summary>
    /// Операция объединения с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
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

    /// <summary>
    /// Операция разности с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
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

    /// <summary>
    /// Операция симметричной разности с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
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

    /// <summary>
    /// Проверяет валидность комнаты.
    /// </summary>
    /// <returns>True - комната валидна, иначе - False.</returns>
    public bool Validate()
    {
        if (this == null)
            return false;

        Vector2Int start = new Vector2Int();
        int tilesSum = 0;

        int[,] tilesToCheck = new int[sizeY, sizeX];
        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
                if (tiles[y, x] != 0)
                {
                    tilesToCheck[y, x] = 1;
                    start.y = y;
                    start.x = x;
                    tilesSum++;
                }


        if (start.x < 0 || start.y < 0 || tilesSum < tilesMin)
            return false;


        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);
        int tilesCheckedSum = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;

            while (IsInside(lx, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, lx] = 2;
                lx--;
                tilesCheckedSum++;
            }
            int rx = l.x + 1;

            while (IsInside(rx, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, rx] = 2;
                tilesCheckedSum++;
                rx++;
            }

            for (int i = lx; i < rx-1; i++)
                if (IsInside(i, l.y + 1, tilesToCheck))
                    stack.Push(new Vector2Int(i, l.y + 1));
            for (int i = lx; i < rx-1; i++)
                if (IsInside(i, l.y - 1, tilesToCheck))
                    stack.Push(new Vector2Int(i, l.y - 1));
        }
        if (tilesSum == tilesCheckedSum)
            return true;
        return false;
    }

    /// <summary>
    /// Проверяет, находится ли плитка внутри комнаты.
    /// </summary>
    /// <param name="x">X координата плитки.</param>
    /// <param name="y">Y координата плитки.</param>
    /// <param name="tilesToCheck">Массив плиток для проверки.</param>
    /// <returns>True - плитка находится внутри комнаты, иначе - False.</returns>
    private bool IsInside(int x, int y, int[,] tilesToCheck)
    {
        if (x >= 0 && y >= 0 && x < tilesToCheck.GetLength(1) && y < tilesToCheck.GetLength(0))
            return tilesToCheck[y, x] == 1;
        else
            return false;
    }

    /// <summary>
    /// Проверяет соединение с другой комнатой.
    /// </summary>
    /// <param name="other">Другая комната.</param>
    /// <returns>True - комнаты соединены, иначе - False.</returns>
    public bool CheckConnection(Room other)
    {
        if (this == null || other == null)
            return false;
        Vector2Int RoomPosNew = Vector2Int.Min(massRoomPos, other.massRoomPos);
        int maxX = Mathf.Max(massRoomPos.x + sizeX, other.massRoomPos.x + other.sizeX);
        int maxY = Mathf.Max(massRoomPos.y + sizeY, other.massRoomPos.y + other.sizeY);

        int sizeNewX = maxX - RoomPosNew.x + 1;
        int sizeNewY = maxY - RoomPosNew.y + 1;

        int tilesSum = 0;
        Vector2Int start = new Vector2Int();


        int[,] tilesToCheck = new int[sizeNewY, sizeNewX];

        for (int y = other.massRoomPos.y; y < other.massRoomPos.y + other.sizeY; y++)
            for (int x = other.massRoomPos.x; x < other.massRoomPos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.massRoomPos.y, x - other.massRoomPos.x] != 0)
                {
                    tilesToCheck[y - RoomPosNew.y, x - RoomPosNew.x] = 1;
                    tilesSum++;
                    start.y = y - RoomPosNew.y;
                    start.x = x - RoomPosNew.x;
                }

            }
        for (int y = massRoomPos.y; y < massRoomPos.y + sizeY; y++)
            for (int x = massRoomPos.x; x < massRoomPos.x + sizeX; x++)
            {
                if (tiles[y - massRoomPos.y, x - massRoomPos.x] != 0 && tilesToCheck[y - RoomPosNew.y, x - RoomPosNew.x] != 1)
                {
                    tilesToCheck[y - RoomPosNew.y, x - RoomPosNew.x] = 1;
                    tilesSum++;
                    start.y = y - RoomPosNew.y;
                    start.x = x - RoomPosNew.x;
                }
            }

        if (start.x < 0 || start.y < 0 || tilesSum < tilesMin)
            return false;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        stack.Push(start);
        int tilesCheckedSum = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;

            while (IsInside(lx, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, lx] = 2;
                tilesCheckedSum++;
                lx--;
            }
            int rx = l.x + 1;

            while (IsInside(rx, l.y, tilesToCheck))
            {
                tilesToCheck[l.y, rx] = 2;
                tilesCheckedSum++;
                rx++;
            }

            for (int i = lx; i < rx - 1; i++)
                if (IsInside(i, l.y + 1, tilesToCheck))
                    stack.Push(new Vector2Int(i, l.y + 1));

            for (int i = lx; i < rx - 1; i++)
                if (IsInside(i, l.y - 1, tilesToCheck))
                    stack.Push(new Vector2Int(i, l.y - 1));
        }

        if (tilesSum == tilesCheckedSum)
            return true;
        return false;
    }
}