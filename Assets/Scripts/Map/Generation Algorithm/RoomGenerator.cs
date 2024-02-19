using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public static int mapMaxHeight = 512;
    public static int mapMaxWidth = 512;
    private int[,] map;

    private List<Room> rooms;

    protected override int[,] GenerateDungeon()
    {
        return Run();
    }

    protected int[,] Run()
    {
        map = new int[mapMaxWidth, mapMaxHeight];
        rooms = new List<Room>();

        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            rooms.Add(GenerateRandomRoom());
        }

        bool roomNotIntersect = true;
        do
        {
            List<Room> roomList = null;
            roomNotIntersect = true;
            for (int i = 0; i < rooms.Count; i++)
                for (int j = i + 1; j < rooms.Count; j++)
                {
                    if (rooms[i].CheckIntersection(rooms[j]))
                    {
                        roomNotIntersect = false;
                        SetOperations.Operations operation;
                        operation = TryOperations(rooms[i], rooms[j], rooms[i].IsProperSubsetOf(rooms[j]));
                        switch (operation)
                        {
                            case SetOperations.Operations.Intersect:
                                rooms[i].Intersect(rooms[j]);
                                break;

                            case SetOperations.Operations.Union:
                                rooms[i].Union(rooms[j]);
                                break;

                            case SetOperations.Operations.DifferenceAB:
                                rooms[i].Difference(rooms[j]);
                                break;

                            case SetOperations.Operations.DifferenceBA:
                                rooms[j].Difference(rooms[i]);
                                break;

                            case SetOperations.Operations.SymmetricDifference:
                                rooms[i].SymmetricDifference(rooms[j]);
                                break;
                        }
                    }
                    else
                    {
                        if (checkConnection && room.CheckConnection(rooms[i]))
                        {
                            room.Union(rooms[i]);
                            roomsValidatedNew.Add(room);
                        }
                        else
                        {
                            roomsValidatedNew.Add(roomsValidated[i]);
                        }

                    }
                }
            else
            {
                roomsValidatedNew.Add(room);
            }
            if (roomNotIntersect)
                roomsValidatedNew.Add(room);
            roomsValidated = roomsValidatedNew;

            //if (roomsGenerated.Count == 0 && roomsValidated.Count < roomNumber / 2)
            //    for (int i = roomsValidated.Count; i < roomNumber; i++)
            //        roomsGenerated.Add(GenerateRandomRoom());

        } while (!roomNotIntersect);

        for (int i = 0; i < mapMaxWidth; i++)
            for (int j = 0; j < mapMaxHeight; j++)
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

    //protected Room GenerateTriangleRoom()
    //{
    //    int roomRadius = Random.Range(6, roomSizeMax/2 + 1);
    //    int sizeX = 0; int sizeY = 0;
    //    int[,] tiles = null;


    //    switch (Random.Range(0, 4))
    //    {
    //        case 0:
    //            sizeX = roomRadius * 2; 
    //            sizeY = roomRadius;
    //            tiles = new int[sizeY, sizeX];
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[i, j] = 1;
    //                        tiles[i, sizeX - j - 1] = 1;
    //                    }
    //                }
    //            break;
    //        case 1:
    //            sizeX = roomRadius * 2;
    //            sizeY = roomRadius;
    //            tiles = new int[sizeY, sizeX];
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[i, sizeX - j - 1] = 1;
    //                        tiles[sizeY - j - 1, sizeX - i - 1] = 1;
    //                    }
    //                }
    //            break;
    //        case 2:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[sizeX - j - 1, sizeX - i - 1] = 1;
    //                        tiles[sizeY - j - 1, i] = 1;
    //                    }
    //                }
    //            break;
    //        case 3:
    //            for (int i = 0; i < roomRadius; i++)
    //                for (int j = 0; j < roomRadius; j++)
    //                {
    //                    if (j >= roomRadius - i && j <= roomRadius + i)
    //                    {
    //                        tiles[sizeY - j - 1, i] = 1;
    //                        tiles[i, j] = 1;
    //                    }
    //                }
    //            break;
    //    }
    //    return new Room(CalculateRoomPos(), sizeX, sizeY, tiles);
    //}
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
    private SetOperations.Operations TryOperations(Room room, Room roomOther, bool isSub)
    {
        SetOperations.Operations op = SetOperations.Operations.None;
        List<SetOperations.Operations> operations = isSub ? SetOperations.GetSubOperationsList : SetOperations.GetOperationsList;
        while (operations.Count > 0)
        {
            int index = Random.Range(0, operations.Count);
            op = operations[index];
            operations.RemoveAt(index);
            Room roomTest;
            switch (op)
            {
                case SetOperations.Operations.Intersect:
                    roomTest = new Room(room);
                    roomTest.Intersect(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case SetOperations.Operations.Union:
                    roomTest = new Room(room);
                    roomTest.Union(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case SetOperations.Operations.DifferenceAB:
                    roomTest = new Room(room);
                    roomTest.Difference(new Room(roomOther));
                    if (roomTest.Validate())
                        return op;
                    break;

                case SetOperations.Operations.DifferenceBA:
                    roomTest = new Room(roomOther);
                    roomTest.Difference(new Room(room));
                    if (roomTest.Validate())
                        return op;
                    break;

                case SetOperations.Operations.SymmetricDifference:
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

}