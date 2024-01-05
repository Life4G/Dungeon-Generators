using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : DungeonGeneratorBase
{
    //Радиус для выбора рандомной точки на окружности
    [SerializeField]
    private int radius = 120;
    //Мин и макс комнат которое может быть нагенерено
    [SerializeField]
    private int roomNumberMin = 16;
    [SerializeField]
    private int roomNumberMax = 64;
    private static int mapMaxHeight = 256;
    private static int mapMaxWidth = 256;
    private int[,] map = new int[mapMaxWidth, mapMaxHeight];

    public List<Room> roomsGenerated;
    private List<Room> roomsValidated;

    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        return Run();
    }

    protected int[,] GenerateDungeonMap()
    {
        return RunMap();
    }
    protected HashSet<Vector2Int> Run()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        roomsGenerated = new List<Room>();
        //Генерируем несколько комнат разных форм
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            roomsGenerated.Add(GenerateRandomRoom());
        }
        //Проводим операции над комнатами (гугли теорию по операциям на множестве если что)
        bool roomNotIntersect = true;
        do
        {
            List<Room> roomsValidatedNew = new List<Room>();
            Room room = null;
            if (roomsGenerated.Count != 0)
            {
                room = roomsGenerated.First();
                roomsGenerated.Remove(room);
            }
            else
            {
                room = roomsValidated.First();
                roomsValidated.Remove(room);
            }
            roomNotIntersect = true;
            if (roomsValidated != null)
                for (int i = 0; i < roomsValidated.Count; i++)
                {

                    if (room.CheckIntersection(roomsValidated[i]))
                    {
                        roomNotIntersect = false;
                        if (room.IsProperSubsetOf(roomsValidated[i]))
                        {
                            List<SetOperations.Operations> operations = SetOperations.GetSubOperationsList;
                            SetOperations.Operations operation = SetOperations.GetRandomSubOperation();
                            operations.Remove(operation);
                            if (!room.TryOperation(roomsValidated[i], operation))
                            {
                                operation = room.TryAllSubOperations(roomsValidated[i]);
                                if (operation == SetOperations.Operations.None)
                                {
                                    roomsGenerated.Add(GenerateRandomRoom());
                                    roomsValidatedNew.Add(roomsValidated[i]);
                                }
                                else
                                {
                                    room.DoOperation(roomsValidated[i], operation);
                                    roomsValidatedNew.Add(room);
                                }
                            }
                            else
                            {
                                room.DoOperation(roomsValidated[i], operation);
                                roomsValidatedNew.Add(room);
                            }
                        }
                        else
                        {
                            List<SetOperations.Operations> operations = SetOperations.GetOperationsList;
                            SetOperations.Operations operation = SetOperations.GetRandomOperation();
                            operations.Remove(operation);
                            if (!room.TryOperation(roomsValidated[i], operation))
                            {
                                operation = room.TryAllOperations(roomsValidated[i]);
                                if (operation == SetOperations.Operations.None)
                                {
                                    roomsGenerated.Add(GenerateRandomRoom());
                                    roomsValidatedNew.Add(roomsValidated[i]);
                                }
                                else
                                {
                                    room.DoOperation(roomsValidated[i], operation);
                                    roomsValidatedNew.Add(room);
                                }
                            }
                            else
                            {
                                room.DoOperation(roomsValidated[i], operation);
                                roomsValidatedNew.Add(room);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (room.CheckConnection(roomsValidated[i]))
                        {
                            room.Union(roomsValidated[i]);
                            roomsValidatedNew.Add(room);
                        }
                        else
                        {
                            if (roomNotIntersect)
                                roomsValidatedNew.Add(room);
                            roomsValidatedNew.Add(roomsValidated[i]);
                        }

                    }
                }
            else
            {
                roomsValidatedNew.Add(room);
            }
            roomsValidated = roomsValidatedNew;

            if (roomsGenerated.Count == 0 && roomsValidated.Count < roomNumber / 2)
                for (int i = roomsValidated.Count; i < roomNumber; i++)
                    roomsGenerated.Add(GenerateRandomRoom());

        } while (roomsGenerated.Count > 0 || !roomNotIntersect);

        for (int i = 0; i < mapMaxWidth; i++)
            for (int j = 0; j < mapMaxHeight; j++)
            {
                map[i, j] = 0;
            }

        for (int i = 0; i < roomsValidated.Count; i++)
        {
            for (int x = 0; x < mapMaxWidth; x++)
                for (int y = 0; y < mapMaxHeight; y++)
                {
                    if (roomsValidated[i].GetPos().x == x && roomsValidated[i].GetPos().y == y)
                        map[x, y] = (int)roomsValidated[i].GetStyle();
                }
            floorPositions.UnionWith(roomsValidated[i].GetTilesPos());
        }

        return floorPositions;
    }

    protected int[,] RunMap()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        roomsGenerated = new List<Room>();
        //Генерируем несколько комнат разных форм
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            roomsGenerated.Add(GenerateRandomRoom());
        }
        //Проводим операции над комнатами (гугли теорию по операциям на множестве если что)
        bool roomNotIntersect = true;
        do
        {
            List<Room> roomsValidatedNew = new List<Room>();
            Room room = null;
            if (roomsGenerated.Count != 0)
            {
                room = roomsGenerated.First();
                roomsGenerated.Remove(room);
            }
            else
            {
                room = roomsValidated.First();
                roomsValidated.Remove(room);
            }
            roomNotIntersect = true;
            if (roomsValidated != null)
                for (int i = 0; i < roomsValidated.Count; i++)
                {

                    if (room.CheckIntersection(roomsValidated[i]))
                    {
                        roomNotIntersect = false;
                        if (room.IsProperSubsetOf(roomsValidated[i]))
                        {
                            List<SetOperations.Operations> operations = SetOperations.GetSubOperationsList;
                            SetOperations.Operations operation = SetOperations.GetRandomSubOperation();
                            operations.Remove(operation);
                            if (!room.TryOperation(roomsValidated[i], operation))
                            {
                                operation = room.TryAllSubOperations(roomsValidated[i]);
                                if (operation == SetOperations.Operations.None)
                                {
                                    roomsGenerated.Add(GenerateRandomRoom());
                                    roomsValidatedNew.Add(roomsValidated[i]);
                                }
                                else
                                {
                                    room.DoOperation(roomsValidated[i], operation);
                                    roomsValidatedNew.Add(room);
                                }
                            }
                            else
                            {
                                room.DoOperation(roomsValidated[i], operation);
                                roomsValidatedNew.Add(room);
                            }
                        }
                        else
                        {
                            List<SetOperations.Operations> operations = SetOperations.GetOperationsList;
                            SetOperations.Operations operation = SetOperations.GetRandomOperation();
                            operations.Remove(operation);
                            if (!room.TryOperation(roomsValidated[i], operation))
                            {
                                operation = room.TryAllOperations(roomsValidated[i]);
                                if (operation == SetOperations.Operations.None)
                                {
                                    roomsGenerated.Add(GenerateRandomRoom());
                                    roomsValidatedNew.Add(roomsValidated[i]);
                                }
                                else
                                {
                                    room.DoOperation(roomsValidated[i], operation);
                                    roomsValidatedNew.Add(room);
                                }
                            }
                            else
                            {
                                room.DoOperation(roomsValidated[i], operation);
                                roomsValidatedNew.Add(room);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (room.CheckConnection(roomsValidated[i]))
                        {
                            room.Union(roomsValidated[i]);
                            roomsValidatedNew.Add(room);
                        }
                        else
                        {
                            if (roomNotIntersect)
                                roomsValidatedNew.Add(room);
                            roomsValidatedNew.Add(roomsValidated[i]);
                        }

                    }
                }
            else
            {
                roomsValidatedNew.Add(room);
            }
            roomsValidated = roomsValidatedNew;

            if (roomsGenerated.Count == 0 && roomsValidated.Count < roomNumber / 2)
                for (int i = roomsValidated.Count; i < roomNumber; i++)
                    roomsGenerated.Add(GenerateRandomRoom());

        } while (roomsGenerated.Count > 0 || !roomNotIntersect);

        for (int i = 0; i < mapMaxWidth; i++)
            for (int j = 0; j < mapMaxHeight; j++)
            {
                map[i, j] = 0;
            }

        for (int i = 0; i < roomsValidated.Count; i++)
        {
            for (int x = 0; x < mapMaxWidth; x++)
                for (int y = 0; y < mapMaxHeight; y++)
                {
                    if (roomsValidated[i].GetPos().x == x && roomsValidated[i].GetPos().y == y)
                        map[x, y] = (int)roomsValidated[i].GetStyle();
                }
        }

        return map;
    }

    protected Room GenerateSquareRoom()
    {
        PreGenSquareRoom room = new PreGenSquareRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        int roomWidth = Random.Range(6, 16);
        int roomHeight = Random.Range(6, 16);

        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        int roomX = roomCenterPos.x - roomWidth / 2;
        int roomY = roomCenterPos.y - roomHeight / 2;
        for (int x = roomX; x < roomX + roomWidth; x++)
            for (int y = roomY; y < roomY + roomHeight; y++)
                tilePositions.Add(new Vector2Int(x, y));
        room.SetTilesPos(tilePositions);

        room.SetStyle(Styles.Style1);
        return room;
    }
    //Создаем круглую комнату
    protected Room GenerateCircleRoom()
    {
        Room room = new Room();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        int roomRadius = Random.Range(4, 8);

        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();

        int x = 0, y = roomRadius, f = 1 - roomRadius, incrE = 3, incrSE = 5 - 2 * roomRadius;
        tilePositions.Add(new Vector2Int(roomCenterPos.x, roomCenterPos.y + roomRadius));
        tilePositions.Add(new Vector2Int(roomCenterPos.x + x, roomCenterPos.y - roomRadius));
        for (int i = roomCenterPos.x - roomRadius; i <= roomCenterPos.x + roomRadius; i++)
            tilePositions.Add(new Vector2Int(i, roomCenterPos.y));
        while (x <= y)
        {
            if (f > 0)
            {
                y--;
                f += incrSE;
                incrSE += 4;
            }
            else
            {
                f += incrE;
                incrSE += 2;
            }
            incrE += 2;
            x++;
            for (int i = roomCenterPos.x - x; i <= roomCenterPos.x + x; i++)
            {
                tilePositions.Add(new Vector2Int(i, roomCenterPos.y + y));
            }
            for (int i = roomCenterPos.x - x; i <= roomCenterPos.x + x; i++)
            {
                tilePositions.Add(new Vector2Int(i, roomCenterPos.y - y));
            }
            for (int i = roomCenterPos.x - y; i <= roomCenterPos.x + y; i++)
            {
                tilePositions.Add(new Vector2Int(i, roomCenterPos.y + x));
            }
            for (int i = roomCenterPos.x - y; i <= roomCenterPos.x + y; i++)
            {
                tilePositions.Add(new Vector2Int(i, roomCenterPos.y - x));
            }

        }
        room.SetTilesPos(tilePositions);

        room.SetStyle(Styles.Style1); //Temp Shit just let it be there
        return room;
    }
    //Функция выбора случайной точки на окружности (если нужно будет мат обоснование то я найду и переведу то откуда спер)
    private Vector2Int CalculateRoomPos()
    {
        float t = 2 * Mathf.PI * Random.Range(0f, 0.9f);
        float u = Random.Range(0f, 0.9f) + Random.Range(0f, 0.9f);
        float r;
        if (u > 1)
            r = 2 - u;
        else
            r = u;
        return new Vector2Int(Mathf.RoundToInt(radius * r * Mathf.Cos(t)) + radius, Mathf.RoundToInt(radius * r * Mathf.Sin(t)) + radius);
    }

    private Room GenerateRandomRoom()
    {
        Room room = null;
        switch (Random.Range(0, 2))
        {
            case 0:
                room = GenerateSquareRoom();
                break;
            case 1:
                room = GenerateCircleRoom();
                break;

        }
        return room;
    }

}