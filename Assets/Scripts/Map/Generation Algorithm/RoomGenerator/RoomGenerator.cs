using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;
using static SetOperations;
using Unity.VisualScripting;
using System.Linq;

public class RoomGenerator : DungeonGeneratorBase
{
    [SerializeField]
    private int roomNumberMin = 4;
    [SerializeField]
    private int roomNumberMax = 6;
    [SerializeField]
    private int roomSizeMax = 16;
    [SerializeField]
    private bool checkConnection = true;

    public int mapMaxHeight = 512;
    public int mapMaxWidth = 512;
    private int roomSpawnRadius = 60;
    private int[,] map;
    private int spawnIncreaseRadius = 0;
    private bool careAboutRoomCount = false;

    protected override int[,] GenerateDungeon()
    {
        return Run();
    }
    protected int[,] Run()
    {
        map = new int[mapMaxWidth, mapMaxHeight];
        List<Room> rooms = new List<Room>();
        List<Room> roomList = new List<Room>();
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            roomList.Add(GenerateRandomRoom());
        }

        int roomNumberCur = roomNumber;
        bool roomsValidated;
        do
        {
            roomsValidated = true;
            int index = -1;
            Room room = null;
            for (int i = 0; i < roomList.Count && room == null; i++)
            {
                if (roomList[i] != null && !roomList[i].GetValidation())
                {
                    index = i;
                    room = roomList[i];
                    roomsValidated = false;
                }
            }
            for (int i = 0; i < roomList.Count && room != null; i++)
            {
                if (roomList[i] != null && roomList[i] != room)
                {
                    if (room.CheckIntersection(roomList[i]))
                    {
                        Operations operation = TryOperations(room, roomList[i], room.IsProperSubsetOf(roomList[i]));
                        switch (operation)
                        {
                            case Operations.Intersect:
                                room.SetValidation(false);
                                room.Intersect(roomList[i]);
                                roomList[i] = null;
                                roomList[index] = room;
                                roomNumberCur--;
                                break;

                            case Operations.Union:
                                room.SetValidation(false);
                                room.Union(roomList[i]);
                                roomList[i] = null;
                                roomList[index] = room;
                                roomNumberCur--;
                                break;

                            case Operations.DifferenceAB:
                                room.SetValidation(false);
                                room.Difference(roomList[i]);
                                roomList[i] = null;
                                roomList[index] = room;
                                roomNumberCur--;
                                break;

                            case Operations.DifferenceBA:
                                roomList[i].Difference(room);
                                roomList[i].SetValidation(false);
                                room = null;
                                roomList[index] = room;
                                roomNumberCur--;
                                break;

                            case Operations.SymmetricDifference:
                                room.SymmetricDifference(roomList[i]);
                                room.SetValidation(false);
                                roomList[i].SetValidation(false);
                                roomList[index] = room;
                                break;

                            case Operations.None:
                                room = null;
                                roomList[index] = room;
                                roomNumberCur--;
                                break;
                        }
                    }
                    else
                    {
                        if (checkConnection && room.CheckConnection(roomList[i]))
                        {
                            room.Union(roomList[i]);
                            roomList[i] = null;
                            roomList[index] = room;
                            room.SetValidation(room.Validate());
                        }
                        else
                        {
                            roomList[index] = room;
                            if (room.Validate())
                                room.SetValidation(true);
                            else
                            {
                                room = null;
                                roomList[index] = room;
                                roomNumberCur--;
                            }
                        }
                    }
                }
            }
            if (spawnIncreaseRadius > 3)
            {
                spawnIncreaseRadius = 0;
                roomSpawnRadius += roomSizeMax;
            }
            if (careAboutRoomCount && roomNumberCur < roomNumberMin)
            {
                for (int i = 0; i < roomList.Count && roomNumberCur < roomNumberMin; i++)
                {
                    if (roomList[i] == null)
                    {
                        roomList[i] = GenerateRandomRoom();
                        roomNumberCur++;
                    }
                }
                spawnIncreaseRadius++;
            }
        } while (!roomsValidated);

