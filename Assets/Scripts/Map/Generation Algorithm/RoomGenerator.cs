using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : GeneratorBase
{
    //Радиус для выбора рандомной точки на окружности
    [SerializeField]
    private int radius = 50;
    //Мин и макс комнат которое может быть нагенерено
    [SerializeField]
    private int roomNumberMin = 10;
    [SerializeField]
    private int roomNumberMax = 32;

    private List<RoomBase> rooms;
    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        return Run();
    }
    protected HashSet<Vector2Int> Run()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        rooms = new List<RoomBase>();
        //Генерируем несколько комнат разных форм
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    rooms.Add(GenerateCircleRoom());
                    break;
                case 1:
                    rooms.Add(GenerateSquareRoom());
                    break;
            }
            
        }
        //Проводим операции над комнатами (гугли теорию по операциям на множестве если что)
        for (int i = 0; i < rooms.Count - 1; i++)
            for (int j = i + 1; j < rooms.Count; j++)
            {
                //Проверяем пересечение всех комнат
                if (rooms[i].CheckIntersection(rooms[j]))
                {
                    //Если комната полностью входит одна в другую
                    if (rooms[i].IsProperSubsetOf(rooms[j]))
                    {
                        //То применяем либо объединение либо разность т.к. другие операции не имеют смысла
                        switch (Random.Range(0, 2))
                            {
                                case 0:
                                    rooms[i].Union(rooms[j]);
                                    break;
                                case 1:
                                    rooms[i].Difference(rooms[j]);
                                    break;
                            }
                        
                        rooms.Remove(rooms[j]); // Выкидываем вторую комнату с которой уже провели оперцию
                                                //Здесь должна быть проверка на валидацию но когда-то она там появится
                    }
                    else
                    {
                        //Если нет то выбираем уже любую операцию из всех
                        switch (Random.Range(0, 4))
                            {
                                case 0:
                                    rooms[i].Intersect(rooms[j]);
                                    break;
                                case 1:
                                    rooms[i].Union(rooms[j]);
                                    break;
                                case 2:
                                    rooms[i].Difference(rooms[j]);
                                    break;
                                case 3:
                                    rooms[i].SymmetricDifference(rooms[j]);
                                    break;
                            }

                        rooms.Remove(rooms[j]); // Выкидываем вторую комнату с которой уже провели оперцию
                                                //Здесь должна быть проверка на валидацию но когда-то она там появится
                    }
                }
            }
        //Объединяем все комнаты в единый хешсет
        for (int i = 0; i < rooms.Count; i++)
        {
            floorPositions.UnionWith(rooms[i].GetTilesPos());
        }
        return floorPositions;
    }

    //Создаем квадратную комнату
    protected SquareRoom GenerateSquareRoom()
    {
        //Задаем случайную позицию
        SquareRoom room = new SquareRoom();
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
    protected CircleRoom GenerateCircleRoom()
    {
        //Задаем случайную позицию
        CircleRoom room = new CircleRoom();
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
}