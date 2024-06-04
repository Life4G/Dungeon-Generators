using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;
using static SetOperations;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "Geometry Data", menuName = "Geometry Generator", order = 51)]
public class RoomGenerator : DungeonGeneratorBase
{
    private int roomNumberMin = 10;
    private int roomNumberMax = 25;
    private bool checkConnection = true;
    public int mapMaxHeight = 512;
    public int mapMaxWidth = 512;
    private int roomSpawnRadius = 60;
    private int[,] map;
    private int spawnIncreaseRadius = 0;
    private bool careAboutRoomCount = false;

    protected override int[,] GenerateDungeon()
    {
        return GenerateMap();
    }
    protected int[,] GenerateMap()
    {
        map = new int[mapMaxWidth, mapMaxHeight];
        List<Room> rooms = new List<Room>();
        List<Room> roomList = new List<Room>();
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            roomList.Add(Room.CreateRandomRoom(CalculateRoomPos()));
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
            //if (spawnIncreaseRadius > 3)
            //{
            //    spawnIncreaseRadius = 0;
            //    roomSpawnRadius += roomSizeMax;
            //}
            //if (careAboutRoomCount && roomNumberCur < roomNumberMin)
            //{
            //    for (int i = 0; i < roomList.Count && roomNumberCur < roomNumberMin; i++)
            //    {
            //        if (roomList[i] == null)
            //        {
            //            roomList[i] = GenerateRandomRoom();
            //            roomNumberCur++;
            //        }
            //    }
            //    spawnIncreaseRadius++;
            //}
        } while (!roomsValidated);

        for (int i = 0; i < roomList.Count; i++)
            if (roomList[i] != null)
                rooms.Add(roomList[i]);

        AssetDatabase.DeleteAsset("Assets/Scripts/Scriptable Objects/Generators/Geometry/Graph Data.asset");
        graph = Graph.CreateGraph(rooms, mapMaxWidth, mapMaxHeight);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();

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
    private void DrawCorridors(List<Corridor> corridors, int offset)
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
                    roomTest = Room.Copy(room);
                    roomTest.Intersect(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.Union:
                    roomTest = Room.Copy(room);
                    roomTest.Union(roomOther);
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.DifferenceAB:
                    roomTest = Room.Copy(room);
                    roomTest.Difference(Room.Copy(roomOther));
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.DifferenceBA:
                    roomTest = Room.Copy(roomOther);
                    roomTest.Difference(Room.Copy(room));
                    if (roomTest.Validate())
                        return op;
                    break;

                case Operations.SymmetricDifference:
                    roomTest = Room.Copy(room);
                    Room roomOtherTest = Room.Copy(roomOther);
                    roomTest.SymmetricDifference(Room.Copy(roomOtherTest));
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
                roomTest = Room.Copy(room);
                roomTest.Intersect(roomOther);
                return roomTest.Validate();

            case Operations.Union:
                roomTest = Room.Copy(room);
                roomTest.Union(roomOther);
                return roomTest.Validate();

            case Operations.DifferenceAB:
                roomTest = Room.Copy(room);
                roomTest.Difference(Room.Copy(roomOther));
                return roomTest.Validate();

            case Operations.DifferenceBA:
                roomTest = Room.Copy(roomOther);
                roomTest.Difference(Room.Copy(room));
                return roomTest.Validate();

            case Operations.SymmetricDifference:
                roomTest = Room.Copy(room);
                Room roomOtherTest = Room.Copy(roomOther);
                roomTest.SymmetricDifference(Room.Copy(roomOtherTest));
                return roomTest.Validate() && roomOtherTest.Validate();

        }
        return false;
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