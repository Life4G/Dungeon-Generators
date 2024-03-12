using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;
using static SetOperations;
using UnityEngine.AI;

public class RoomGenerator : DungeonGeneratorBase
{
    [SerializeField]
    private int radiusOfRoomSpawn = 120;
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

        //for (int i = 0; i < rooms.Count - 1; i++)
        //    roomConnections.Add(new RoomConnection(i, i + 1, rooms[i].GetPosCenter(), rooms[i + 1].GetPosCenter()));
        // Graph
        graph = new Graph(rooms, roomConnections);
        roomConnections = Triangulation(rooms, mapMaxWidth, mapMaxHeight);

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

                    map[y, x] = rooms.Count+i;
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
    private List<RoomConnection> Triangulation(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < rooms.Count; i++)
            points.Add(rooms[i].GetPosCenter());
        Triangle superTriangle = Triangle.SuperTriangle(points, maxWidth, maxHeigth);
        List<Triangle> triangles = new List<Triangle> { superTriangle };

        for (int i = 0; i < points.Count; i++)
        {
            triangles = AddVertex(points[i], i, triangles);
        }
        List<RoomConnection> roomConnections = new List<RoomConnection>();
        for (int i = 0; i < triangles.Count; i++)
        {

            RoomConnection connection1 = new RoomConnection(triangles[i].edges[0], triangles[i].edges[1], Vector2Int.RoundToInt(triangles[i].posPointFirst), Vector2Int.RoundToInt(triangles[i].posPointSecond));
            RoomConnection connection2 = new RoomConnection(triangles[i].edges[1], triangles[i].edges[2], Vector2Int.RoundToInt(triangles[i].posPointSecond), Vector2Int.RoundToInt(triangles[i].posPointThird));
            RoomConnection connection3 = new RoomConnection(triangles[i].edges[2], triangles[i].edges[0], Vector2Int.RoundToInt(triangles[i].posPointThird), Vector2Int.RoundToInt(triangles[i].posPointFirst));

            if (connection1.roomFirst > -1 && connection1.roomSecond > -1 )
                roomConnections.Add(connection1);
            if (connection2.roomFirst > -1 && connection2.roomSecond > -1 )
                roomConnections.Add(connection2);
            if (connection3.roomFirst > -1 && connection3.roomSecond > -1 )
                roomConnections.Add(connection3);

        };
        return roomConnections;
    }
    private List<Triangle> AddVertex(Vector2 vertex, int roomId, List<Triangle> triangles)
    {
        List<Edge> edges = new List<Edge>();
        List<Triangle> badTriangles = new List<Triangle>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].circle.radius != -1)
            {
                if (triangles[i].InCircle(vertex))
                {
                    edges.Add(new Edge(triangles[i].posPointFirst, triangles[i].posPointSecond, triangles[i].edges[0], triangles[i].edges[1]));
                    edges.Add(new Edge(triangles[i].posPointSecond, triangles[i].posPointThird, triangles[i].edges[1], triangles[i].edges[2]));
                    edges.Add(new Edge(triangles[i].posPointThird, triangles[i].posPointFirst, triangles[i].edges[2], triangles[i].edges[0]));
                    badTriangles.Add(triangles[i]);
                }
            }
        }
        foreach (Triangle triangle in badTriangles)
        {
            triangles.Remove(triangle);
        }
        List<Edge> uniqueEdges = new List<Edge>();
        for (int i = 0; i < edges.Count; i++)
        {
            bool isUnique = true;
            for (int j = 0; j < edges.Count; j++)
            {
                if (i != j && edges[i] == edges[j])
                {
                    isUnique = false;
                    break;
                }
            }
            if (isUnique)
                uniqueEdges.Add(edges[i]);
        }

        foreach (Edge edge in uniqueEdges)
        {
                triangles.Add(new Triangle(edge.posPointFirst, edge.posPointSecond, vertex, new int[3] { edge.idFirst, edge.idSecond, roomId }));
        }
        return triangles;
    }

}

public class Edge
{
    public int idFirst;
    public int idSecond;
    public Vector2 posPointFirst;
    public Vector2 posPointSecond;
    public bool isUnque;

