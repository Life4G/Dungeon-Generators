using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;
using static SetOperations;
using static UnityEngine.EventSystems.EventTrigger;

public class RoomGenerator : DungeonGeneratorBase
{
    [SerializeField]
    private int radiusOfRoomSpawn = 40;
    [SerializeField]
    private int roomNumberMin = 4;
    [SerializeField]
    private int roomNumberMax = 6;
    [SerializeField]
    int roomSizeMax = 16;
    [SerializeField]
    bool checkConnection = true;
    public readonly static int mapMaxHeight = 512;
    public readonly static int mapMaxWidth = 512;
    private int[,] map;

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

        bool roomsValidated;
        do
        {
            roomsValidated = true;
            int index = -1;
            Room room = null;
            Operations operation = Operations.None;
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
                        operation = TryOperations(room, roomList[i], room.IsProperSubsetOf(roomList[i]));
                        switch (operation)
                        {
                            case Operations.Intersect:
                                room.Intersect(roomList[i]);
                                roomList[i] = null;
                                break;

                            case Operations.Union:
                                room.Union(roomList[i]);
                                roomList[i] = null;
                                break;

                            case Operations.DifferenceAB:
                                room.Difference(roomList[i]);
                                roomList[i] = null;
                                break;

                            case Operations.DifferenceBA:
                                roomList[i].Difference(room);
                                roomList[i].SetValidation(false);
                                room = null;
                                break;

                            case Operations.SymmetricDifference:
                                room.SymmetricDifference(roomList[i]);
                                roomList[i].SetValidation(false);
                                break;
                        }
                    }
                    else
                    {
                        if (checkConnection && room.CheckConnection(roomList[i]))
                        {
                            room.Union(roomList[i]);
                            roomList[i] = null;
                        }
                    }
                }
            }
            if (room != null)
            {
                if (room.Validate())
                {
                    room.SetValidation(true);
                    roomList[index] = room;
                }
                else
                {
                    if (operation == Operations.None && roomList.Count - 1 > roomNumber)
                        roomList.Remove(room);
                    else
                    {
                        room = GenerateRandomRoom();
                        roomList[index] = room;
                    }
                }

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

            for (int y = 0; y < roomSize.Height && y + roomPos.y < mapMaxHeight; y++)
                for (int x = 0; x < roomSize.Width && x + roomPos.x < mapMaxWidth; x++)
                {
                    if (roomTiles[y, x] != 0)
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
        switch (Random.Range(0, 3))
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
                room = GenerateCornerRoom();
                break;
            case 4:
                room = GenerateTriangleRoom();
                break;

        }
        return room;
    }
    private Room GenerateSquareRoom()
    {
        int roomWidth = Random.Range(6, roomSizeMax + 1);
        int roomHeight = Random.Range(6, roomSizeMax + 1);

        int[,] tilePositions = new int[roomHeight, roomWidth];

        int tilesNum = 0;
        for (int y = 0; y < roomHeight; y++)
            for (int x = 0; x < roomWidth; x++)
            {
                tilePositions[y, x] = 1;
                tilesNum++;
            }
        return new Room(CalculateRoomPos(), roomWidth, roomHeight, tilePositions);
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


        switch (Random.Range(0, 4))
        {
            case 0:
                sizeX = roomRadius * 2;
                sizeY = roomRadius;
                tiles = new int[sizeY, sizeX];
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
                sizeX = roomRadius * 2;
                sizeY = roomRadius;
                tiles = new int[sizeY, sizeX];
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
        return new Vector2Int(Mathf.RoundToInt(radiusOfRoomSpawn * r * Mathf.Cos(t)) + radiusOfRoomSpawn, Mathf.RoundToInt(radiusOfRoomSpawn * r * Mathf.Sin(t)) + radiusOfRoomSpawn);
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
}
//private Room OperationApplication(Room room, Room roomOther)
//{
//    Room result = null;
//    Operations operation;
//    operation = TryOperations(room, roomOther, room.IsProperSubsetOf(roomOther));
//    switch (operation)
//    {
//        case Operations.Intersect:
//            room.Intersect(roomOther);
//            break;

//        case Operations.Union:
//            room.Union(roomOther);
//            break;

//        case Operations.DifferenceAB:
//            room.Difference(roomOther);
//            break;

//        case Operations.DifferenceBA:
//            roomOther.Difference(room);
//            break;

//        case Operations.SymmetricDifference:
//            room.SymmetricDifference(roomOther);
//            break;
//    }
//    return result;
//}

//private List<Room> RoomCollison()
//{
//    int Ymax = mapMaxHeight / (roomSizeMax * 2);
//    int Xmax = mapMaxWidth / (roomSizeMax * 2);
//    int[,] collisionMap = new int[Ymax, Xmax];

//    Dictionary<int,Vector2Int> roomsCentres = new Dictionary<int, Vector2Int>();
//    bool roomsValidated;
//    for (int y = 0; y < Ymax; y++)
//        for (int x = 0; x < Xmax; x++)
//            collisionMap[y, x] = -1;
//    do
//    {
//        roomsValidated = true;

//        for (int i = 0; i < rooms.Count; i++)
//        {
//            if (rooms[i] != null)
//            {
//                if (roomsCentres.ContainsValue(rooms[i].GetPosCenter() / (roomSizeMax * 2)))
//                {

//                    Operations operation;
//                    operation = TryOperations(rooms[i], rooms[collisionMap[y, x]], rooms[i].IsProperSubsetOf(rooms[collisionMap[y, x]]));
//                    switch (operation)
//                    {
//                        case Operations.Intersect:
//                            rooms[i].Intersect(rooms[collisionMap[y, x]]);
//                            rooms[collisionMap[y, x]] = null;
//                            break;

//                        case Operations.Union:
//                            rooms[i].Union(rooms[collisionMap[y, x]]);
//                            rooms[collisionMap[y, x]] = null;
//                            break;

//                        case Operations.DifferenceAB:
//                            rooms[i].Difference(rooms[collisionMap[y, x]]);
//                            rooms[collisionMap[y, x]] = null;
//                            break;

//                        case Operations.DifferenceBA:
//                            rooms[collisionMap[y, x]].Difference(rooms[i]);
//                            rooms[i] = null;
//                            break;

//                        case Operations.SymmetricDifference:
//                            rooms[i].SymmetricDifference(rooms[collisionMap[y, x]]);
//                            break;
//                    }
//                }
//                else
//                    roomsCentres.Add(i,rooms[i].GetPosCenter() / (roomSizeMax * 2));

//                if (!rooms[i].GetValidation())
//                    roomsValidated = false;
//            }

//            collisionMap[roomsCentres[i].y, roomsCentres[i].x] = i;
//        }
//        for (int i = 0; rooms[i] != null && i < rooms.Count; i++)
//        {
//            for (int y = roomsCentres[i].y - 1; y < roomsCentres[i].y + 1; y++)
//                for (int x = roomsCentres[i].x - 1; y < roomsCentres[i].x + 1; x++)
//                    if (collisionMap[y, x] != -1 && collisionMap[y, x] != i && rooms[i].CheckIntersection(rooms[collisionMap[y, x]]))
//                    {
//                        Operations operation;
//                        operation = TryOperations(rooms[i], rooms[collisionMap[y, x]], rooms[i].IsProperSubsetOf(rooms[collisionMap[y, x]]));
//                        switch (operation)
//                        {
//                            case Operations.Intersect:
//                                rooms[i].Intersect(rooms[collisionMap[y, x]]);
//                                rooms[collisionMap[y, x]] = null;
//                                break;

//                            case Operations.Union:
//                                rooms[i].Union(rooms[collisionMap[y, x]]);
//                                rooms[collisionMap[y, x]] = null;
//                                break;

//                            case Operations.DifferenceAB:
//                                rooms[i].Difference(rooms[collisionMap[y, x]]);
//                                rooms[collisionMap[y, x]] = null;
//                                break;

//                            case Operations.DifferenceBA:
//                                rooms[collisionMap[y, x]].Difference(rooms[i]);
//                                rooms[i] = null;
//                                break;

//                            case Operations.SymmetricDifference:
//                                rooms[i].SymmetricDifference(rooms[collisionMap[y, x]]);
//                                break;
//                        }
//                    }
//                    else
//                    {
//                        if (checkConnection && rooms[i].CheckConnection(rooms[collisionMap[y, x]]))
//                        {
//                            rooms[i].Union(rooms[i]);
//                            rooms[i] = null;
//                        }
//                        else
//                            rooms[i].SetValidation(true);
//                    }
//        }
//    }
//    while (!roomsValidated);
//    List<Room> roomCollison = new List<Room>();
//    for (int i = 0; i < rooms.Count; i++)
//        if (rooms[i] != null)
//            roomCollison.Add(rooms[i]);
//    return roomCollison;
//}

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