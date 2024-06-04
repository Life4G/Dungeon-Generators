using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Room : ScriptableObject
{
    [SerializeField]
    public Vector2Int pos;
    [SerializeField]
    private int sizeX, sizeY;
    [SerializeField]
    private int[,] tiles;
    [SerializeField]
    private bool valid;
    private int tilesMin = 8;
    private int tilesMax = 16;

    //public Room(Vector2Int massRoomPos, int sizeX, int sizeY, int[,] tiles)
    //{
    //    this.massRoomPos = massRoomPos;
    //    this.sizeX = sizeX;
    //    this.sizeY = sizeY;
    //    this.tiles = tiles;
    //    this.valid = false;
    //}
    //public Room(Room room)
    //{
    //    massRoomPos = new Vector2Int(massRoomPos.x, massRoomPos.y);
    //    sizeX = room.sizeX;
    //    sizeY = room.sizeY;
    //    tiles = room.tiles.Clone() as int[,];
    //    valid = false;
    //}

    public static Room Copy(Room room)
    {
        Room copy = CreateInstance<Room>();
        copy.InitCopy(room);
        return copy;
    }
    public static Room CreateRandomRoom(Vector2Int pos)
    {
        Room room = CreateInstance<Room>();
        switch (Random.Range(0, 4))
        {
            case 0:
                room.InitSquareRoom(pos);
                break;
            case 1:
                room.InitCircleRoom(pos);
                break;
            case 2:
                room.InitRombusRoom(pos);
                break;
            case 3:
                room.InitPolygonRoom(pos);
                break;
                //case 4:
                //    room.InitCornerRoom(pos);
                //    break;
                //case 5:
                //    room.InitTriangleRoom(pos);
                //    break;

        }


        //Vector2Int pos = room.GetPos();
        //Size size = room.GetSize();
        //if (pos.x + size.Width > mapMaxWidth)
        //    mapMaxWidth += pos.x + size.Width;
        //if (pos.y + size.Height > mapMaxHeight)
        //    mapMaxHeight += pos.y + size.Height;
        return room;
    }
    private void InitSquareRoom(Vector2Int pos)
    {
        this.pos = pos;
        sizeX = Random.Range(6, tilesMax + 1);
        sizeY = Random.Range(6, tilesMax + 1);
        valid = false;
        tiles = new int[sizeY, sizeX];

        for (int y = 0; y < sizeY; y++)
            for (int x = 0; x < sizeX; x++)
            {
                tiles[y, x] = 1;
            }
    }
    private void InitPolygonRoom(Vector2Int pos)
    {
        this.pos = pos;
        int pointsSpawnRadius = 16;
        sizeX = -1; 
        sizeY = -1;
        valid = false;
        int pointsSpawnNum = 32;
        List<Vector2Int> points = GenerateSetOfPoints(pointsSpawnRadius, pointsSpawnNum);
        for (int i = 0; i < points.Count; i++)
        {

            if (sizeX < points[i].x)
                sizeX = points[i].x;
            if (sizeY < points[i].y)
                sizeY = points[i].y;
        }
        tiles = new int[++sizeY, ++sizeX];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = points.Aggregate(Vector2Int.zero, (current, point) => current + point) / points.Count;
        stack.Push(start);
        int counter = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;

            while (IsInsidePolygon(lx, l.y, points) && tiles[l.y, lx] == 0)
            {
                tiles[l.y, lx] = 1;
                lx--;
                counter++;
            }
            int rx = l.x + 1;

            while (IsInsidePolygon(rx, l.y, points) && tiles[l.y, rx] == 0)
            {
                tiles[l.y, rx] = 1;
                rx++;
                counter++;
            }

            for (int i = lx; i < rx - 1; i++)
                if (IsInsidePolygon(i, l.y + 1, points) && tiles[l.y + 1, i] == 0)
                {
                    stack.Push(new Vector2Int(i, l.y + 1));
                    counter++;
                }
            for (int i = lx; i < rx - 1; i++)
                if (IsInsidePolygon(i, l.y - 1, points) && tiles[l.y - 1, i] == 0)
                {
                    stack.Push(new Vector2Int(i, l.y - 1));
                    counter++;
                }
        }
       
    }
    private void InitCircleRoom(Vector2Int pos)
    {
        this.pos = pos;
        int roomRadius = Random.Range(4, tilesMax / 2 + 1);
        sizeX = roomRadius * 2;
        sizeY = sizeX;
        valid = false;
        tiles = new int[sizeY, sizeX];

        int x = 0;
        int y = roomRadius;
        int d = 1 - roomRadius;
        int incrE = 3;
        int incrSE = 5 - 2 * roomRadius;
        int cx = sizeX / 2;
        int cy = sizeY / 2;


        while (x <= y)
        {
            if (d > 0)
            {
                y--;
                d += incrSE;
                incrSE += 4;
            }
            else
            {
                d += incrE;
                incrSE += 2;
            }

            incrE += 2;
            x++;
            for (int i = cy - y; i < cy + y; i++)
                for (int j = cx - x; j < cx + x; j++)
                {
                    tiles[i, j] = 1;
                }
            for (int i = cy - x; i < cy + x; i++)
                for (int j = cx - y; j < cx + y; j++)
                {
                    tiles[i, j] = 1;
                }

        }
    }
    private void InitRombusRoom(Vector2Int pos)
    {
        this.pos = pos;
        int roomRadius = Random.Range(6, tilesMax / 2 + 1);
        sizeX = roomRadius * 2;
        sizeY = sizeX;
        valid = false;
        tiles = new int[sizeY, sizeX];

        for (int i = 0; i < roomRadius; i++)
            for (int j = 0; j < roomRadius; j++)
            {
                if (j >= roomRadius - i && j <= roomRadius + i)
                {
                    tiles[i, j] = 1;
                    tiles[i, sizeX - j - 1] = 1;
                    tiles[sizeY - j - 1, i] = 1;
                    tiles[sizeY - j - 1, sizeX - i - 1] = 1;
                }
            }
    }
    private void InitCornerRoom(Vector2Int pos)
    {
        this.pos = pos;
        int roomRadius = Random.Range(6, tilesMax + 1);
        sizeX = roomRadius; 
        sizeY = sizeX;
        valid = false;
        tiles = new int[sizeY, sizeX];

        switch (Random.Range(0, 4))
        {
            case 0:
                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[i, j] = 1;
                        }
                    }
                break;
            case 1:
                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[i, sizeX - j - 1] = 1;
                        }
                    }
                break;
            case 2:
                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[sizeX - j - 1, i] = 1;
                        }
                    }
                break;
            case 3:
                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[sizeX - j - 1, sizeX - i - 1] = 1;
                        }
                    }
                break;
        }
    }
    private void InitTriangleRoom(Vector2Int pos)
    {
        this.pos = pos;
        int roomRadius = Random.Range(6, tilesMax / 2 + 1);
        sizeX = roomRadius * 2;
        sizeY = roomRadius * 2;
        valid = false;
        tiles = new int[sizeY, sizeX];

        switch (Random.Range(0, 4))
        {
            case 0:

                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[i, j] = 1;
                            tiles[i, sizeX - j - 1] = 1;
                        }
                    }
                break;
            case 1:

                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[i, sizeX - j - 1] = 1;
                            tiles[sizeY - j - 1, sizeX - i - 1] = 1;
                        }
                    }
                break;
            case 2:

                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[sizeX - j - 1, sizeX - i - 1] = 1;
                            tiles[sizeY - j - 1, i] = 1;
                        }
                    }
                break;
            case 3:

                for (int i = 0; i < roomRadius; i++)
                    for (int j = 0; j < roomRadius; j++)
                    {
                        if (j >= roomRadius - i && j <= roomRadius + i)
                        {
                            tiles[sizeY - j - 1, i] = 1;
                            tiles[i, j] = 1;
                        }
                    }
                break;
        }
    }
    private void InitCopy(Room room) 
    {
        pos = new Vector2Int(room.pos.x, room.pos.y);
        sizeX = room.sizeX;
        sizeY = room.sizeY;
        tiles = room.tiles.Clone() as int[,];
        valid = false;
    }
    private bool IsInsidePolygon(int x, int y, List<Vector2Int> points)
    {
        bool res = false;
        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
            if (((points[i].y > y) != (points[j].y > y)) && (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                res = !res;
        return res;
    }
    private List<Vector2Int> GenerateSetOfPoints(int pointsSpawnRadius, int pointsSpawnNum)
    {
        List<Vector2Int> points = new List<Vector2Int>();
        int cx = pointsSpawnRadius;
        int cy = pointsSpawnRadius;

        for (int i = 0; i < 4; i++)
        {
            float a = Random.Range(3.1415f * i, 3.1415f / 2 * i);
            for (int j = 0; j < pointsSpawnNum / 4; j++)
            {

                float r = Random.Range(4, pointsSpawnRadius);

                points.Add(new Vector2Int(Mathf.RoundToInt(cx + r * Mathf.Cos(a)), Mathf.RoundToInt(cy + r * Mathf.Sin(a))));
            }
        }
        for (int i = 0; i < points.Count - 1; i++)
        {
            int min = i;

            for (int j = i + 1; j < points.Count; j++)
            {
                if (points[j].x < points[min].x || (points[j].x == points[min].x && points[j].y < points[min].y))
                {
                    min = j;
                }
            }

            var temp = points[min];
            points[min] = points[i];
            points[i] = temp;
        }

        List<Vector2Int> polygon = new List<Vector2Int>(new Vector2Int[pointsSpawnNum * 2]);
        int k = 0;
        for (int i = 0; i < pointsSpawnNum; ++i)
        {
            while (k >= 2 && (polygon[k - 1].x - polygon[k - 2].x) * (points[i].y - polygon[k - 2].y)
                - (polygon[k - 1].y - polygon[k - 2].y) * (points[i].x - polygon[k - 2].x) <= 0)
                k--;
            polygon[k++] = points[i];
        }

        // Build upper hull
        for (int i = pointsSpawnNum - 2, t = k + 1; i >= 0; --i)
        {
            while (k >= t && (polygon[k - 1].x - polygon[k - 2].x) * (points[i].y - polygon[k - 2].y)
                - (polygon[k - 1].y - polygon[k - 2].y) * (points[i].x - polygon[k - 2].x) <= 0)
                k--;
            polygon[k++] = points[i];
        }
        return polygon.Take(k - 1).ToList();
    }

    public Vector2Int GetPos()
    {
        return pos;
    }
    public Vector2Int GetPosCenter()
    {
        return new Vector2Int(pos.x + sizeX / 2, pos.y + sizeY / 2); ;
    }
    public Size GetSize()
    {
        return new Size(sizeX, sizeY);
    }
    public void SetPos(int x, int y)
    {
        pos = new Vector2Int(x, y);
    }
    public void SetPos(Vector2Int pos)
    {
        this.pos = pos;
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

        int collisonX1 = Mathf.Max(pos.x, other.pos.x);
        int collisonY1 = Mathf.Max(pos.y, other.pos.y);

        int collisonX2 = Mathf.Min(pos.x + sizeX, other.pos.x + other.sizeX);
        int collisonY2 = Mathf.Min(pos.y + sizeY, other.pos.y + other.sizeY);

        if (collisonX1 >= collisonX2 || collisonY1 >= collisonY2)
            return false;

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - pos.y, x - pos.x] * other.tiles[y - other.pos.y, x - other.pos.x] == 1)
                    return true;
            }

        return false;
    }
    public bool IsProperSubsetOf(Room other)
    {
        int collisonX1 = Mathf.Max(pos.x, other.pos.x);
        int collisonY1 = Mathf.Max(pos.y, other.pos.y);
        int collisonX2 = Mathf.Min(pos.x + sizeX, other.pos.x + other.sizeX);
        int collisonY2 = Mathf.Min(pos.y + sizeY, other.pos.y + other.sizeY);

        if (collisonX1 > collisonX2 || collisonY1 > collisonY2)
            return false;
        return true;
    }
    //Операция пересечения с другой комнатой
    public void Intersect(Room other)
    {
        int collisonX1 = Mathf.Max(pos.x, other.pos.x);
        int collisonY1 = Mathf.Max(pos.y, other.pos.y);
        int collisonX2 = Mathf.Min(pos.x + sizeX, other.pos.x + other.sizeX);
        int collisonY2 = Mathf.Min(pos.y + sizeY, other.pos.y + other.sizeY);

        int sizeXNew = collisonX2 - collisonX1 + 1;
        int sizeYNew = collisonY2 - collisonY1 + 1;
        int[,] tilesNew = new int[sizeYNew, sizeXNew];

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - pos.y, x - pos.x] * other.tiles[y - other.pos.y, x - other.pos.x] == 1)
                    tilesNew[y - pos.y, x - pos.x] = 1;
                else
                    tilesNew[y - pos.y, x - pos.x] = 0;
            }
        tiles = tilesNew;
        pos = new Vector2Int(collisonX1, collisonY1);
        sizeX = sizeXNew;
        sizeY = sizeYNew;
    }
    //Операция объединения с другой комнатой
    public void Union(Room other)
    {
        Vector2Int roomPosNew = Vector2Int.Min(pos, other.pos);
        int maxX = Mathf.Max(pos.x + sizeX, other.pos.x + other.sizeX);
        int maxY = Mathf.Max(pos.y + sizeY, other.pos.y + other.sizeY);

        int sizeNewX = maxX - roomPosNew.x + 1;
        int sizeNewY = maxY - roomPosNew.y + 1;


        int[,] tilesNew = new int[sizeNewY, sizeNewX];

        for (int y = other.pos.y; y < other.pos.y + other.sizeY; y++)
            for (int x = other.pos.x; x < other.pos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.pos.y, x - other.pos.x] == 1)
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 1;
                else
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 0;

            }
        for (int y = pos.y; y < pos.y + sizeY; y++)
            for (int x = pos.x; x < pos.x + sizeX; x++)
            {
                if (tiles[y - pos.y, x - pos.x] == 1)
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 1;
                else
                    tilesNew[y - roomPosNew.y, x - roomPosNew.x] = 0;

            }

        tiles = tilesNew;
        sizeX = sizeNewX;
        sizeY = sizeNewY;
        pos = roomPosNew;
    }
    //Операция разности с другой комнатой
    public void Difference(Room other)
    {
        int collisonX1 = Mathf.Max(pos.x, other.pos.x);
        int collisonY1 = Mathf.Max(pos.y, other.pos.y);
        int collisonX2 = Mathf.Min(pos.x + sizeX, other.pos.x + other.sizeX);
        int collisonY2 = Mathf.Min(pos.y + sizeY, other.pos.y + other.sizeY);

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - pos.y, x - pos.x] * other.tiles[y - other.pos.y, x - other.pos.x] == 1)
                    tiles[y - pos.y, x - pos.x] = 0;
            }

    }
    //Операция симметричной разности с другой комнатой
    public void SymmetricDifference(Room other)
    {
        int collisonX1 = Mathf.Max(pos.x, other.pos.x);
        int collisonY1 = Mathf.Max(pos.y, other.pos.y);
        int collisonX2 = Mathf.Min(pos.x + sizeX, other.pos.x + other.sizeX);
        int collisonY2 = Mathf.Min(pos.y + sizeY, other.pos.y + other.sizeY);

        for (int y = collisonY1; y < collisonY2; y++)
            for (int x = collisonX1; x < collisonX2; x++)
            {
                if (tiles[y - pos.y, x - pos.x] * other.tiles[y - other.pos.y, x - other.pos.x] == 1)
                {
                    tiles[y - pos.y, x - pos.x] = 0;
                    other.tiles[y - other.pos.y, x - other.pos.x] = 0;
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
    private bool IsInside(int x, int y, int[,] tilesToCheck)
    {
        if (x >= 0 && y >= 0 && x < tilesToCheck.GetLength(1) && y < tilesToCheck.GetLength(0))
            return tilesToCheck[y, x] == 1;
        else
            return false;
    }
    public bool CheckConnection(Room other)
    {
        if (this == null || other == null)
            return false;
        Vector2Int RoomPosNew = Vector2Int.Min(pos, other.pos);
        int maxX = Mathf.Max(pos.x + sizeX, other.pos.x + other.sizeX);
        int maxY = Mathf.Max(pos.y + sizeY, other.pos.y + other.sizeY);

        int sizeNewX = maxX - RoomPosNew.x + 1;
        int sizeNewY = maxY - RoomPosNew.y + 1;

        int tilesSum = 0;
        Vector2Int start = new Vector2Int();


        int[,] tilesToCheck = new int[sizeNewY, sizeNewX];

        for (int y = other.pos.y; y < other.pos.y + other.sizeY; y++)
            for (int x = other.pos.x; x < other.pos.x + other.sizeX; x++)
            {
                if (other.tiles[y - other.pos.y, x - other.pos.x] != 0)
                {
                    tilesToCheck[y - RoomPosNew.y, x - RoomPosNew.x] = 1;
                    tilesSum++;
                    start.y = y - RoomPosNew.y;
                    start.x = x - RoomPosNew.x;
                }

            }
        for (int y = pos.y; y < pos.y + sizeY; y++)
            for (int x = pos.x; x < pos.x + sizeX; x++)
            {
                if (tiles[y - pos.y, x - pos.x] != 0 && tilesToCheck[y - RoomPosNew.y, x - RoomPosNew.x] != 1)
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