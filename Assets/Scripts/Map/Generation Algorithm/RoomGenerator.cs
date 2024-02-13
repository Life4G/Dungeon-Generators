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
    //Радиус для выбора рандомной точки на окружности

    [SerializeField]
    private int radius = 120;
    //Мин и макс комнат которое может быть нагенерено
    [SerializeField]
    private int roomNumberMin = 16;
    [SerializeField]
    private int roomNumberMax = 64;
    [SerializeField]
    bool checkConnection = true;
    [SerializeField]
    bool presetLoad = false;
    public static int mapMaxHeight = 512;
    public static int mapMaxWidth = 512;
    private int[,] map;

    private List<MassRoom> roomsGenerated;
    public List<MassRoom> roomsValidated;

    protected override int[,] GenerateDungeon()
    {
        if(presetLoad)
        {
            string fileContents = File.ReadAllText(Application.persistentDataPath + "/gamedata.json");
            map = JsonUtility.FromJson<int[,]>(fileContents);
            return map;
        }
        return Run();
    }

    protected int[,] Run()
    {
        map = new int[mapMaxWidth, mapMaxHeight];
        roomsValidated = new List<MassRoom>();
        roomsGenerated = new List<MassRoom>();
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
            List<MassRoom> roomsValidatedNew = new List<MassRoom>();
            MassRoom room = null;
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
            if (roomsValidated.Count != 0)
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
<<<<<<< Updated upstream
                                if (operation == SetOperations.Operations.None)
=======

                                if (operation == SetOperations.Operations.None && roomsValidated.Count + 1 < roomNumber)
>>>>>>> Stashed changes
                                {
                                    roomsGenerated.Add(GenerateRandomRoom());
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
                        if (checkConnection && room.CheckConnection(roomsValidated[i]))
                        {
                            room.Union(roomsValidated[i]);
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

        } while (roomsGenerated.Count > 0 || !roomNotIntersect);

        for (int i = 0; i < mapMaxWidth; i++)
            for (int j = 0; j < mapMaxHeight; j++)
            {
                map[i, j] = -1;
            }

        for (int i = 0; i < roomsValidated.Count; i++)
        {
            int[,] roomTiles = roomsValidated[i].GetTiles();
            Vector2Int roomPos = roomsValidated[i].GetPos();
            Size roomSize = roomsValidated[i].GetSize();

            for (int y = 0; y < roomSize.Height && y + roomPos.y < mapMaxHeight; y++)
                for (int x = 0; x < roomSize.Width && x + roomPos.x < mapMaxWidth; x++)
                {
                    if (roomTiles[y, x] != 0)
                        map[y + roomPos.y, x + roomPos.x] = i;
                }
        }
        return map;
    }

    protected PreGenSquareRoom GenerateSquareRoom()
    {
        PreGenSquareRoom room = new PreGenSquareRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        int roomWidth = Random.Range(6, 16);
        int roomHeight = Random.Range(6, 16);

        int[,] tilePositions = new int[roomHeight, roomWidth];

        int tilesNum = 0;
        for (int y = 0; y < roomHeight; y++)
            for (int x = 0; x < roomWidth; x++)
            {
                tilePositions[y, x] = 1;
                tilesNum++;
            }
        room.SetTiles(tilePositions);
        room.SetSize(roomWidth, roomHeight);
        room.SetStyle(Styles.Style1);
        return room;
    }
    //Создаем круглую комнату
    protected PreGenCircleRoom GenerateCircleRoom()
    {
        PreGenCircleRoom room = new PreGenCircleRoom();
        room.SetPos(CalculateRoomPos());

        int roomRadius = Random.Range(4, 8);

        int[,] tilePositions = new int[roomRadius * 2 + 1, roomRadius * 2 + 1];
        int x = 0, y = roomRadius, f = 1 - roomRadius, incrE = 3, incrSE = 5 - 2 * roomRadius;
        //tilePositions.Add(new Vector2Int(roomCenterPos.x, roomCenterPos.y + roomRadius));
        //tilePositions.Add(new Vector2Int(roomCenterPos.x + x, roomCenterPos.y - roomRadius));
        tilePositions[roomRadius, roomRadius * 2 - 1] = 1;
        tilePositions[roomRadius * 2, 0] = 1;

        for (int i = 0; i <= roomRadius * 2; i++)
        {
            //tilePositions.Add(new Vector2Int(i, roomCenterPos.y));
            tilePositions[i, roomRadius] = 1;
        }
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
            for (int i = roomRadius - x; i <= roomRadius + x; i++)
            {
                //tilePositions.Add(new Vector2Int(i, roomCenterPos.y + y));
                tilePositions[i, roomRadius + y] = 1;
            }
            for (int i = roomRadius - x; i <= roomRadius + x; i++)
            {
                //tilePositions.Add(new Vector2Int(i, roomCenterPos.y - y));
                tilePositions[i, roomRadius - y] = 1;
            }
            for (int i = roomRadius - y; i <= roomRadius + y; i++)
            {
                //tilePositions.Add(new Vector2Int(i, roomCenterPos.y + x));
                tilePositions[i, roomRadius + x] = 1;
            }
            for (int i = roomRadius - y; i <= roomRadius + y; i++)
            {
                //tilePositions.Add(new Vector2Int(i, roomCenterPos.y - x));
                tilePositions[i, roomRadius - x] = 1;
            }

        }
        room.SetSize(roomRadius * 2);
        room.SetTiles(tilePositions);
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

    private MassRoom GenerateRandomRoom()
    {
        MassRoom room = null;
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

    public override int GetRoomStyle(int id)
    {
        return ((int)roomsValidated[id].GetStyle());
    }

}