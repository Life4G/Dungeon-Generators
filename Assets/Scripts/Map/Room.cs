using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static SetOperations;

public static class SetOperations
{
    public enum Operations
    {
        Intersect,
        Union,
        Difference,
        SymmetricDifference,
        None
    }
    public static Operations GetRandomOperation()
    {
        int random = Random.Range(0, 4);
        Operations operation = Operations.None;
        switch (random)
        {
            case 0:
                operation = Operations.Intersect;
                break;
            case 1:
                operation = Operations.Union;
                break;
            case 2:
                operation = Operations.Difference;
                break;
            case 3:
                operation = Operations.SymmetricDifference;
                break;
        }
        return operation;
    }
    public static Operations GetRandomSubOperation()
    {
        int random = Random.Range(0, 2);
        Operations operation = Operations.None;
        switch (random)
        {
            case 0:
                operation = Operations.Union;
                break;
            case 1:
                operation = Operations.Difference;
                break;
        }
        return operation;
    }
    public static List<Operations> GetOperationsList = new List<Operations>
    {
        Operations.Intersect,
        Operations.Union,
        Operations.Difference,
        Operations.SymmetricDifference

    };
    public static List<Operations> GetSubOperationsList = new List<Operations>
    {
        Operations.Union,
        Operations.Difference,
    };

}

//Возможные стили
public enum Styles
{
    None, Style1,
}

public class Room
{
    //Позиции центра комнаты (в прямом смысле центра; правда если условно размер 2x2 то центр там будет смещен :D)
    protected Vector2Int positionCenter;
    //Позиции тайлов
    protected HashSet<Vector2Int> positionRoomTiles;
    protected Styles style;
    int x_min, x_max;
    int y_min, y_max;