    public Edge(Vector2 posPointFirst, Vector2 posPointSecond, int idFirst, int idSecond)
    {
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
        this.idFirst = idFirst;
        this.idSecond = idSecond;
        isUnque = true;
    }
    public static bool operator ==(Edge first, Edge second)
    {
        return first.posPointFirst == second.posPointFirst && first.posPointSecond == second.posPointSecond
            || first.posPointFirst == second.posPointSecond && first.posPointSecond == second.posPointFirst;
    }
    public static bool operator !=(Edge first, Edge second)
    {
        return !(first == second);
    }
    public override bool Equals(object other)
    {
        if (!(other is Edge))
        {
            return false;
        }

        return Equals((Edge)other);
    }
    public bool Equals(Edge other)
    {
        return posPointFirst == other.posPointFirst && posPointSecond == other.posPointSecond
        || posPointFirst == other.posPointSecond && posPointSecond == other.posPointFirst;
    }
    public override int GetHashCode()
    {
        return posPointFirst.GetHashCode() ^ (posPointSecond.GetHashCode() << 2);
    }

}
public class Triangle
{
    public int[] edges;
    public Vector2 posPointFirst;
    public Vector2 posPointSecond;
    public Vector2 posPointThird;
    public CircumCircle circle;

    public Triangle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird, int[] edgesId)
    {
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
        this.posPointThird = posPointThird;
        circle = new CircumCircle(posPointFirst, posPointSecond, posPointThird);
        edges = edgesId;
    }
    public static Triangle SuperTriangle(List<Vector2> pointsList, int maxWidth, int maxHeigth)
    {
        //int minX = 512, minY = 512;
        //int maxX = -1, maxY = -1;
        //foreach (Vector2Int point in pointsList)
        //{
        //    minX = Math.Min(minX, point.x);
        //    minY = Math.Min(minY, point.y);
        //    maxX = Math.Max(maxX, point.x);
        //    maxY = Math.Max(maxY, point.y);
        //}
        //int dx = (maxX - minX) * 100;
        //int dy = (maxY - minY) * 100;
        //return new Triangle(new Vector2Int(minX - dx, minY - dy * 3), new Vector2Int(minX - dx, minY + dy), new Vector2Int(minX + dx * 3, minY + dy), new int[3] { -1, -2, -3 });
        int offsetX = maxWidth * 4;
        int offsetY = maxHeigth * 4;
        return new Triangle(new Vector2(0 - offsetX, 0 - offsetY), new Vector2(maxWidth + offsetX, 0 - offsetY), new Vector2(maxWidth + offsetX, maxHeigth + offsetY), new int[3] { -1, -2, -3 });
    }
    public bool InCircle(Vector2 vertex)
    {
        if ((vertex.x - circle.center.x) * (vertex.x - circle.center.x)  + (vertex.y - circle.center.y) * (vertex.y - circle.center.y) <= circle.radius * circle.radius)
            return true;
        else
            return false;
    }
    public class CircumCircle
    {
        public Vector2 center;
        public float radius;
        public CircumCircle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
        {
            if (!IsPerpendicular(posPointFirst, posPointSecond, posPointThird))
                getCircumCircle(posPointFirst, posPointSecond, posPointThird);
            else if (!IsPerpendicular(posPointFirst, posPointThird, posPointSecond))
                getCircumCircle(posPointFirst, posPointThird, posPointSecond);
            else if (!IsPerpendicular(posPointSecond, posPointFirst, posPointThird))
                getCircumCircle(posPointSecond, posPointFirst, posPointThird);
            else if (!IsPerpendicular(posPointSecond, posPointThird, posPointFirst))
                getCircumCircle(posPointSecond, posPointThird, posPointFirst);
            else if (!IsPerpendicular(posPointThird, posPointSecond, posPointFirst))
                getCircumCircle(posPointThird, posPointSecond, posPointFirst);
            else if (!IsPerpendicular(posPointThird, posPointFirst, posPointSecond))
                getCircumCircle(posPointThird, posPointFirst, posPointSecond);
            else
            {
                //The three points are perpendicular to axis
                radius = -1;
            }
        }
        private bool IsPerpendicular(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
        {
            float deltaYSecondFirst = Math.Abs(posPointSecond.y - posPointFirst.y);
            float deltaXSecondFirst = Math.Abs(posPointSecond.x - posPointFirst.x);
            float deltaYThirdSecond = Math.Abs(posPointThird.y - posPointSecond.y);
            float deltaXThirdSecond = Math.Abs(posPointThird.x - posPointSecond.x);
            //The points are pependicular and parallel to x-y axis
            if (deltaXSecondFirst <= 0 && deltaYThirdSecond <= 0)
                return false;
            //A line of two point are perpendicular to x - axis
            if (deltaYSecondFirst <= 0)
                return true;
            //A line of two point are perpendicular to x - axis
            else if (deltaYThirdSecond <= 0)
                return true;
            //A line of two point are perpendicular to y - axis
            else if (deltaXSecondFirst <= 0)
                return true;
            //A line of two point are perpendicular to y - axis
            else if (deltaXThirdSecond <= 0)
                return true;
            return false;
        }

        private void getCircumCircle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
        {
            float deltaYSecondFirst = Math.Abs(posPointSecond.y - posPointFirst.y);
            float deltaXSecondFirst = Math.Abs(posPointSecond.x - posPointFirst.x);
            float deltaYThirdSecond = Math.Abs(posPointThird.y - posPointSecond.y);
            float deltaXThirdSecond = Math.Abs(posPointThird.x - posPointSecond.x);
            if (deltaXSecondFirst <= 0 && deltaYThirdSecond <= 0)
            {
                center = new Vector2(posPointSecond.x + posPointThird.x, posPointFirst.y + posPointSecond.y) * 0.5f;
                radius = Math.Max(Math.Max(Vector2.Distance(center, posPointFirst), Vector2.Distance(center, posPointSecond)), Vector2.Distance(center, posPointThird));
                return;
            }
            float slopeFirst = deltaYSecondFirst / deltaXSecondFirst;
            float slopeSecond = deltaYThirdSecond / deltaXThirdSecond;
            if (Math.Abs(slopeFirst - slopeSecond) <= 0)
            {
                radius = -1;
                return;
            }
            float x = (slopeFirst * slopeSecond * (posPointFirst.y - posPointThird.y) + slopeSecond * (posPointFirst.x + posPointSecond.x) - slopeFirst * (posPointSecond.x + posPointThird.x))
                / (2 * (slopeSecond - slopeFirst));
            float y = -1 * (x - (posPointFirst.x + posPointSecond.x) / 2) / slopeFirst + (posPointFirst.y + posPointSecond.y) / 2;
            center = new Vector2(x, y);
            radius = Math.Max(Math.Max(Vector2.Distance(center, posPointFirst), Vector2.Distance(center, posPointSecond)), Vector2.Distance(center, posPointThird));

            //float ox = (Math.Min(Math.Min(posPointFirst.x, posPointSecond.x), posPointThird.x) + Math.Max(Math.Max(posPointFirst.x, posPointSecond.x), posPointThird.x)) / 2;
            //float oy = (Math.Min(Math.Min(posPointFirst.y, posPointSecond.y), posPointThird.y) + Math.Max(Math.Max(posPointFirst.y, posPointSecond.y), posPointThird.y)) / 2;
            //float ax = posPointFirst.x - ox, ay = posPointFirst.y - oy;
            //float bx = posPointSecond.x - ox, by = posPointSecond.y - oy;
            //float cx = posPointThird.x - ox, cy = posPointThird.y - oy;
            //float d = (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) * 2;
            //if (d == 0)
            //    radius = -1;
            //float x = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            //float y = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            //center = new Vector2(x, y);
            //radius = Math.Max(Math.Max(Vector2.Distance(center,posPointFirst), Vector2.Distance(center, posPointSecond)), Vector2.Distance(center, posPointThird));
        }
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

    public static bool operator ==(RoomConnection first, RoomConnection second)
    {
        return first.posFirst == second.posFirst && first.posSecond == second.posSecond
            || first.posFirst == second.posSecond && first.posSecond == second.posFirst;
    }
    public static bool operator !=(RoomConnection first, RoomConnection second)
    {
        return !(first == second);
    }
    public override bool Equals(object other)
    {
        if (!(other is RoomConnection))
        {
            return false;
        }

        return Equals((RoomConnection)other);
    }
    public bool Equals(RoomConnection other)
    {
        return posFirst == other.posFirst && posSecond == other.posSecond
        || posFirst == other.posSecond && posSecond == other.posFirst;
    }
    public override int GetHashCode()
    {
        return posFirst.GetHashCode() ^ (posSecond.GetHashCode() << 2);
    }

}
