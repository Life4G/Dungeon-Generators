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

        public DungeonRoomManager(int[,] floorMap)
        {
            Dictionary<int, List<Vector2Int>> roomTiles = new Dictionary<int, List<Vector2Int>>();

            // перебор массива / сбор плиток для каждой комнаты
            for (int y = 0; y < floorMap.GetLength(1); y++)
            {
                for (int x = 0; x < floorMap.GetLength(0); x++)
                {
                    int roomId = floorMap[x, y];
                    if (roomId >= 0)
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

        // рандомные индексы стилей имеющимся комнатам
        // автонейминг комнат
        public void AssignRandomStylesToRooms(RoomStyleManager roomStyleManager)
        {
            foreach (var room in rooms)
            {

                room.styleId = roomStyleManager.GetRandomStyleIndex();
                string styleName = roomStyleManager.GetStyleNameByIndex(room.styleId);
                room.name = $"{styleName}_Room_{room.id}";
            }
        }

        // вывести информацию о всех комнатах в консоль
        public void PrintRoomsInfo()
        {
            foreach (DungeonRoom room in rooms)
            {
                Debug.Log($"Room ID: {room.id}, Name: {room.name}, Size: {room.size}, Width: {room.width}, Height: {room.height}, Style ID: {room.styleId}");
            }
        }

        // вернуть id стиля комнаты
        public int GetRoomStyleId(int roomId)
        {
            foreach (var room in rooms)
            {
                if (room.id == roomId)
                {
                    return room.styleId;
                }
            }
            return -1;
        }

    }
}