    //Функции с получением изменением позиции
    public Vector2Int GetPos()
    {
        return positionCenter;
    }
    public void SetPos(int x, int y)
    {
        positionCenter = new Vector2Int(x, y);
    }
    public void SetPos(Vector2Int pos)
    {
        positionCenter = pos;
    }
    //Функции с получением изменением стиля
    public Styles GetStyle()
    {
        return style;
    }
    public void SetStyle(Styles style)
    {
        this.style = style;
    }
    //Функции с получением изменением позиции тайлов
    public HashSet<Vector2Int> GetTilesPos()
    {
        return positionRoomTiles;
    }
    public void SetTilesPos(HashSet<Vector2Int> positions)
    {
        positionRoomTiles = positions;
    }
    //Функция проверки пересечения
    public bool CheckIntersection(Room other)
    {
        return positionRoomTiles.Overlaps(other.GetTilesPos());
    }
    //Функции проверки что эта комната входит в другую
    public bool IsSubsetOf(Room other)
    {
        return positionRoomTiles.IsSubsetOf(other.GetTilesPos());
    }
    public bool IsSupersetOf(Room other)
    {
        return positionRoomTiles.IsSupersetOf(other.GetTilesPos());
    }
    //Функции проверки что в эту комнату входит другая
    public bool IsProperSubsetOf(Room other)
    {
        return positionRoomTiles.IsProperSubsetOf(other.GetTilesPos());
    }
    public bool IsProperSupersetOf(Room other)
    {
        return positionRoomTiles.IsProperSupersetOf(other.GetTilesPos());
    }
    //Операция пересечения с другой комнатой
    public void Intersect(Room other)
    {
        positionRoomTiles.IntersectWith(other.GetTilesPos());
    }
    //Операция объединения с другой комнатой
    public void Union(Room other)
    {
        positionRoomTiles.UnionWith(other.GetTilesPos());
    }
    //Операция разности с другой комнатой
    public void Difference(Room other)
    {
        positionRoomTiles.ExceptWith(other.GetTilesPos());
    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(Room other)
    {
        positionRoomTiles.SymmetricExceptWith(other.GetTilesPos());
    }
    public bool CheckConnection(Room other)
    {
        return Validate(other.GetTilesPos(), other.x_min, other.x_max, other.y_min, other.y_max);
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
            case SetOperations.Operations.Difference:
                Difference(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                SymmetricDifference(other);
                break;
        }
    }
    public bool TryOperation(Room other, SetOperations.Operations operation)
    {
        Room roomTest = this;
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                roomTest.Intersect(other);
                break;
            case SetOperations.Operations.Union:
                roomTest.Union(other);
                break;
            case SetOperations.Operations.Difference:
                roomTest.Difference(other);
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
    public bool Validate()
    {
        if (positionRoomTiles.Count < 16)
            return false;

        List<Vector2Int> OpenStates = new List<Vector2Int>();
        List<Vector2Int> ClosedStates = new List<Vector2Int>();
        Vector2Int vecTemp = positionRoomTiles.First();
        OpenStates.Add(vecTemp);
        int x_min = vecTemp.x, x_max = vecTemp.x;
        int y_min = vecTemp.y, y_max = vecTemp.y;

        while (OpenStates.Count > 0)
        {
            Vector2Int currentPos = OpenStates.First();
            OpenStates.Remove(OpenStates.First());
            ClosedStates.Add(currentPos);
            List<Vector2Int> newPoses = Direction2D.GetNewCardinalPosesFromPos(currentPos);

            foreach (var pos in newPoses)
            {
                if (!(OpenStates.Contains(pos) || ClosedStates.Contains(pos)) && positionRoomTiles.Contains(pos))
                {
                    if (x_min > pos.x)
                        x_min = pos.x;
                    else if (x_max < pos.x)
                        x_max = pos.x;

                    if (y_min > pos.y)
                        y_min = pos.y;
                    else if (y_max < pos.y)
                        y_max = pos.y;

                    OpenStates.Add(pos);
                }
            }
        }
        if (ClosedStates.Count == positionRoomTiles.Count)
        {
            this.x_min = x_min; this.x_max = x_max;
            this.y_min = y_min; this.y_max = y_max;
            return true;
        }
        return false;
    }
    public bool Validate(HashSet<Vector2Int> tilesPos, int roomX_min, int roomX_max, int roomY_min, int roomY_max)
    {
        int d1x = roomX_min - x_max;
        int d1y = roomY_min - y_max;
        int d2x = x_min - roomX_max;
        int d2y = y_min - roomY_max;

        if (d1x > 0 || d1y > 0)
            return false;

        if (d2x > 0 || d2y > 0)
            return false;

        HashSet<Vector2Int> tilesPosTest = new HashSet<Vector2Int>(this.GetTilesPos());
        tilesPosTest.UnionWith(tilesPos);
        if (tilesPosTest.Count < 16)
            return false;

        List<Vector2Int> OpenStates = new List<Vector2Int>();
        List<Vector2Int> ClosedStates = new List<Vector2Int>();

        OpenStates.Add(tilesPosTest.First());
        while (OpenStates.Count > 0)
        {
            Vector2Int currentPos = OpenStates.First();
            OpenStates.Remove(OpenStates.First());
            ClosedStates.Add(currentPos);
            List<Vector2Int> newPoses = Direction2D.GetNewCardinalPosesFromPos(currentPos);

            foreach (var pos in newPoses)
            {
                if (!(OpenStates.Contains(pos) || ClosedStates.Contains(pos)) && tilesPosTest.Contains(pos))
                {
                    OpenStates.Add(pos);
                }
            }
        }
        if (ClosedStates.Count == tilesPosTest.Count)
            return true;

        return false;
    }
}

public class MassRoom
{
    public Vector2Int massRoomPos;
    private int sizeX, sizeY;
    private int[,] tiles;
    private Styles style;

    //Функции с получением изменением позиции
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
    //Функции с получением изменением стиля
    public Styles GetStyle()
    {
        return style;
    }
    public void SetStyle(Styles style)
    {
        this.style = style;
    }
    //Функции с получением изменением позиции тайлов
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
    public bool CheckIntersection(MassRoom other)
    {
        bool collisionX = massRoomPos.x + sizeX >= other.massRoomPos.x &&
            other.massRoomPos.x + other.sizeX >= massRoomPos.x;

        bool collisionY = massRoomPos.y + sizeY >= other.massRoomPos.y &&
                   other.massRoomPos.y + other.sizeY >= massRoomPos.y;

        if (collisionX && collisionY)
        {
            Vector2Int offset = massRoomPos - other.massRoomPos;
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {
                    if (y + offset.y >= 0 && y + offset.y < other.sizeY
                        && x + offset.x >= 0 && x + offset.x < other.sizeX
                        && tiles[y, x] == other.tiles[y + offset.y, x + offset.x])
                        return true;
                }
        }
        return false;
    }
    public bool IsProperSubsetOf(MassRoom other)
    {
        bool collisionX = massRoomPos.x >= other.massRoomPos.x &&
                   massRoomPos.x + sizeX <= other.massRoomPos.x + other.sizeX;

        bool collisionY = massRoomPos.y >= other.massRoomPos.y &&
                   massRoomPos.y + sizeY <= other.massRoomPos.y + other.sizeY;
        if (collisionX && collisionY)
        {
            return true;
        }
        return false;
    }
    //Операция пересечения с другой комнатой
    public void Intersect(MassRoom other)
    {
        int[,] tilesNew = new int[sizeY, sizeX];
        Vector2Int offset = massRoomPos - other.massRoomPos;

        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                if (y + offset.y >= 0 && y + offset.y < other.sizeY
                    && x + offset.x >= 0 && x + offset.x < other.sizeX
                    && tiles[y, x] == other.tiles[y + offset.y, x + offset.x])
                    tilesNew[y, x] = tiles[y, x];
                else
                    tilesNew[y, x] = 0;
            }

        tiles = tilesNew;
    }
    //Операция объединения с другой комнатой
    public void Union(MassRoom other)
    {
        int minX, minY, maxX, maxY;
        if (massRoomPos.x > other.massRoomPos.x)
        {
            minX = other.massRoomPos.x;
        }
        else
        {
            minX = massRoomPos.x;
        }
        if (massRoomPos.y > other.massRoomPos.y)
        {
            minY = other.massRoomPos.y;
        }
        else
        {
            minY = massRoomPos.y;
        }
        if (massRoomPos.x + sizeX > other.massRoomPos.x + other.sizeX)
        {
            maxX = massRoomPos.x + sizeX;
        }
        else
        {
            maxX = other.massRoomPos.x + other.sizeX;
        }
        if (massRoomPos.y + sizeY > other.massRoomPos.y + other.sizeY)
        {
            maxY = massRoomPos.y + sizeY;
        }
        else
        {
            maxY = other.massRoomPos.y + other.sizeY;
        }

        int[,] tilesNew = new int[maxY - minY, maxX - minX];
        Vector2Int offset = new Vector2Int(minY, minX) - massRoomPos;
        Vector2Int offsetOther = new Vector2Int(minY, minX) - other.massRoomPos;

        for (int y = 0; y < other.sizeY; y++)
            for (int x = 0; x < other.sizeX; x++)
            {
                if (y + offsetOther.y >= 0 && y + offsetOther.y < maxY - minY
                    && x + offsetOther.x >= 0 && x + offsetOther.x < maxX - minX)
                    tilesNew[y + offsetOther.y, x + offsetOther.x] = other.tiles[y, x];

            }
        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                if (y + offsetOther.y >= 0 && y + offsetOther.y < maxY - minY
                    && x + offsetOther.x >= 0 && x + offsetOther.x < maxX - minX)
                    tilesNew[y + offsetOther.y, x + offsetOther.x] = tiles[y, x];
            }

        tiles = tilesNew;
        sizeX = maxX - minX;
        sizeY = maxY - minY;
        massRoomPos = new Vector2Int(minX, minY);
    }
    //Операция разности с другой комнатой
    public void Difference(MassRoom other)
    {
        int[,] tilesNew = new int[sizeY, sizeX];

        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                Vector2Int offset = massRoomPos - other.massRoomPos;
                if (y + offset.y >= 0 && y + offset.y < other.sizeY
                    && x + offset.x >= 0 && x + offset.x < other.sizeX
                    && tiles[y, x] != other.tiles[y + offset.y, x + offset.x])
                    tilesNew[y, x] = tiles[y, x];
                else
                    tilesNew[y, x] = 0;
            }

