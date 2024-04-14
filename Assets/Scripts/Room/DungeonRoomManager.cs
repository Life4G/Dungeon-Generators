using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Room
{
    public class DungeonRoomManager
    {
        public DungeonRoom[] rooms;

        // Не вычисляет центры
        /// <summary>
        /// Конструктор по массиву тайлов пола.
        /// </summary>
        /// <param name="floorMap">Массив тайлов пола.</param>
        public DungeonRoomManager(int[,] floorMap)
        {
            try
            {
                Dictionary<int, List<Vector2Int>> roomTiles = new Dictionary<int, List<Vector2Int>>();

                // перебор массива / сбор плиток для каждой комнаты
                for (int y = 0; y < floorMap.GetLength(1); y++)
                {
                    for (int x = 0; x < floorMap.GetLength(0); x++)
                    {
                        int roomId = floorMap[x, y];
                        if (roomId > -1)
                        {
                            if (!roomTiles.ContainsKey(roomId))
                            {
                                roomTiles[roomId] = new List<Vector2Int>();
                            }
                            roomTiles[roomId].Add(new Vector2Int(x, y));
                        }
                    }
                }

                // создание комнат на основе собранных данных
                List<DungeonRoom> createdRooms = new List<DungeonRoom>();
                foreach (var roomEntry in roomTiles)
                {
                    int id = roomEntry.Key;
                    var tiles = roomEntry.Value;
                    int size = tiles.Count;
                    int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
                    foreach (var tile in tiles)
                    {
                        minX = Mathf.Min(minX, tile.x);
                        maxX = Mathf.Max(maxX, tile.x);
                        minY = Mathf.Min(minY, tile.y);
                        maxY = Mathf.Max(maxY, tile.y);
                    }
                    int width = maxX - minX + 1;
                    int height = maxY - minY + 1;

                    DungeonRoom room = new DungeonRoom
                    {
                        id = id,
                        size = size,
                        width = width,
                        height = height
                    };
                    createdRooms.Add(room);
                }

                rooms = createdRooms.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка при инициализации DungeonRoomManager: " + e.Message);
            }
        }

        /// <summary>
        /// Конструктор по структуре карты.
        /// </summary>
        /// <param name="dungeonMap">Карта.</param>
        public DungeonRoomManager(DungeonMap dungeonMap)
        {
            try
            {
                Dictionary<int, List<Vector2Int>> roomTiles = new Dictionary<int, List<Vector2Int>>();

                for (int y = 0; y < dungeonMap.GetHeight(); y++)
                {
                    for (int x = 0; x < dungeonMap.GetWidth(); x++)
                    {
                        DungeonTile tile = dungeonMap.GetTile(x, y);
                        int roomId = tile.roomIndex;
                        if (roomId > -1)
                        {
                            if (!roomTiles.ContainsKey(roomId))
                            {
                                roomTiles[roomId] = new List<Vector2Int>();
                            }
                            roomTiles[roomId].Add(new Vector2Int(x, y));
                        }
                    }
                }

                // создание комнат на основе собранных данных
                List<DungeonRoom> createdRooms = new List<DungeonRoom>();
                foreach (var roomEntry in roomTiles)
                {
                    int id = roomEntry.Key;
                    var tiles = roomEntry.Value;
                    int size = tiles.Count;
                    int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
                    foreach (var tile in tiles)
                    {
                        minX = Mathf.Min(minX, tile.x);
                        maxX = Mathf.Max(maxX, tile.x);
                        minY = Mathf.Min(minY, tile.y);
                        maxY = Mathf.Max(maxY, tile.y);
                    }
                    int width = maxX - minX + 1;
                    int height = maxY - minY + 1;
                    float centerX = (minX + maxX) / 2.0f;
                    float centerY = (minY + maxY) / 2.0f;

                    DungeonRoom room = new DungeonRoom
                    {
                        id = id,
                        size = size,
                        width = width,
                        height = height,
                        centerX = centerX,
                        centerY = centerY
                    };
                    createdRooms.Add(room);
                }

                rooms = createdRooms.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка при инициализации DungeonRoomManager с DungeonMap: " + e.Message);
            }
        }

        /// <summary>
        /// Определяет рандомные индексы стилей имеющимся комнатам и дает им имена.
        /// </summary>
        /// <param name="roomStyleManager">Менеджер стилей коомнат.</param>
        public void AssignRandomStylesToRooms(RoomStyleManager roomStyleManager)
        {
            try
            {
                foreach (var room in rooms)
                {

                    room.styleId = roomStyleManager.GetRandomStyleIndex();
                    string styleName = roomStyleManager.GetStyleNameByIndex(room.styleId);
                    room.name = $"{styleName}_Room_{room.id}";
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка при назначении случайных стилей комнатам: " + e.Message);
            }
        }

        /// <summary>
        /// Выводит информацию о всех комнатах в консоль.
        /// </summary>
        public void PrintRoomsInfo()
        {
            foreach (DungeonRoom room in rooms)
            {
                Debug.Log($"Room ID: {room.id}, Name: {room.name}, Size: {room.size}, Width: {room.width}, Height: {room.height}, Style ID: {room.styleId}");
            }
        }

        /// <summary>
        /// Возвращает иденнтификатор стиля комнаты.
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты.</param>
        /// <returns></returns>
        public int GetRoomStyleId(int roomId)
        {
            if (rooms == null)
            {
                Debug.LogError("Массив комнат не инициализирован.");
                return -1;
            }
            foreach (var room in rooms)
            {
                if (room.id == roomId)
                {
                    return room.styleId;
                }
            }
            Debug.LogWarning($"Комната с id {roomId} не найдена.");
            return -1;
        }

        /// <summary>
        /// Отображает информацию о комнатах на карте.
        /// </summary>
        public void DisplayRoomsInfoOnMap()
        {
            GameObject textParent = new GameObject("RoomsInfo");

            foreach (DungeonRoom room in rooms)
            {
                GameObject textObj = new GameObject($"RoomInfo_{room.id}");
                textObj.transform.SetParent(textParent.transform);

                Vector3 textPosition = new Vector3(room.centerX, room.centerY, 0);
                textObj.transform.position = textPosition;

                TextMesh textMesh = textObj.AddComponent<TextMesh>();
                textMesh.text = $"ID: {room.id}\nSize: {room.size}\nWidth: {room.width}\nHeight: {room.height}";
                textMesh.characterSize = 0.1f;
                textMesh.anchor = TextAnchor.MiddleCenter;

                textMesh.fontSize = 100;
                textMesh.color = Color.black;
                textMesh.fontStyle = FontStyle.Bold;
            }

        }

        /// <summary>
        /// Очищает информацию о комнатах на карте.
        /// </summary>
        public void ClearRoomsInfoFromMap()
        {
            // поиск объекта "RoomsInfo" на сцене
            GameObject roomsInfoParent = GameObject.Find("RoomsInfo");

            // удалить с дочерними если найденн
            if (roomsInfoParent != null)
            {
                GameObject.DestroyImmediate(roomsInfoParent);
            }
        }


    }
}
