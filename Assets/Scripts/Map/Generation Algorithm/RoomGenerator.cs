using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : DungeonGeneratorBase
{
    //Радиус для выбора рандомной точки на окружности
    [SerializeField]
    private int radius = 70;
    //Мин и макс комнат которое может быть нагенерено
    [SerializeField]
    private int roomNumberMin = 16;
    [SerializeField]
    private int roomNumberMax = 64;

    public List<RoomBase> roomsGenerated;
    private List<RoomBase> roomsValidated;

    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        return Run();
    }
    protected HashSet<Vector2Int> Run()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        roomsGenerated = new List<RoomBase>();
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
            List<RoomBase> roomsValidatedNew = new List<RoomBase>();
            RoomBase room = null;
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

            if (roomsValidated != null)
                for (int i = 0; i < roomsValidated.Count; i++)
                {
                    if (room.CheckIntersection(roomsValidated[i]))
                    {
                        roomNotIntersect = false;
                        //Если комната полностью входит одна в другую
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
                        roomsValidatedNew.Add(roomsValidated[i]);
                    }
                    if (roomNotIntersect)
                        roomsValidatedNew.Add(room);
                }
            else
            {
                roomsValidatedNew.Add(room);
            }
            roomsValidated = roomsValidatedNew;

            if (roomsGenerated.Count < 0 && roomsValidated.Count < roomNumber / 2)
                for(int i = roomsValidated.Count; i <roomNumber; i++)
                    roomsGenerated.Add(GenerateRandomRoom());
            
        } while (roomsGenerated.Count > 0 && roomNotIntersect);
        //Объединяем все комнаты в единый хешсет
        for (int i = 0; i < roomsValidated.Count; i++)
        {
            floorPositions.UnionWith(roomsValidated[i].GetTilesPos());
        }
        return floorPositions;
    }

    //Создаем квадратную комнату
    protected PreGenSquareRoom GenerateSquareRoom()
    {
        //Задаем случайную позицию
        PreGenSquareRoom room = new PreGenSquareRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        //Рандомим размер
        int roomWidth = Random.Range(room.widthMin, room.widthMax);
        int roomHeight = Random.Range(room.heighthMin, room.heighthMax);
        room.SetSize(roomWidth, roomHeight);

        //Получаем координаты тайлов комнаты и запихиваем в хешсет
        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        int roomX = roomCenterPos.x - roomWidth / 2;
        int roomY = roomCenterPos.y - roomHeight / 2;
        for (int x = roomX; x < roomX + roomWidth; x++)
            for (int y = roomY; y < roomY + roomHeight; y++)
                tilePositions.Add(new Vector2Int(x, y));
        room.SetTilesPos(tilePositions);

        //Стиль комнаты задаем пока как есть (Потом ты допишишь стили и я буду рандомить это:D)
        room.SetStyle(Styles.Style1); //Temp Shit just let it be there
        return room;
    }
    //Создаем круглую комнату
    protected PreGenCircleRoom GenerateCircleRoom()
    {
        //Задаем случайную позицию
        PreGenCircleRoom room = new PreGenCircleRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        //Рандомим размер
        int roomRadius = Random.Range(room.radiusMin, room.radiusMax);
        room.SetSize(roomRadius);

        //Получаем координаты тайлов комнаты и запихиваем в хешсет
        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        for (int y = -roomRadius; y < roomRadius; y++)
        {
            int half_row_width = Mathf.RoundToInt(Mathf.Sqrt(roomRadius * roomRadius - y * y));
            for (int x = -half_row_width; x < half_row_width; x++)
                tilePositions.Add(new Vector2Int(roomCenterPos.x + x, roomCenterPos.y + y));
        }
        room.SetTilesPos(tilePositions);

        //Стиль комнаты задаем пока как есть (Потом ты допишишь стили и я буду рандомить это:D)
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
        return new Vector2Int(Mathf.RoundToInt(radius * r * Mathf.Cos(t)), Mathf.RoundToInt(radius * r * Mathf.Sin(t)));
    }

    //private bool CheckAllRoomsIntersection()
    //{
    //    List<RoomBase> rooms = roomsGenerated;
    //    if (rooms == null)
    //        return false;
    //    while (rooms.Count > 1)
    //    {
    //        RoomBase room = rooms.First();
    //        rooms.Remove(room);
    //        foreach (RoomBase roomOther in rooms)
    //        {
    //            if (room.CheckIntersection(roomOther))
    //                return true;
    //        }
    //    }
    //    return false;
    //}

    private RoomBase GenerateRandomRoom()
    {
        RoomBase room = null;
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