        tiles = tilesNew;
    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(MassRoom other)
    {
        int minX, minY, maxX, maxY;
        if (massRoomPos.x > other.massRoomPos.x)
        {
            minX = other.massRoomPos.x;
        }
        else
        {
            minX = massRoomPos.x;
        }
        if (massRoomPos.y > other.massRoomPos.y)
        {
            minY = other.massRoomPos.y;
        }
        else
        {
            minY = massRoomPos.y;
        }
        if (massRoomPos.x + sizeX > other.massRoomPos.x + other.sizeX)
        {
            maxX = massRoomPos.x + sizeX;
        }
        else
        {
            maxX = other.massRoomPos.x + other.sizeX;
        }
        if (massRoomPos.y + sizeY > other.massRoomPos.y + other.sizeY)
        {
            maxY = massRoomPos.y + sizeY;
        }
        else
        {
            maxY = other.massRoomPos.y + other.sizeY;
        }

        int[,] tilesNew = new int[maxY - minY, maxX - minX];
        Vector2Int offset = new Vector2Int(minY, minX) - massRoomPos;
        Vector2Int offsetOther = new Vector2Int(minY, minX) - other.massRoomPos;

        for (int y = 0; y < other.sizeY; y++)
            for (int x = 0; x < other.sizeX; x++)
            {
                if (y + offsetOther.y >= 0 && y + offsetOther.y < maxY - minY
                    && x + offsetOther.x >= 0 && x + offsetOther.x < maxX - minX)
                    tilesNew[y + offsetOther.y, x + offsetOther.x] = other.tiles[y, x];
            }
        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                if (y + offsetOther.y >= 0 && y + offsetOther.y < maxY - minY
                    && x + offsetOther.x >= 0 && x + offsetOther.x < maxX - minX)
                    if (tilesNew[y + offsetOther.y, x + offsetOther.x] == 0)
                        tilesNew[y + offsetOther.y, x + offsetOther.x] = tiles[y, x];
                    else
                        tilesNew[y + offsetOther.y, x + offsetOther.x] = 0;
            }
        tiles = tilesNew;
        sizeX = maxX - minX;
        sizeY = maxY - minY;
        massRoomPos = new Vector2Int(minX, minY);
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