        for (int i = 0; i < roomList.Count; i++)
            if (roomList[i] != null)
                rooms.Add(roomList[i]);

        graph = new Graph(rooms, mapMaxWidth, mapMaxHeight);

        for (int i = 0; i < mapMaxHeight; i++)
            for (int j = 0; j < mapMaxWidth; j++)
            {
                map[i, j] = -1;
            }

        for (int i = 0; i < rooms.Count; i++)
        {
            int[,] roomTiles = rooms[i].GetTiles();
            Vector2Int roomPos = rooms[i].GetPos();
            Size roomSize = rooms[i].GetSize();

            for (int y = 0; y < roomSize.Height && y + roomPos.y < mapMaxHeight - 1; y++)
                for (int x = 0; x < roomSize.Width && x + roomPos.x < mapMaxWidth - 1; x++)
                {
                    if (y + roomPos.y > 0 && x + roomPos.x > 0 && roomTiles[y, x] != 0)
                        map[y + roomPos.y, x + roomPos.x] = i;
                }
        }
        DrawCorridors(graph.GetCorridors(), rooms.Count);
        return map;
    }
    private int ipart(double x) { return (int)x; }
    private int round(double x) { return ipart(x + 0.5); }
    private void DrawCorridors(List<GraphEdge> corridors, int offset)
    {
        for (int index = 0; index < corridors.Count; index++)
        {
            double x1 = corridors[index].posPoint1.x, x2 = corridors[index].posPoint2.x, y1 = corridors[index].posPoint1.y, y2 = corridors[index].posPoint2.y;
            bool steer = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            double temp;

            if (steer)
            {
                temp = x1; x1 = y1; y1 = temp;
                temp = x2; x2 = y2; y2 = temp;
            }
            if (x1 > x2)
            {

                temp = x1; x1 = x2; x2 = temp;
                temp = y1; y1 = y2; y2 = temp;
            }
            double dx = x2 - x1;
            double dy = y2 - y1;
            double gradient;
            if (dx == 0)
                gradient = 1;
            else
                gradient = dy / dx;

            double xEnd = round(x1);
            double yEnd = y1 + gradient * (xEnd - x1);
            double xPixel1 = xEnd;
            double yPixel1 = ipart(yEnd);
            if (steer)
            {
                map[(int)xPixel1, (int)yPixel1] = index + offset;
                map[(int)xPixel1, (int)yPixel1 + 1] = index + offset;
            }
            else
            {
                map[(int)yPixel1, (int)xPixel1] = index + offset;
                map[(int)yPixel1 + 1, (int)xPixel1] = index + offset;
            }
            double intery = yEnd + gradient;

            xEnd = round(x2);
            yEnd = y2 + gradient * (xEnd - x2);
            double xPixel2 = xEnd;
            double yPixel2 = ipart(yEnd);
            if (steer)
            {
                map[(int)xPixel2, (int)yPixel2] = index + offset;
                map[(int)xPixel2, (int)yPixel2 + 1] = index + offset;
            }
            else
            {
                map[(int)yPixel2, (int)xPixel2] = index + offset;
                map[(int)yPixel2 + 1, (int)xPixel2] = index + offset;
            }

            if (steer)
            {
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    map[x, ipart(intery)] = index + offset;
                    map[x, ipart(intery) + 1] = index + offset;
                    intery += gradient;
                }
            }
            else
            {
                for (int x = (int)(xPixel1 + 1); x <= xPixel2 - 1; x++)
                {
                    map[ipart(intery), x] = index + offset;
                    map[ipart(intery) + 1, x] = index + offset;
                    intery += gradient;
                }
            }
        }
    }
    private Room GenerateRandomRoom()
    {
        Room room = null;
        switch (Random.Range(0, 4))
        {
            case 0:
                room = GenerateSquareRoom();
                break;
            case 1:
                room = GenerateCircleRoom();
                break;
            case 2:
                room = GenerateRombusRoom();
                break;
            case 3:
                room = GeneratePolygonRoom();
                break;
                //case 4:
                //    room = GenerateCornerRoom();
                //    break;
                //case 5:
                //    room = GenerateTriangleRoom();
                //    break;

        }
        Vector2Int pos = room.GetPos();
        Size size = room.GetSize();

        if (pos.x + size.Width > mapMaxWidth)
            mapMaxWidth += pos.x + size.Width;

        if (pos.y + size.Height > mapMaxHeight)
            mapMaxHeight += pos.y + size.Height;
        return room;
    }
    private Room GenerateSquareRoom()
    {
        int roomWidth = Random.Range(6, roomSizeMax + 1);
        int roomHeight = Random.Range(6, roomSizeMax + 1);

        int[,] tilePositions = new int[roomHeight, roomWidth];

        for (int y = 0; y < roomHeight; y++)
            for (int x = 0; x < roomWidth; x++)
            {
                tilePositions[y, x] = 1;
            }
        return new Room(CalculateRoomPos(), roomWidth, roomHeight, tilePositions);
    }
    private Room GeneratePolygonRoom()
    {
        int pointsSpawnRadius = 16;
        int maxX = -1, maxY = -1;
        int pointsSpawnNum = 32;
        List<Vector2Int> points = GenerateSetOfPoints(pointsSpawnRadius, pointsSpawnNum);
        for (int i = 0; i < points.Count; i++)
        {

            if (maxX < points[i].x)
                maxX = points[i].x;
            if (maxY < points[i].y)
                maxY = points[i].y;
        }
        int[,] tilePositions = new int[++maxY, ++maxX];
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = points.Aggregate(Vector2Int.zero, (current, point) => current + point) / points.Count;
        stack.Push(start);
        int counter = 0;
        while (stack.Count > 0)
        {
            Vector2Int l = stack.Pop();
            int lx = l.x;

            while (IsInsidePolygon(lx, l.y, points) && tilePositions[l.y, lx] == 0)
            {
                tilePositions[l.y, lx] = 1;
                lx--;
                counter++;
            }
            int rx = l.x + 1;

            while (IsInsidePolygon(rx, l.y, points) && tilePositions[l.y, rx] == 0)
            {
                tilePositions[l.y, rx] = 1;
                rx++;
                counter++;
            }

            for (int i = lx; i < rx - 1; i++)
                if (IsInsidePolygon(i, l.y + 1, points) && tilePositions[l.y + 1, i] == 0)
                {
                    stack.Push(new Vector2Int(i, l.y + 1));
                    counter++;
                }
            for (int i = lx; i < rx - 1; i++)
                if (IsInsidePolygon(i, l.y - 1, points) && tilePositions[l.y - 1, i] == 0)
                {
                    stack.Push(new Vector2Int(i, l.y - 1));
                    counter++;
                }
        }
        return new Room(CalculateRoomPos(), maxX, maxY, tilePositions);
    }
    private Room GenerateCircleRoom()
    {
        int roomRadius = Random.Range(4, roomSizeMax / 2 + 1);
        int sizeX = roomRadius * 2; int sizeY = sizeX;
        int[,] tiles = new int[sizeY, sizeX];

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
        return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    }
    private Room GenerateRombusRoom()
    {
        int roomRadius = Random.Range(6, roomSizeMax / 2 + 1);
        int sizeX = roomRadius * 2; int sizeY = sizeX;
        int[,] tiles = new int[sizeY, sizeX];

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
        return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    }
    protected Room GenerateCornerRoom()
    {
        int roomRadius = Random.Range(6, roomSizeMax + 1);
        int sizeX = roomRadius; int sizeY = sizeX;
        int[,] tiles = new int[sizeY, sizeX];


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
        return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    }
    protected Room GenerateTriangleRoom()
    {
        int roomRadius = Random.Range(6, roomSizeMax / 2 + 1);
        int sizeX = 0; int sizeY = 0;
        int[,] tiles = null;
        sizeX = roomRadius * 2;
        sizeY = roomRadius * 2;
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
        return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    }
    private Vector2Int CalculateRoomPos()
    {
        float t = 2 * Mathf.PI * Random.Range(0f, 0.9f);
        float u = Random.Range(0f, 0.9f) + Random.Range(0f, 0.9f);
        float r;
        if (u > 1)
            r = 2 - u;
        else
            r = u;
        return new Vector2Int(Mathf.RoundToInt(roomSpawnRadius * r * Mathf.Cos(t)) + roomSpawnRadius, Mathf.RoundToInt(roomSpawnRadius * r * Mathf.Sin(t)) + roomSpawnRadius);
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
    private Operations TryOperations(Room room, Room roomOther, bool isSub)
    {
        Operations op = Operations.None;
        List<Operations> operations = isSub ? new List<Operations>(GetSubOperationsList) : new List<Operations>(GetOperationsList);
        while (operations.Count > 0)
        {
            int index = Random.Range(0, operations.Count);
            op = operations[index];
            operations.RemoveAt(index);
            Room roomTest;
            switch (op)
            {
                case Operations.Intersect:
                    roomTest = new Room(room);
                    roomTest.Intersect(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.Union:
                    roomTest = new Room(room);
                    roomTest.Union(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.DifferenceAB:
                    roomTest = new Room(room);
                    roomTest.Difference(new Room(roomOther));
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.DifferenceBA:
                    roomTest = new Room(roomOther);
                    roomTest.Difference(new Room(room));
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.SymmetricDifference:
                    roomTest = new Room(room);
                    Room roomOtherTest = new Room(roomOther);
                    roomTest.SymmetricDifference(new Room(roomOtherTest));
                    if (roomTest.Validate() && roomOtherTest.Validate())
                        return op;
                    break;
            }
        }
        return op;
    }
    private bool TryOperation(Room room, Room roomOther, Operations operation)
    {
        Room roomTest;
        switch (operation)
        {
            case Operations.Intersect:
                roomTest = new Room(room);
                roomTest.Intersect(roomOther);
                return roomTest.Validate();

            case Operations.Union:
                roomTest = new Room(room);
                roomTest.Union(roomOther);
                return roomTest.Validate();

            case Operations.DifferenceAB:
                roomTest = new Room(room);
                roomTest.Difference(new Room(roomOther));
                return roomTest.Validate();

            case Operations.DifferenceBA:
                roomTest = new Room(roomOther);
                roomTest.Difference(new Room(room));
                return roomTest.Validate();

            case Operations.SymmetricDifference:
                roomTest = new Room(room);
                Room roomOtherTest = new Room(roomOther);
                roomTest.SymmetricDifference(new Room(roomOtherTest));
                return roomTest.Validate() && roomOtherTest.Validate();

        }
        return false;
    }
    private bool IsInsidePolygon(int x, int y, List<Vector2Int> points)
    {
        bool res = false;
        for (int i = 0, j = points.Count - 1; i < points.Count; j = i++)
            if (((points[i].y > y) != (points[j].y > y)) && (x < (points[j].x - points[i].x) * (y - points[i].y) / (points[j].y - points[i].y) + points[i].x))
                res = !res;
        return res;
    }
}
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
    public static readonly List<Operations> GetOperationsList = new List<Operations>
    {
        Operations.Intersect,
        Operations.Union,
        Operations.DifferenceAB,
        Operations.DifferenceBA,
        Operations.SymmetricDifference

    };
    public static readonly List<Operations> GetSubOperationsList = new List<Operations>
    {
        Operations.Union,
        Operations.DifferenceAB,
        Operations.DifferenceBA,
    };

}