using Assets.Scripts.Fraction;
using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Room
{
    /// <summary>
    /// Менеджер комнат подземелья.
    /// </summary>
    public class DungeonRoomManager : MonoBehaviour
    {
        [SerializeField]
        private FractionManager fractionManager;

        public DungeonRoom[] rooms;

        private int[,] graph;

        public enum DistributionMethod
        {
            Sequential,
            Random,
            GraphBased
        }

        [SerializeField]
        private DistributionMethod distributionMethod;

        /// <summary>
        /// Вызов распределения фракций в зависимости от выбранного метода в distributionMethod.
        /// </summary>
        public void AssignFractions()
        {
            switch (distributionMethod)
            {
                case DistributionMethod.Sequential:
                    AssignFractionsToRoomsSequentially();
                    break;
                case DistributionMethod.Random:
                    AssignFractionsToRoomsRandomly();
                    break;
                case DistributionMethod.GraphBased:
                    AssignFractionsToRoomsGraphBased(graph);
                    break;
            }
        }

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
        public DungeonRoomManager(DungeonMap dungeonMap, int[,] corridorsGraph)
        {
            try
            {
                Dictionary<int, List<Vector2Int>> roomTiles = new Dictionary<int, List<Vector2Int>>();
                int roomsCount = corridorsGraph.GetLength(0);

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
                    bool isCorridor = true;
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

                    if (id < roomsCount) { isCorridor = false; }

                    DungeonRoom room = new DungeonRoom
                    {
                        id = id,
                        size = size,
                        width = width,
                        height = height,
                        centerX = centerX,
                        centerY = centerY,
                        isCorridor = isCorridor
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
        /// Инициализация по структуре карты.
        /// </summary>
        /// <param name="dungeonMap">Карта.</param>
        public void Initialize(DungeonMap dungeonMap, int[,] corridorsGraph)
        {
            try
            {
                InitializeGraph(corridorsGraph);

                Dictionary<int, List<Vector2Int>> roomTiles = new Dictionary<int, List<Vector2Int>>();
                int roomsCount = corridorsGraph.GetLength(0);

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
                    bool isCorridor = true;
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

                    if (id < roomsCount) { isCorridor = false; }

                    DungeonRoom room = new DungeonRoom
                    {
                        id = id,
                        size = size,
                        width = width,
                        height = height,
                        centerX = centerX,
                        centerY = centerY,
                        isCorridor = isCorridor
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
                Debug.Log($"Room ID: {room.id}, Name: {room.name}, Size: {room.size}, Width: {room.width}, Height: {room.height}, Style ID: {room.styleId}, isCorridor: {room.isCorridor}");
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
            // удалить старую инфу
            ClearRoomsInfoFromMap();

            GameObject textParent = new GameObject("RoomsInfo");

            // случайные цвета
            Color[] factionColors = new Color[fractionManager.fractions.Count];
            for (int i = 0; i < fractionManager.fractions.Count; i++)
            {
                factionColors[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            }

            foreach (DungeonRoom room in rooms)
            {
                GameObject textObj = new GameObject($"RoomInfo_{room.id}");
                textObj.transform.SetParent(textParent.transform);

                Vector3 textPosition = new Vector3(room.centerX, room.centerY, 0);
                textObj.transform.position = textPosition;

                TextMesh textMesh = textObj.AddComponent<TextMesh>();

                string roomType = room.isCorridor ? "corridor" : "room";
                string fractionName = room.fractionIndex >= 0 ? fractionManager.fractions[room.fractionIndex].name : "None";
                textMesh.text = $"ID: {room.id} ({roomType})\nFraction: {fractionName}\nSize: {room.size}\nWidth: {room.width}\nHeight: {room.height}";

                // цвет
                if (room.fractionIndex >= 0 && room.fractionIndex < factionColors.Length)
                {
                    textMesh.color = factionColors[room.fractionIndex];
                }
                else
                {
                    textMesh.color = Color.white; // без фракции или коридоры
                }

                textMesh.characterSize = 0.1f;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.fontSize = 100;
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

        /// <summary>
        /// Получение комнаты по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор комнаты.</param>
        /// <returns>Комната.</returns>
        public DungeonRoom GetRoomById(int id)
        {
            foreach (var room in rooms)
            {
                if (room.id == id)
                {
                    return room;
                }
            }

            Debug.LogWarning($"Room with ID {id} not found.");
            return null;
        }

        /// <summary>
        /// Подсчет комнат, не являющихся коридорами.
        /// </summary>
        /// <returns>Общее кол-во комнат.</returns>
        public int CountNonCorridorRooms()
        {
            int count = 0;
            foreach (DungeonRoom room in rooms)
            {
                if (!room.isCorridor)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Получение индексов всех комнат, не являющихся коридорами.
        /// </summary>
        /// <returns>Список индексов комнат.</returns>
        public List<int> GetNonCorridorRoomIndices()
        {
            List<int> nonCorridorRoomIndices = new List<int>();
            foreach (DungeonRoom room in rooms)
            {
                if (!room.isCorridor)
                {
                    nonCorridorRoomIndices.Add(room.id);
                }
            }
            return nonCorridorRoomIndices;
        }

        public void InitializeGraph(int[,] corridorsGraph)
        {
            if (corridorsGraph == null)
            {
                Debug.LogError("corridorsGraph null.");
                return;
            }

            int rows = corridorsGraph.GetLength(0);
            int cols = corridorsGraph.GetLength(1);

            graph = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    graph[i, j] = corridorsGraph[i, j];
                }
            }
        }


        /// <summary>
        /// Последовательное присваивание комнатам, не являющимся коридорами, индексы фракций на основе их коэффициентов.
        /// </summary>
        public void AssignFractionsToRoomsSequentially()
        {
            if (fractionManager.fractions.Count == 0)
            {
                Debug.LogError("Fractions not defined.");
                return;
            }

            // очиста прошлых присваиваний
            foreach (var room in rooms)
            {
                room.fractionIndex = -1; // -1 - отсутствие фракции
            }

            int totalRooms = CountNonCorridorRooms();
            List<int> nonCorridorRoomIndices = GetNonCorridorRoomIndices();
            List<int> roomAssignments = new List<int>();

            for (int i = 0; i < fractionManager.fractions.Count; i++)
            {
                int roomsForFraction = fractionManager.CalculateRoomsForFraction(totalRooms, i);
                for (int j = 0; j < roomsForFraction; j++)
                {
                    if (nonCorridorRoomIndices.Count > 0)
                    {
                        int roomIndex = nonCorridorRoomIndices[0];
                        nonCorridorRoomIndices.RemoveAt(0);
                        DungeonRoom room = GetRoomById(roomIndex);
                        if (room != null)
                        {
                            room.fractionIndex = i;
                            roomAssignments.Add(roomIndex);
                        }
                    }
                }
            }

            // если остались комнаты -> равномерно распределить их между фракциями
            int remainingRooms = nonCorridorRoomIndices.Count;
            int fractionIndex = 0;
            while (remainingRooms > 0)
            {
                if (fractionIndex >= fractionManager.fractions.Count)
                    fractionIndex = 0;

                int roomIndex = nonCorridorRoomIndices[0];
                nonCorridorRoomIndices.RemoveAt(0);
                DungeonRoom room = GetRoomById(roomIndex);
                if (room != null)
                {
                    room.fractionIndex = fractionIndex;
                }

                fractionIndex++;
                remainingRooms--;
            }
        }

        /// <summary>
        /// Случайное присваивание комнатам, не являющимся коридорами, индексы фракций на основе их коэффициентов.
        /// </summary>
        public void AssignFractionsToRoomsRandomly()
        {
            if (fractionManager.fractions.Count == 0)
            {
                Debug.LogError("Fractions not defined.");
                return;
            }

            // очиста прошлых присваиваний
            foreach (var room in rooms)
            {
                room.fractionIndex = -1; // -1 - отсутствие фракции
            }

            int totalRooms = rooms.Count(r => !r.isCorridor);
            Dictionary<int, int> fractionRoomCounts = new Dictionary<int, int>();

            // кол-ва комнат для фракций
            for (int i = 0; i < fractionManager.fractions.Count; i++)
            {
                fractionRoomCounts[i] = fractionManager.CalculateRoomsForFraction(totalRooms, i);
            }

            // список индексов доступных комнат
            List<int> availableRoomIndexes = rooms
                .Select((room, index) => new { Room = room, Index = index })
                .Where(x => !x.Room.isCorridor)
                .Select(x => x.Index)
                .ToList();

            System.Random random = new System.Random();

            foreach (var pair in fractionRoomCounts)
            {
                int fractionIndex = pair.Key;
                int roomsForFraction = pair.Value;

                for (int i = 0; i < roomsForFraction; i++)
                {
                    if (availableRoomIndexes.Count == 0)
                    {
                        Debug.LogError("Not enough rooms.");
                        return;
                    }

                    // случайный индекс из доступных комнат
                    int randomIndex = random.Next(availableRoomIndexes.Count);
                    int roomIndex = availableRoomIndexes[randomIndex];
                    availableRoomIndexes.RemoveAt(randomIndex);

                    rooms[roomIndex].fractionIndex = fractionIndex;
                }
            }
        }

        public void AssignFractionsToRoomsGraphBased(int[,] connections)
        {
            int totalRooms = CountNonCorridorRooms();
            List<int> availableRooms = rooms.Where(room => !room.isCorridor).Select(room => room.id).ToList();
            Dictionary<int, int> roomToFaction = new Dictionary<int, int>();
            List<int> activeFactions = fractionManager.fractions.Select((f, idx) => idx).ToList();
            Dictionary<int, int> roomsTarget = fractionManager.CalculateRoomsForAllFractions(totalRooms);
            System.Random random = new System.Random();

            // Логируем начальные цели по комнатам для каждой фракции
            foreach (var target in roomsTarget)
            {
                Debug.Log($"Фракция {fractionManager.fractions[target.Key].name} должна занять {target.Value} комнат.");
            }

            // Начальное распределение по одной комнате для каждой фракции
            foreach (var factionIndex in activeFactions)
            {
                if (availableRooms.Count == 0) break;
                int roomIndex = random.Next(availableRooms.Count);
                int roomId = availableRooms[roomIndex];
                roomToFaction[roomId] = factionIndex;
                availableRooms.RemoveAt(roomIndex);
            }

            // Основной цикл распределения
            bool addedAnyRoom;
            do
            {
                addedAnyRoom = false;
                List<int> nextRoundActiveFactions = new List<int>();

                foreach (var factionIndex in activeFactions)
                {
                    var ownedRooms = roomToFaction.Where(pair => pair.Value == factionIndex).Select(pair => pair.Key).ToList();
                    List<int> expandableRooms = new List<int>();

                    // Найти смежные комнаты, которые можно занять
                    foreach (var ownedRoomId in ownedRooms)
                    {
                        for (int i = 0; i < rooms.Length; i++)
                        {
                            if (ownedRoomId < connections.GetLength(0) && i < connections.GetLength(1) && connections[ownedRoomId, i] >= 0 && availableRooms.Contains(i) && !roomToFaction.ContainsKey(i))
                            {
                                expandableRooms.Add(i);
                            }
                        }
                    }

                    if (expandableRooms.Count > 0)
                    {
                        int newRoomId = expandableRooms[random.Next(expandableRooms.Count)];
                        roomToFaction[newRoomId] = factionIndex;
                        availableRooms.Remove(newRoomId);
                        addedAnyRoom = true;

                        int currentCount = roomToFaction.Count(pair => pair.Value == factionIndex);
                        if (currentCount < roomsTarget[factionIndex])
                        {
                            nextRoundActiveFactions.Add(factionIndex);
                        }
                    }
                }

                activeFactions = nextRoundActiveFactions;
            } while (addedAnyRoom && availableRooms.Count > 0 && activeFactions.Count > 0);

            // Логируем конечные результаты распределения
            foreach (var faction in fractionManager.fractions.Select((f, idx) => idx))
            {
                int roomCount = roomToFaction.Count(pair => pair.Value == faction);
                Debug.Log($"Фракция {fractionManager.fractions[faction].name} занимает {roomCount} комнат.");
            }

            // Применить распределение к комнатам
            foreach (var room in rooms)
            {
                if (roomToFaction.ContainsKey(room.id))
                {
                    room.fractionIndex = roomToFaction[room.id];
                }
            }
        }
    }
}