    public bool CheckConnection(MassRoom other)
    {
        int minX, minY, maxX, maxY;
        if (massRoomPos.x > other.massRoomPos.x)
        {
            minX = other.massRoomPos.x;
        }
        else
        {
            minX = massRoomPos.x;
        }
        if (massRoomPos.y > other.massRoomPos.y)
        {
            minY = other.massRoomPos.y;
        }
        else
        {
            minY = massRoomPos.y;
        }

        if (massRoomPos.x + sizeX > other.massRoomPos.x + other.sizeX)
        {
            maxX = massRoomPos.x + sizeX;
        }
        else
        {
            maxX = other.massRoomPos.x + other.sizeX;
        }
        if (massRoomPos.y + sizeY > other.massRoomPos.y + other.sizeY)
        {
            maxY = massRoomPos.y + sizeY;
        }
        else
        {
            maxY = other.massRoomPos.y + other.sizeY;
        }

        int[,] tilesToValidate = new int[maxY, maxX];
        bool run = true;
        int startY = 0, startX = 0;
        int step = 0;
        int tilesTotalNum = 0;

        for (int y = 0; y < other.sizeY; y++)
            for (int x = 0; x < other.sizeX; x++)
            {
                int offsetY = minY + y;
                int offsetX = minX + x;

                if (offsetY >= massRoomPos.y && offsetY < massRoomPos.y + sizeY
                    && offsetX >= massRoomPos.x && offsetX < massRoomPos.x + sizeX
                    && tiles[offsetY - massRoomPos.y, offsetX - massRoomPos.x] != 0)
                {
                    tilesTotalNum++;
                    tilesToValidate[y, x] = -1;
                }
                else
                    tilesToValidate[y, x] = -2;
            }
        for (int y = 0; y < other.sizeY; y++)
            for (int x = 0; x < other.sizeX; x++)
            {
                int offsetY = minY + y;
                int offsetX = minX + x;

                if (offsetY >= massRoomPos.y && offsetY < massRoomPos.y + sizeY
                    && offsetX >= massRoomPos.x && offsetX < massRoomPos.x + sizeX
                    && tiles[offsetY - massRoomPos.y, offsetX - massRoomPos.x] != 0)
                {
                    tilesTotalNum++;
                    tilesToValidate[y, x] = -1;
                }

                else
                {
                    tilesToValidate[y, x] = -2;
                    startY = y; startX = x;
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
        }
        if (tilesNum == tilesTotalNum)
            return true;
        return false;
    }

    public void DoOperation(MassRoom other, SetOperations.Operations operation)
    {
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                Intersect(other);
                break;
            case SetOperations.Operations.Union:
                Union(other);
                break;
            case SetOperations.Operations.Difference:
                Difference(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                SymmetricDifference(other);
                break;
        }
    }
    public bool TryOperation(MassRoom other, SetOperations.Operations operation)
    {
        MassRoom roomTest = this;
        switch (operation)
        {
            case SetOperations.Operations.Intersect:
                roomTest.Intersect(other);
                break;
            case SetOperations.Operations.Union:
                roomTest.Union(other);
                break;
            case SetOperations.Operations.Difference:
                roomTest.Difference(other);
                break;
            case SetOperations.Operations.SymmetricDifference:
                roomTest.SymmetricDifference(other);
                break;
        }
        return roomTest.Validate();
    }
    public SetOperations.Operations TryAllOperations(MassRoom other)
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
    public SetOperations.Operations TryAllSubOperations(MassRoom other)
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