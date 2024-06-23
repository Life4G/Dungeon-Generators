using Assets.Scripts.Fraction;
using Assets.Scripts.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Room
{
    /// <summary>
    /// Менеджер комнат подземелья.
    /// </summary>
    public class DungeonRoomManager : MonoBehaviour
    {
        /// <summary>
        /// Менеджер фракций.
        /// </summary>
        [SerializeField]
        private FractionManager fractionManager;

        /// <summary>
        /// Сид для генератора случайных чисел.
        /// </summary>
        private int seed;

        /// <summary>
        /// Массив комнат подземелья.
        /// </summary>
        public DungeonRoom[] rooms;

        /// <summary>
        /// Граф комнат подземелья.
        /// </summary>
        private int[,] graph;

        /// <summary>
        /// Методы распределения фракций по комнатам.
        /// </summary>
        public enum DistributionMethod
        {
            Sequential,
            Random,
            SequentiallyGraphBased, // последовательное
            ParallelGraphBased,      // параллельное
            CSP_Base,
            CSP_MOD
        }

        /// <summary>
        /// Метод распределения фракций.
        /// </summary>
        [SerializeField]
        private DistributionMethod distributionMethod;

        /// <summary>
        /// Генерирует случайное число, которое используется в качестве сида.
        /// </summary>
        /// <returns>Случайное число.</returns>
        public int GenerateSeed()
        {
            string text = "";
            for (int i = 0; i < 10; i++)
            {
                text += "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789"[UnityEngine.Random.Range(0, "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ0123456789".Length)].ToString();
            }
            return ((text == "") ? 0 : text.GetHashCode());
        }

        public int[,] GetGraph()
        {
            return graph;
        }    

        /// <summary>
        /// Позволяет установить сид.
        /// </summary>
        /// <param name="seed"></param>
        public void SetSeed(int seed)
        {
            this.seed = seed;
        }

        /// <summary>
        /// Вызов распределения фракций в зависимости от выбранного метода в distributionMethod.
        /// </summary>
        public void AssignFractions()
        {
            Random.State state = Random.state;

            Random.InitState(GenerateSeed());
            switch (distributionMethod)
            {
                case DistributionMethod.Sequential:
                    AssignFractionsToRoomsSequentially();
                    break;
                case DistributionMethod.Random:
                    AssignFractionsToRoomsRandomly();
                    break;
                case DistributionMethod.SequentiallyGraphBased:
                    AssignFractionsToRoomsSequentiallyGraphBased(graph);
                    break;
                case DistributionMethod.ParallelGraphBased:
                    AssignFractionsToRoomsParallelGraphBased(graph);
                    break;
                case DistributionMethod.CSP_Base:
                    CSP(graph);
                    break;
                case DistributionMethod.CSP_MOD:
                    CSP_MOD(graph);
                    break;
            }
            Random.state = state;
        }

        /// <summary>
        /// Вызов распределения фракций в зависимости от выбранного метода в distributionMethod.
        /// </summary>
        public void AssignFractions(int seed)
        {
            Random.State state = Random.state;

            Random.InitState(seed);
            switch (distributionMethod)
            {
                case DistributionMethod.Sequential:
                    AssignFractionsToRoomsSequentially();
                    break;
                case DistributionMethod.Random:
                    AssignFractionsToRoomsRandomly();
                    break;
                case DistributionMethod.SequentiallyGraphBased:
                    AssignFractionsToRoomsSequentiallyGraphBased(graph);
                    break;
                case DistributionMethod.ParallelGraphBased:
                    AssignFractionsToRoomsParallelGraphBased(graph);
                    break;
                case DistributionMethod.CSP_Base:
                    CSP(graph);
                    break;
                case DistributionMethod.CSP_MOD:
                    CSP_MOD(graph);
                    break;
            }
            Random.state = state;
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
                        int roomId = floorMap[y, x];
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

            // Уже не случайные цвета
            Color[] factionColors = new Color[fractionManager.fractions.Count];
            for (int i = 0; i < fractionManager.fractions.Count; i++)
            {
                //factionColors[i] = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
                factionColors[i] = fractionManager.fractions[i].color;
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

        /// <summary>
        /// Инициализация графа коридоров.
        /// </summary>
        /// <param name="corridorsGraph">Граф коридоров.</param>
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

            // очистка прошлых присваиваний
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
                    int randomIndex = Random.Range(0, availableRoomIndexes.Count);
                    int roomIndex = availableRoomIndexes[randomIndex];
                    availableRoomIndexes.RemoveAt(randomIndex);

                    rooms[roomIndex].fractionIndex = fractionIndex;
                }
            }
        }

        /// <summary>
        /// Параллельное присваивание комнатам, не являющимся коридорами, индексы фракций на основе их коэффициентов и графа соединений.
        /// </summary>
        /// <param name="connections">Граф соединений комнат коридорами.</param>
        public void AssignFractionsToRoomsParallelGraphBased(int[,] connections)
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
            List<int> availableRooms = rooms.Where(room => !room.isCorridor).Select(room => room.id).ToList();
            Dictionary<int, int> roomToFaction = new Dictionary<int, int>();
            List<int> activeFactions = fractionManager.fractions.Select((f, idx) => idx).ToList();
            Dictionary<int, int> roomsTarget = fractionManager.CalculateRoomsForAllFractions(totalRooms);


            // логи - старт
            Debug.Log($"Total rooms: {availableRooms.Count}");
            foreach (var target in roomsTarget)
            {
                Debug.Log($"Fraction {fractionManager.fractions[target.Key].name} needs {target.Value} rooms");
            }

            // по одной комнате на фракцию
            foreach (var factionIndex in activeFactions.ToList())
            {
                if (availableRooms.Count == 0) break;
                int roomIndex = Random.Range(0, availableRooms.Count);
                int roomId = availableRooms[roomIndex];
                roomToFaction[roomId] = factionIndex;
                availableRooms.RemoveAt(roomIndex);
                Debug.Log($"Fraction {fractionManager.fractions[factionIndex].name} starts with room {roomId}");
            }

            bool addedAnyRoom;
            do
            {
                addedAnyRoom = false;
                List<int> nextRoundActiveFactions = new List<int>();

                foreach (var factionIndex in activeFactions)
                {
                    var ownedRooms = roomToFaction.Where(pair => pair.Value == factionIndex).Select(pair => pair.Key).ToList();
                    List<int> expandableRooms = new List<int>();
                    bool foundExpandable = false;

                    // найти смежные свободые комнаты
                    foreach (var ownedRoomId in ownedRooms)
                    {
                        for (int i = 0; i < rooms.Length; i++)
                        {
                            if (ownedRoomId < connections.GetLength(0) && i < connections.GetLength(1) && connections[ownedRoomId, i] >= 0 && availableRooms.Contains(i) && !roomToFaction.ContainsKey(i))
                            {
                                expandableRooms.Add(i);
                                foundExpandable = true;
                            }
                        }
                    }

                    if (expandableRooms.Count > 0)
                    {
                        int newRoomId = expandableRooms[Random.Range(0, expandableRooms.Count)];
                        roomToFaction[newRoomId] = factionIndex;
                        availableRooms.Remove(newRoomId);
                        addedAnyRoom = true;
                    }
                    else if (!foundExpandable && availableRooms.Count > 0) // если не найдено свободных соседей, назначить случайную комнату
                    {
                        int randomRoomId = availableRooms[Random.Range(0, availableRooms.Count)];
                        roomToFaction[randomRoomId] = factionIndex;
                        availableRooms.Remove(randomRoomId);
                        addedAnyRoom = true;
                    }

                    int currentCount = roomToFaction.Count(pair => pair.Value == factionIndex);

                    if (currentCount < roomsTarget[factionIndex])
                    {
                        nextRoundActiveFactions.Add(factionIndex);
                    }
                }

                activeFactions = nextRoundActiveFactions;
            } while (addedAnyRoom && availableRooms.Count > 0);

            // логи - финиш
            foreach (var faction in fractionManager.fractions.Select((f, idx) => idx))
            {
                int roomCount = roomToFaction.Count(pair => pair.Value == faction);
                Debug.Log($"Fraction {fractionManager.fractions[faction].name} has {roomCount} rooms");
            }

            // применить распределение к комнатам
            foreach (var room in rooms)
            {
                if (roomToFaction.ContainsKey(room.id))
                {
                    room.fractionIndex = roomToFaction[room.id];
                }
            }
        }

        /// <summary>
        /// Последовательное присваивание комнатам, не являющимся коридорами, индексы фракций на основе их коэффициентов и графа соединений.
        /// </summary>
        /// <param name="connections">Граф соединений комнат коридорами.</param>
        public void AssignFractionsToRoomsSequentiallyGraphBased(int[,] connections)
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
            List<int> availableRooms = rooms.Where(room => !room.isCorridor).Select(room => room.id).ToList();
            Dictionary<int, int> roomToFaction = new Dictionary<int, int>();
            Dictionary<int, int> roomsTarget = fractionManager.CalculateRoomsForAllFractions(totalRooms);


            // логи - старт
            Debug.Log($"Total rooms: {availableRooms.Count}");
            foreach (var target in roomsTarget)
            {
                Debug.Log($"Fraction {fractionManager.fractions[target.Key].name} needs {target.Value} rooms");
            }

            foreach (var factionIndex in fractionManager.fractions.Select((f, idx) => idx))
            {
                if (availableRooms.Count == 0) break;

                // стартовая комната
                int startRoomIndex = Random.Range(0, availableRooms.Count);
                int startRoomId = availableRooms[startRoomIndex];
                roomToFaction[startRoomId] = factionIndex;
                availableRooms.RemoveAt(startRoomIndex);
                Debug.Log($"Fraction {fractionManager.fractions[factionIndex].name} starts with room {startRoomId}");

                // расширение
                bool addedRoom;
                do
                {
                    addedRoom = false;
                    var ownedRooms = roomToFaction.Where(pair => pair.Value == factionIndex).Select(pair => pair.Key).ToList();

                    foreach (var ownedRoomId in ownedRooms)
                    {
                        List<int> expandableRooms = new List<int>();

                        // найти смежные свободые комнаты
                        for (int i = 0; i < rooms.Length; i++)
                        {
                            if (ownedRoomId < connections.GetLength(0) && i < connections.GetLength(1) && connections[ownedRoomId, i] >= 0 && availableRooms.Contains(i) && !roomToFaction.ContainsKey(i))
                            {
                                expandableRooms.Add(i);
                            }
                        }

                        // если найдена
                        if (expandableRooms.Count > 0 && roomToFaction.Count(pair => pair.Value == factionIndex) < roomsTarget[factionIndex])
                        {
                            int newRoomId = expandableRooms[Random.Range(0, expandableRooms.Count)];
                            roomToFaction[newRoomId] = factionIndex;
                            availableRooms.Remove(newRoomId);
                            addedRoom = true;
                        }
                    }

                    // если не найдена - присвоить случайную
                    if (!addedRoom && availableRooms.Count > 0 && roomToFaction.Count(pair => pair.Value == factionIndex) < roomsTarget[factionIndex])
                    {
                        int randomRoomId = availableRooms[Random.Range(0, availableRooms.Count)];
                        roomToFaction[randomRoomId] = factionIndex;
                        availableRooms.Remove(randomRoomId);
                        addedRoom = true;
                    }
                } while (addedRoom && roomToFaction.Count(pair => pair.Value == factionIndex) < roomsTarget[factionIndex]);
            }

            // логи - финиш
            foreach (var faction in fractionManager.fractions.Select((f, idx) => idx))
            {
                int roomCount = roomToFaction.Count(pair => pair.Value == faction);
                Debug.Log($"Fraction {fractionManager.fractions[faction].name} has {roomCount} rooms");
            }

            // применить распределение к комнатам
            foreach (var room in rooms)
            {
                if (roomToFaction.ContainsKey(room.id))
                {
                    room.fractionIndex = roomToFaction[room.id];
                }
            }
        }

        /// <summary>
        /// Возвращает цвет фракции комнаты по ее идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор комнаты.</param>
        /// <returns>Цвет фракции.</returns>
        public Color GetRoomFractionColor(int id)
        {
            return fractionManager.GetColorByIndex(rooms[id].fractionIndex);
        }

        /// <summary>
        /// Распределяет фракции с помощью CSP
        /// </summary>
        /// <param name="connections">Граф соединений комнат коридорами</param>
        public void CSP(int[,] connections)
        {
            Dictionary<DungeonRoom, Fraction.Fraction> AllPairs = new Dictionary<DungeonRoom, Fraction.Fraction>();
            int RealRoomNumber = CountNonCorridorRooms();
            AllPairs = SetFactions(AllPairs, connections, RealRoomNumber);
            if (AllPairs == null)
                Debug.Log("Невозможно разместить фракции методом CSP");
            else
            {
                for (int i = 0; i < rooms.Count(); i++)
                    foreach (var pair in AllPairs)
                        if (rooms[i].id == pair.Key.id)
                            rooms[i].fractionIndex = fractionManager.GetFractionId(pair.Value);
            }
        }

        /// <summary>
        /// Алгоритм рекурсивного распределение фракций через CSP
        /// </summary>
        /// <param name="AllPairs">Словарь содержащий пару комната - фракция</param>
        /// <param name="connections">Граф соединений комнат коридорами</param>
        /// <returns></returns>
        public Dictionary<DungeonRoom, Fraction.Fraction> SetFactions(Dictionary<DungeonRoom, Fraction.Fraction> AllPairs, int[,] connections, int RealRoomNumber)
        {

            if (AllPairs.Count == RealRoomNumber)
                return AllPairs;

            DungeonRoom R = null;
            for (int i = 0; i < rooms.Count() && R == null; i++)
                if (!rooms[i].isCorridor && !AllPairs.ContainsKey(rooms[i]))
                    R = rooms[i];

            foreach (Fraction.Fraction F in fractionManager.fractions)
                if (CheckRules(R, F, AllPairs, connections))
                {
                    AllPairs.Add(R, F);
                    Dictionary<DungeonRoom, Fraction.Fraction> Result = SetFactions(AllPairs, connections, RealRoomNumber);
                    if (Result != null)
                        return Result;
                    else
                        AllPairs.Remove(R);
                }
            return null;
        }

        public void CSP_MOD(int[,] connections)
        {
            Dictionary<DungeonRoom, Fraction.Fraction> AllPairs = new Dictionary<DungeonRoom, Fraction.Fraction>();
            Dictionary<DungeonRoom, List<Fraction.Fraction>> PossibleFactions = new Dictionary<DungeonRoom, List<Fraction.Fraction>>();
            foreach (var room in rooms)
            {
                if (!room.isCorridor)
                {
                    PossibleFactions.Add(room, (fractionManager.fractions.ToList()));
                }
            }
            int RealRoomNumber = CountNonCorridorRooms();
            AllPairs = SetFactionsMod(AllPairs, PossibleFactions, connections, RealRoomNumber);
            if (AllPairs == null)
                Debug.Log("Невозможно разместить фракции модифицированным методом CSP");
            else
            {
                for (int i = 0; i < rooms.Count(); i++)
                    foreach (var pair in AllPairs)
                        if (rooms[i].id == pair.Key.id)
                            rooms[i].fractionIndex = fractionManager.GetFractionId(pair.Value);
            }
        }

        public Dictionary<DungeonRoom, List<Fraction.Fraction>> GetNewPossibileFactions(Dictionary<DungeonRoom, Fraction.Fraction> AllPairs, Dictionary<DungeonRoom, List<Fraction.Fraction>> PossibleFactions, int[,] connections)
        {
            Dictionary<DungeonRoom, List<Fraction.Fraction>> NewPossibleFactions = new Dictionary<DungeonRoom, List<Fraction.Fraction>>();
            foreach (var PF in PossibleFactions)
            {
                DungeonRoom R = PF.Key;
                List<Fraction.Fraction> Factions = new List<Fraction.Fraction>();
                foreach (Fraction.Fraction F in fractionManager.fractions)
                {
                    if (CheckRules(R, F, AllPairs, connections))
                        Factions.Add(F);
                }
                if (Factions.Count == 0)
                    return null;
                else
                    NewPossibleFactions.Add(R, Factions);
            }
            return NewPossibleFactions;
        }

        public Dictionary<DungeonRoom, Fraction.Fraction> SetFactionsMod(Dictionary<DungeonRoom, Fraction.Fraction> AllPairs, Dictionary<DungeonRoom, List<Fraction.Fraction>> PossibleFactions, int[,] connections, int RealRoomNumber)
        {
            if (AllPairs.Count == RealRoomNumber)
                return AllPairs;

            int min = int.MaxValue;
            foreach (var pair in PossibleFactions)
                if (!AllPairs.ContainsKey(pair.Key) && pair.Value.Count < min)
                    min = pair.Value.Count;

            List<DungeonRoom> Rooms = new List<DungeonRoom>();
            foreach (var pair in PossibleFactions)
                if (!AllPairs.ContainsKey(pair.Key) && pair.Value.Count == min)
                    Rooms.Add(pair.Key);

            DungeonRoom R = Rooms[Random.Range(0, Rooms.Count)];
            List<Fraction.Fraction> Factions;
            PossibleFactions.TryGetValue(R, out Factions);

            //Случайное перемешивание фракций
            int n = Factions.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                Fraction.Fraction value = Factions[k];
                Factions[k] = Factions[n];
                Factions[n] = value;
            }

            foreach (var F in Factions)
            {
                AllPairs.Add(R, F);
                var NewPF = GetNewPossibileFactions(AllPairs, PossibleFactions, connections);
                if (NewPF == null)
                {
                    AllPairs.Remove(R);
                    continue;
                }
                var Result = SetFactionsMod(AllPairs, NewPF, connections, RealRoomNumber);
                if (Result != null)
                    return Result;
                else
                    AllPairs.Remove(R);
            }
            return null;
        }

        /// <summary>
        /// Проверка комнаты на соответствие правилам
        /// </summary>
        /// <param name="room"> Комната</param>
        /// <param name="fraction">Фракция</param>
        /// <param name="AllPairs">Словарь содержащий пару комната - фракция</param>
        /// <param name="connections">Граф соединений комнат коридорами</param>
        /// <returns>Если комната соответсвует хотя бы одному правилу то возвращается true</returns>
        private bool CheckRules(DungeonRoom room, Fraction.Fraction fraction, Dictionary<DungeonRoom, Fraction.Fraction> AllPairs, int[,] connections)
        {
            return (SingleFractionCheck(room, fraction, AllPairs) || HasFractionNeighbour(room, fraction, AllPairs, connections));
            //&& FractionCountInPairs(fraction, AllPairs) < fractionManager.CalculateRoomsForFraction(CountNonCorridorRooms(), fractionManager.GetFractionId(fraction));

        }
        /// <summary>
        /// Проверка фракции на принадлежность только одной комнате
        /// </summary>
        /// <param name="room"></param>
        /// <param name="fraction"></param>
        /// <param name="AllPairs"></param>
        /// <returns>Если фракция принадлежит только данной комнате вернуть true</returns>
        private bool SingleFractionCheck(DungeonRoom room, Fraction.Fraction fraction, Dictionary<DungeonRoom, Fraction.Fraction> AllPairs)
        {
            foreach (var pair in AllPairs)
            {
                if (room != pair.Key && fraction == pair.Value)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Проверка на наличие соседних комнат с такой же фракцией
        /// </summary>
        /// <param name="room"> Комната</param>
        /// <param name="fraction">Фракция</param>
        /// <param name="AllPairs">Словарь содержащий пару комната - фракция</param>
        /// <param name="connections">Граф соединений комнат коридорами</param>
        /// <returns>Если у комнаты есть соседи с такой же фракцией вернуть true</returns>
        private bool HasFractionNeighbour(DungeonRoom room, Fraction.Fraction fraction, Dictionary<DungeonRoom, Fraction.Fraction> AllPairs, int[,] connections)
        {
            foreach (var pair in AllPairs)
                if (room != pair.Key && connections[room.id, pair.Key.id] >= 0 && fraction == pair.Value)
                {
                    return true;
                }
            return false;
        }

        private int FractionCountInPairs(Fraction.Fraction fraction, Dictionary<DungeonRoom, Fraction.Fraction> AllPairs)
        {
            int counter = 0;
            foreach (var pair in AllPairs)
                if (pair.Value == fraction)
                    counter++;
            return counter;
        }
    }
}
