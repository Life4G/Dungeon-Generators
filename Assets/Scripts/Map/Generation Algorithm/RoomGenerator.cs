using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static SetOperations;
using UnityEditor.PackageManager;

public class RoomGenerator : DungeonGeneratorBase
{
    [SerializeField]
    private int radiusOfRoomSpawn = 120;
    //Мин и макс комнат которое может быть нагенерено
    [SerializeField]
    private int roomNumberMin = 16;
    [SerializeField]
    private int roomNumberMax = 64;
    [SerializeField]
    int roomSizeMax = 16;
    [SerializeField]
    bool checkConnection = true;
    public readonly static int mapMaxHeight = 512;
    public readonly static int mapMaxWidth = 512;
    private int[,] map;

    private List<Room> rooms;
    private List<RoomConnection> roomConnections;

    protected override int[,] GenerateDungeon()
    {
        return Run();
    }

    protected int[,] Run()
    {
        map = new int[mapMaxWidth, mapMaxHeight];
        rooms = new List<Room>();
        List<Room> roomList = new List<Room>();
        roomConnections = new List<RoomConnection>();
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            roomList.Add(GenerateRandomRoom());
        }

        bool roomsValidated;
        do
        {
            roomsValidated = true;
            Room room = null;
            for (int i = 0; i < roomList.Count && room == null; i++)
            {
                if (roomList[i] != null && !roomList[i].GetValidation())
                {
                    room = roomList[i];
                    roomsValidated = false;
                }
            }
            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i] != null && room != null && roomList[i] != room)
                {

                    if (room.CheckIntersection(roomList[i]))
                    {
                        Operations operation;
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
                                room = null;
                                break;

                            case Operations.SymmetricDifference:
                                room.SymmetricDifference(roomList[i]);
                                break;
                        }
                    }
                    else
                    {
                        if (checkConnection && room.CheckConnection(roomList[i]))
                        {
                            room.Union(rooms[i]);
                            rooms[i] = null;
                        }

                    }

                }
            }
            if (room != null)
            {
                room.SetValidation(true);
            }
            //if (roomsGenerated.Count == 0 && roomsValidated.Count < roomNumber / 2)
            //    for (int i = roomsValidated.Count; i < roomNumber; i++)
            //        roomsGenerated.Add(GenerateRandomRoom());

        } while (!roomsValidated);

        for (int i = 0; i < roomList.Count; i++)
            if (roomList[i] != null)
                rooms.Add(roomList[i]);

        for (int i = 0; i < rooms.Count - 1; i++)
            roomConnections.Add(new RoomConnection(i, i + 1, rooms[i].GetPosCenter(), rooms[i + 1].GetPosCenter()));

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
        for (int i = 0; i < roomConnections.Count; i++)
        {
            int dx = Mathf.Abs(roomConnections[i].posSecond.x - roomConnections[i].posFirst.x);
            int sx = roomConnections[i].posFirst.x < roomConnections[i].posSecond.x ? 1 : -1;
            int dy = -Mathf.Abs(roomConnections[i].posSecond.y - roomConnections[i].posFirst.y);
            int sy = roomConnections[i].posFirst.y < roomConnections[i].posSecond.y ? 1 : -1;
            int error = dx + dy;
            int x = roomConnections[i].posFirst.x;
            int y = roomConnections[i].posFirst.y;
            while (true)
            {
                if (map[y, x] == -1)
                {
                    map[y, x + 1] = rooms.Count;
                    map[y, x] = rooms.Count;
                    map[y + 1, x] = rooms.Count;
                }
                if (x == roomConnections[i].posSecond.x && y == roomConnections[i].posSecond.y)
                    break;
                int e2 = 2 * error;
                if (e2 >= dy)
                {
                    if (x == roomConnections[i].posSecond.x)
                        break;
                    error = error + dy;
                    x += sx;
                };
                if (e2 <= dx)
                {
                    if (y == roomConnections[i].posSecond.y)
                        break;
                    error = error + dx;
                    y += sy;
                };

            }
        }

        return map;
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
                //case 3:
                //    room = GenerateCornerRoom();
                //    break;
                //case 4:
                //    room = GenerateTriangleRoom();
                //    break;

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
    //protected Room GenerateCornerRoom()
    //{
    //    int roomRadius = Random.Range(6, roomSizeMax + 1);
    //    int sizeX = roomRadius; int sizeY = sizeX;
    //    int[,] tiles = new int[sizeY, sizeX];


    //    switch (Random.Range(0, 4))
    //    {
    //        case 0:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[i, j] = 1;
    //                    }
    //                }
    //            break;
    //        case 1:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[i, sizeX - j - 1] = 1;
    //                    }
    //                }
    //            break;
    //        case 2:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[sizeX - j - 1, i] = 1;
    //                    }
    //                }
    //            break;
    //        case 3:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[sizeX - j - 1, sizeX - i - 1] = 1;
    //                    }
    //                }
    //            break;
    //    }
    //    return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    //}
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
    private List<Room> RoomCollison()
    {
        int Ymax = mapMaxHeight / (roomSizeMax * 2);
        int Xmax = mapMaxWidth / (roomSizeMax * 2);
        int[,] collisionMap = new int[Ymax, Xmax];
        Vector2Int[] roomsCentres = new Vector2Int[rooms.Count];
        bool roomsValidated;
        do
        {
            roomsValidated = true;

            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i] != null)
                {
                    roomsCentres[i] = rooms[i].GetPosCenter() / (roomSizeMax * 2);
                    if (!rooms[i].GetValidation())

                        roomsValidated = false;
                }
                else
                    roomsCentres[i] = Vector2Int.zero;


                collisionMap[roomsCentres[i].y, roomsCentres[i].x] = i;
            }
            for (int i = 0; rooms[i] != null && i < rooms.Count; i++)
            {
                for (int y = roomsCentres[i].y - 1; y < roomsCentres[i].y + 1; y++)
                    for (int x = roomsCentres[i].x - 1; y < roomsCentres[i].x + 1; x++)
                        if (collisionMap[y, x] != 0 && rooms[i].CheckIntersection(rooms[collisionMap[y, x]]))
                        {
                            Operations operation;
                            operation = TryOperations(rooms[i], rooms[collisionMap[y, x]], rooms[i].IsProperSubsetOf(rooms[collisionMap[y, x]]));
                            switch (operation)
                            {
                                case Operations.Intersect:
                                    rooms[i].Intersect(rooms[collisionMap[y, x]]);
                                    rooms[collisionMap[y, x]] = null;
                                    break;

                                case Operations.Union:
                                    rooms[i].Union(rooms[collisionMap[y, x]]);
                                    rooms[collisionMap[y, x]] = null;
                                    break;

                                case Operations.DifferenceAB:
                                    rooms[i].Difference(rooms[collisionMap[y, x]]);
                                    rooms[collisionMap[y, x]] = null;
                                    break;

                                case Operations.DifferenceBA:
                                    rooms[collisionMap[y, x]].Difference(rooms[i]);
                                    rooms[i] = null;
                                    break;

                                case Operations.SymmetricDifference:
                                    rooms[i].SymmetricDifference(rooms[collisionMap[y, x]]);
                                    break;
                            }
                        }
                        else
                        {
                            if (checkConnection && rooms[i].CheckConnection(rooms[collisionMap[y, x]]))
                            {
                                rooms[i].Union(rooms[i]);
                                rooms[i] = null;
                            }
                            else
                                rooms[i].SetValidation(true);
                        }
            }
        }
        while (!roomsValidated);
        List<Room> roomCollison = new List<Room>();
        for (int i = 0; i < rooms.Count; i++)
            if (rooms[i] != null)
                roomCollison.Add(rooms[i]);
        return roomCollison;
    }


}

public class RoomConnection
{
    public int roomFirst,
        roomSecond;
    public Vector2Int posFirst,
        posSecond;

    public RoomConnection(int roomFirstID, int roomSecondID, Vector2Int firstPos, Vector2Int secondPos)
    {
        roomFirst = roomFirstID;
        roomSecond = roomSecondID;
        posFirst = firstPos;
        posSecond = secondPos;

    }
}