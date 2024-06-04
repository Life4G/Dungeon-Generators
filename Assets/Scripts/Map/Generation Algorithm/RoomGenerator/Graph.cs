using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEditorInternal;
using Unity.VisualScripting;
using UnityEditor;

[CreateAssetMenu(fileName = "Graph Data", menuName = "Graph", order = 52)]
/// <summary>
/// Класс, представляющий граф комнат подземелья.
/// </summary>
public class Graph : ScriptableObject
{
    
    [SerializeField]
    /// <summary>
    /// Список вершин графа, представляющих комнаты.
    /// </summary>
    private List<Room> vertices;
    [SerializeField]
    /// <summary>
    /// Список ребер графа.
    /// </summary>
    private List<Corridor> edges;
    [SerializeField]
    /// <summary>
    /// Список центров комнат для Gizmo.
    /// </summary>
    private List<Vector3> gizmoCentres;
    [SerializeField]
    /// <summary>
    /// Матрица смежности графа.
    /// </summary>
    private int[,] graphMap;

    //public Graph(List<Room> rooms, int maxWidth, int maxHeigth)
    //{
    //    vertices = rooms.ToList();
    //    gizmoCentres = new List<Vector3>();
    //    for (int i = 0; i < rooms.Count; i++)
    //    {
    //        Vector2Int center = rooms[i].GetPosCenter();
    //        gizmoCentres.Add(new Vector3(center.y, center.x, 0));
    //    }
    //    edges = Triangulation(maxWidth, maxHeigth);
    //    graphMap = CalcMap();
    //    List<Corridor> spanningTree = SpanningTree(edges);
    //    edges.Except(spanningTree);
    //    for (int i = 0; i < UnityEngine.Random.Range(0, edges.Count); i++)
    //    {
    //        int index = UnityEngine.Random.Range(0, edges.Count);
    //        spanningTree.Add(edges[index]);
    //        edges.RemoveAt(index);
    //    }
    //    edges = spanningTree;
    //    AdjustEdges();
    //    graphMap = CalcMap();

    //    FinalizeMap();
    //}

    /// <summary>
    /// Конструктор графа.
    /// </summary>
    /// <param name="rooms">Список комнат.</param>
    /// <param name="maxWidth">Максимальная ширина карты.</param>
    /// <param name="maxHeight">Максимальная высота карты.</param>
    private void Init(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        vertices = rooms.ToList();
        gizmoCentres = new List<Vector3>();
        for (int i = 0; i < rooms.Count; i++)
        {
            Vector2Int center = rooms[i].GetPosCenter();
            gizmoCentres.Add(new Vector3(center.x, center.y, 0));
        }
        edges = Triangulation(maxWidth, maxHeigth);
        graphMap = CalcMap();
        List<Corridor> spanningTree = SpanningTree(edges);
        edges.Except(spanningTree);
        for (int i = 0; i < UnityEngine.Random.Range(0, edges.Count); i++)
        {
            int index = UnityEngine.Random.Range(0, edges.Count);
            spanningTree.Add(edges[index]);
            edges.RemoveAt(index);
        }
        edges = spanningTree;
        AdjustEdges();
        graphMap = CalcMap();
        List<String> roomJson = new List<string>();
        foreach (Room vertex in vertices)
        {
            roomJson.Add(JsonUtility.ToJson(vertex));
        }
        FinalizeMap();
    }
    public static Graph CreateGraph(List<Room> rooms, int maxWidth, int maxHeigth)
    {

        Graph graph = CreateInstance<Graph>();
        graph.Init(rooms, maxWidth, maxHeigth);
        AssetDatabase.CreateAsset(graph, "Assets/Scripts/Scriptable Objects/Generators/Geometry/Graph Data.asset");

        return graph;
    }
    public void OnEnable() //On Reload
    {
        Debug.Log("OnEnable");

    }
    public void ConvertToFile()
    {
        JsonUtility.ToJson(vertices);
    }
    public void ConvertToGraph()
    {

    }
    /// <summary>
    /// Проверяет, смежны ли две комнаты.
    /// </summary>
    /// <param name="room">Идентификатор первой комнаты.</param>
    /// <param name="roomOther">Идентификатор второй комнаты.</param>
    /// <returns>True - комнаты смежны, иначе - False.</returns>
    public bool IsAdjucent(int room, int roomOther)
    {
        return edges.Contains(Corridor.CreateCorridor(room, roomOther));
    }

    /// <summary>
    /// Проверяет, является ли идентификатор комнаты валидным.
    /// </summary>
    /// <param name="id">Идентификатор комнаты.</param>
    /// <returns>True - идентификатор валиден, иначе - False.</returns>
    public bool IsRoom(int id)
    {
        return id < vertices.Count;
    }

    /// <summary>
    /// Проверяет, является ли комната по идентификатору коридором.
    /// </summary>
    /// <param name="id">Идентификатор комнаты.</param>
    /// <returns>True - идентификатор коридора, иначе - False.</returns>
    public bool IsCorridor(int id)
    {
        return id >= vertices.Count;
    }

    /// <summary>
    /// Возвращает матрицу смежности графа.
    /// </summary>
    /// <returns>Матрица смежности графа.</returns>
    public int[,] GetGraphMap()
    {
        return graphMap;
    }

    /// <summary>
    /// Возвращает список комнат.
    /// </summary>
    /// <returns>Список комнат.</returns>
    public List<Room> GetRooms()
    {
        return vertices;
    }

    /// <summary>
    /// Возвращает список центров комнат для Gizmo.
    /// </summary>
    /// <returns>Список центров комнат.</returns>
    public List<Vector3> GetGizmos()
    { return gizmoCentres; }

    /// <summary>
    /// Возвращает центр комнаты для Gizmo по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор комнаты.</param>
    /// <returns>Центр комнаты.</returns>
    public Vector3 GetGizmoById(int id) 
    {
        if (id < vertices.Count)
            return gizmoCentres[id];
        else
            return Vector3.back;
    }

    /// <summary>
    /// Возвращает список коридоров графа.
    /// </summary>
    /// <returns>Список коридоров.</returns>
    public List<GraphEdge> GetCorridors()
    {
        return edges;
    }

    /// <summary>
    /// Возвращает идентификаторы комнат, соединенных ребром.
    /// </summary>
    /// <param name="id">Идентификатор ребра.</param>
    /// <returns>Кортеж идентификаторов комнат.</returns>
    public Tuple<int, int> GetRoomsFromEdge(int id)
    {
        return edges[id - vertices.Count].GetRoomsIds();
    }

    /// <summary>
    /// Возвращает коридор по его идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор коридора.</param>
    /// <returns>Коридор.</returns>
    public GraphEdge GetCorridor(int id)
    {
        if (id - vertices.Count < 0)
            return null;
        else
            return edges[id - vertices.Count];
    }

    /// <summary>
    /// Возвращает комнату по ее идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор комнаты.</param>
    /// <returns>Комната.</returns>
    public Room GetRoom(int id)
    {
        if (id >= vertices.Count)

            return null;
        else
            return vertices[id];
    }

    /// <summary>
    /// Возвращает координаты ребра.
    /// </summary>
    /// <param name="id">Идентификатор ребра.</param>
    /// <returns>Кортеж координат ребра.</returns>
    public Tuple<Vector2Int, Vector2Int> GetEdgeCoords(int id)
    {
        return edges[id - vertices.Count].GetCoords();
    }

    /// <summary>
    /// Возвращает список соседних комнат для заданной комнаты.
    /// </summary>
    /// <param name="room">Идентификатор комнаты.</param>
    /// <returns>Список соседних комнат.</returns>
    public List<int> GetNeighbors(int room)
    {
        List<int> neighbors = new List<int>();
        for (int i = 0; i < vertices.Count; i++)
        {
            if (graphMap[room, i] != -1)
                neighbors.Add(i);
        }
        return neighbors;
    }

    //???
    /// <summary>
    /// Корректирует позиции ребер.
    /// </summary>
    private void AdjustEdges()
    {
        foreach (Corridor edge in edges)
        {
            edge.AdjustPos(vertices[edge.idRoomFirst], vertices[edge.idRoomSecond]);
        }
    }

    /// <summary>
    /// Вычисляет матрицу смежности графа.
    /// </summary>
    /// <returns>Матрица смежности графа.</returns>
    private int[,] CalcMap()
    {
        int[,] map;
        if (graphMap == null)
        {
            map = new int[vertices.Count, vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
                for (int j = 0; j < vertices.Count; j++)
                    map[i, j] = -1;

            for (int i = 0; i < edges.Count; i++)
            {
                map[edges[i].idRoomFirst, edges[i].idRoomSecond] = i;
                map[edges[i].idRoomSecond, edges[i].idRoomFirst] = i;
            }
        }
        else
        {
            map = graphMap;
            for (int i = 0; i < vertices.Count; i++)
                for (int j = 0; j < vertices.Count; j++)
                    graphMap[i, j] = -1;

            for (int i = 0; i < edges.Count; i++)
            {
                graphMap[edges[i].idRoomFirst, edges[i].idRoomSecond] = i;
                graphMap[edges[i].idRoomSecond, edges[i].idRoomFirst] = i;
            }
        }
        return map;
    }

    /// <summary>
    /// Завершает построение карты.
    /// </summary>
    private void FinalizeMap()
    {
        for (int i = 0; i < vertices.Count; i++)
            for (int j = 0; j < vertices.Count; j++)
                if (graphMap[i, j] != -1)
                    graphMap[i, j] += vertices.Count();
    }

    //???
    /// <summary>
    /// Находит вершину с минимальным ключом, которая не включена в MST.
    /// </summary>
    /// <param name="key">Массив ключей.</param>
    /// <param name="mstSet"> - - -.</param>
    /// <returns>Индекс вершины с минимальным ключом.</returns>
    private int minKey(int[] key, bool[] mstSet)
    {
        int min = int.MaxValue, min_index = -1;

        for (int v = 0; v < vertices.Count; v++)
            if (mstSet[v] == false && key[v] < min)
            {
                min = key[v];
                min_index = v;
            }

        return min_index;
    }

    /// <summary>
    /// Строит остовное дерево графа.
    /// </summary>
    /// <param name="edges">Список ребер графа.</param>
    /// <returns>Список ребер остовного дерева.</returns>
    private List<GraphEdge> SpanningTree(List<GraphEdge> edges)
    {
        int[] parent = new int[vertices.Count];

        int[] key = new int[vertices.Count];

        bool[] mstSet = new bool[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            key[i] = int.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        for (int count = 0; count < vertices.Count - 1; count++)
        {
            int u = minKey(key, mstSet);
            mstSet[u] = true;
            for (int v = 0; v < vertices.Count; v++)
                if (graphMap[u, v] != -1 && mstSet[v] == false
                    && edges[graphMap[u, v]].GetLength() < key[v])
                {
                    parent[v] = u;
                    key[v] = (int)edges[graphMap[u, v]].GetLength();
                }
        }
        List<Corridor> edgesNew = new List<Corridor>();
        for (int i = 1; i < vertices.Count; i++)
            edgesNew.Add(edges[graphMap[i, parent[i]]]);
        return edgesNew;
    }

    /// <summary>
    /// Выполняет триангуляцию графа.
    /// </summary>
    /// <param name="maxWidth">Максимальная ширина карты.</param>
    /// <param name="maxHeigth">Максимальная высота карты.</param>
    /// <returns>Список ребер триангуляции.</returns>
    private List<GraphEdge> Triangulation(int maxWidth, int maxHeigth)
    {
        List<Corridor> edges = new List<Corridor>();

        List<Vector2Int> points = new List<Vector2Int>();
        for (int i = 0; i < vertices.Count; i++)
            points.Add(vertices[i].GetPosCenter());

        Triangle superTriangle = Triangle.SuperTriangle(maxWidth, maxHeigth);
        List<Triangle> triangles = new List<Triangle> { superTriangle };
        for (int vertId = 0; vertId < points.Count; vertId++)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            List<Corridor> badEdges = new List<Corridor>();
            foreach (Triangle triangle in triangles)
            {
                if (triangle.ContainsPoint(points[vertId]))
                {
                    badTriangles.Add(triangle);
                    foreach (Corridor badEdge in triangle.edges)
                        badEdges.Add(badEdge);
                }
            }
            List<Corridor> polygon = new List<Corridor>();
            for (int i = 0; i < badEdges.Count; i++)
            {
                bool isUnique = true;
                for (int j = 0; j < badEdges.Count; j++)
                {
                    if (i != j && badEdges[i] == badEdges[j])
                    {
                        isUnique = false;
                        break;
                    }
                }
                if (isUnique)
                    polygon.Add(badEdges[i]);
            }
            foreach (Triangle badTriangle in badTriangles)
            {
                triangles.Remove(badTriangle);
            }
            foreach (Corridor edge in polygon)
            {
                triangles.Add(new Triangle(edge, points[vertId], vertId));
            }
        }
        foreach (Triangle triangle in triangles)
            foreach (Corridor edge in triangle.edges)
                if (!edge.Contains(-1) && !edge.Contains(-2) && !edge.Contains(-3) && !edges.Contains(edge))
                    edges.Add(edge);


        return edges;
    }
}

/// <summary>
/// Класс, представляющий коридор.
/// </summary>
public class Corridor : ScriptableObject
{
    [SerializeField]
    /// <summary>
    /// Идентификатор первой комнаты.
    /// </summary>
    public int idRoomFirst;
    [SerializeField]
    /// <summary>
    /// Идентификатор второй комнаты.
    /// </summary>
    public int idRoomSecond;
    [SerializeField]
    /// <summary>
    /// Позиция первой точки ребра.
    /// </summary>
    public Vector2Int posPoint1;
    [SerializeField]
        /// <summary>
    /// Позиция второй точки ребра.
    /// </summary>
    public Vector2Int posPoint2;
    [SerializeField]
        /// <summary>
    /// Длина ребра.
    /// </summary>
    private float length;
    
    //public Corridor(int idFirst, int idSecond)
    //{
    //    idRoomFirst = idFirst;
    //    idRoomSecond = idSecond;
    //    posPoint1 = Vector2Int.zero;
    //    posPoint2 = Vector2Int.zero;
    //    length = -1;
    //}
    //public Corridor(int idFirst, int idSecond, Vector2Int posPoint1, Vector2Int posPoint2)
    //{
    //    idRoomFirst = idFirst;
    //    idRoomSecond = idSecond;
    //    this.posPoint1 = posPoint1;
    //    this.posPoint2 = posPoint2;
    //    length = Vector2.Distance(posPoint1, posPoint2);
    //}
    
    /// <summary>
    /// Конструктор ребра графа.
    /// </summary>
    /// <param name="idFirst">Идентификатор первой комнаты.</param>
    /// <param name="idSecond">Идентификатор второй комнаты.</param>
    private void Init(int idFirst, int idSecond)
    {
        idRoomFirst = idFirst;
        idRoomSecond = idSecond;
        posPoint1 = Vector2Int.zero;
        posPoint2 = Vector2Int.zero;
        length = -1;
    }
    
    /// <summary>
    /// Конструктор ребра графа с указанием позиций точек.
    /// </summary>
    /// <param name="idFirst">Идентификатор первой комнаты.</param>
    /// <param name="idSecond">Идентификатор второй комнаты.</param>
    /// <param name="posPoint1">Позиция первой точки.</param>
    /// <param name="posPoint2">Позиция второй точки.</param>
    private void Init(int idFirst, int idSecond, Vector2Int posPoint1, Vector2Int posPoint2)
    {
        idRoomFirst = idFirst;
        idRoomSecond = idSecond;
        this.posPoint1 = posPoint1;
        this.posPoint2 = posPoint2;
        length = Vector2.Distance(posPoint1, posPoint2);
    }
    public static Corridor CreateCorridor(int idFirst, int idSecond)
    {
        Corridor corridor = CreateInstance<Corridor>();
        corridor.Init(idFirst, idSecond);
        return corridor;
    }
    public static Corridor CreateCorridor(int idFirst, int idSecond, Vector2Int posPoint1, Vector2Int posPoint2)
    {
        Corridor corridor = CreateInstance<Corridor>();
        corridor.Init(idFirst, idSecond, posPoint1, posPoint2);
        return corridor;
    }
    /// <summary>
    /// Оператор сравнения ребер графа.
    /// </summary>
    public static bool operator ==(Corridor first, Corridor second)
    {
        return first.idRoomFirst == second.idRoomFirst && first.idRoomSecond == second.idRoomSecond
            || first.idRoomFirst == second.idRoomSecond && first.idRoomSecond == second.idRoomFirst;
    }
    /// <summary>
    /// Оператор неравенства ребер графа.
    /// </summary>
    public static bool operator !=(Corridor first, Corridor second)
    {
        return !(first == second);
    }

    /// <summary>
    /// Проверяет равенство текущего ребра с другим.
    /// </summary>
    /// <param name="other">Другое ребро.</param>
    /// <returns>True - ребра равны, иначе - False.</returns>
    public override bool Equals(object other)
    {
        if (!(other is Corridor))
        {
            return false;
        }

        return Equals((Corridor)other);
    }
    /// <summary>
    /// Проверяет равенство текущего ребра с другим.
    /// </summary>
    /// <param name="other">Другое ребро.</param>
    /// <returns>True - ребра равны, иначе - False.</returns>
    public bool Equals(Corridor other)
    {
        return idRoomFirst == other.idRoomFirst && idRoomSecond == other.idRoomSecond
            || idRoomFirst == other.idRoomSecond && idRoomSecond == other.idRoomFirst;
    }

    /// <summary>
    /// Возвращает хэш-код ребра.
    /// </summary>
    /// <returns>Хэш-код ребра.</returns>
    public override int GetHashCode()
    {
        return idRoomFirst.GetHashCode() ^ (idRoomSecond.GetHashCode() << 2);
    }

    /// <summary>
    /// Проверяет, содержит ли ребро указанную комнату.
    /// </summary>
    /// <param name="idRoom">Идентификатор комнаты.</param>
    /// <returns>True - ребро содержит комнату, иначе - False.</returns>
    public bool Contains(int idRoom)
    {
        return idRoom == idRoomFirst || idRoom == idRoomSecond;
    }

    /// <summary>
    /// Возвращает идентификатор соседней комнаты.
    /// </summary>
    /// <param name="idRoom">Идентификатор комнаты.</param>
    /// <returns>Идентификатор соседней комнаты.</returns>
    public int GetRoomNeighbor(int idRoom)
    {
        if (idRoom == idRoomFirst)
            return idRoomSecond;
        if (idRoom == idRoomSecond)
            return idRoomFirst;
        return -1;
    }

    /// <summary>
    /// Возвращает кортеж идентификаторов комнат, соединенных ребром.
    /// </summary>
    /// <returns>Кортеж идентификаторов комнат.</returns>
    public Tuple<int, int> GetRoomsIds()
    {
        return new Tuple<int, int>(idRoomFirst, idRoomSecond);
    }

    /// <summary>
    /// Возвращает кортеж позиций точек ребра.
    /// </summary>
    /// <returns>Кортеж позиций точек ребра.</returns>
    public Tuple<Vector2Int, Vector2Int> GetCoords()
    {
        return new Tuple<Vector2Int, Vector2Int>(posPoint1, posPoint2);
    }

    /// <summary>
    /// Возвращает длину ребра.
    /// </summary>
    /// <returns>Длина ребра.</returns>
    public float GetLength()
    {
        return length;
    }

    /// <summary>
    /// Возвращает позицию точки по идентификатору комнаты.
    /// </summary>
    /// <param name="id">Идентификатор комнаты.</param>
    /// <returns>Позиция точки.</returns>
    public Vector2Int GetPosById(int id)
    {
        if (id == idRoomFirst)
            return posPoint1;
        if (id == idRoomSecond)
            return posPoint2;
        return Vector2Int.zero;
    }

    /// <summary>
    /// Устанавливает позиции точек ребра.
    /// </summary>
    /// <param name="posPoint1">Позиция первой точки.</param>
    /// <param name="posPoint2">Позиция второй точки.</param>
    public void SetPoses(Vector2Int posPoint1, Vector2Int posPoint2)
    {
        this.posPoint1 = posPoint1;
        this.posPoint2 = posPoint2;
    }

    /// <summary>
    /// Корректирует позиции ребра на основе позиций комнат.
    /// </summary>
    /// <param name="room">Первая комната.</param>
    /// <param name="roomOther">Вторая комната.</param>
    public void AdjustPos(Room room, Room roomOther)
    {

        Vector2Int roomPos = room.GetPos();
        Vector2Int roomPosOther = roomOther.GetPosCenter();
        int newX = -1;
        int newY = -1;

        int roomDistance = int.MaxValue;
        int[,] roomTiles = room.GetTiles();
        Size roomSize = room.GetSize();
        for (int y = 0; y < roomSize.Height; y++)
            for (int x = 0; x < roomSize.Width; x++)
            {
                if (roomTiles[y, x] != 0)
                {
                    int roomDistanceNew = (roomPos.x + x - roomPosOther.x) * (roomPos.x + x - roomPosOther.x) + (roomPos.y + y - roomPosOther.y) * (roomPos.y + y - roomPosOther.y);
                    if (roomDistance >= roomDistanceNew)
                    {
                        roomDistance = roomDistanceNew;
                        newX = roomPos.x + x;
                        newY = roomPos.y + y;
                    }
                }
            }
        posPoint1.Set(newX, newY);

        roomPos = roomOther.GetPos();
        roomPosOther = posPoint1;
        roomDistance = int.MaxValue;
        roomTiles = roomOther.GetTiles();
        roomSize = roomOther.GetSize();
        for (int y = 0; y < roomSize.Height; y++)
            for (int x = 0; x < roomSize.Width; x++)
            {
                if (roomTiles[y, x] != 0)
                {
                    int roomDistanceNew = (roomPos.x + x - roomPosOther.x) * (roomPos.x + x - roomPosOther.x) + (roomPos.y + y - roomPosOther.y) * (roomPos.y + y - roomPosOther.y);
                    if (roomDistance >= roomDistanceNew)
                    {
                        roomDistance = roomDistanceNew;
                        newX = roomPos.x + x;
                        newY = roomPos.y + y;
                    }
                }
            }
        posPoint2.Set(newX, newY);
    }

}

/// <summary>
/// Класс, представляющий треугольник для триангуляции.
/// </summary>
public class Triangle
{
    /// <summary>
    /// Массив ребер треугольника.
    /// </summary>
    public Corridor[] edges;
        /// <summary>
    /// Описанная окружность треугольника.
    /// </summary>
    public CircumCircle circle;

    /// <summary>
    /// Конструктор треугольника с указанием позиций точек и идентификаторов ребер.
    /// </summary>
    /// <param name="posPoint1">Позиция первой точки.</param>
    /// <param name="posPoint2">Позиция второй точки.</param>
    /// <param name="posPoint3">Позиция третьей точки.</param>
    /// <param name="edgesId">Массив идентификаторов ребер.</param>
    public Triangle(Vector2Int posPoint1, Vector2Int posPoint2, Vector2Int posPoint3, int[] edgesId)
    {
        edges = new Corridor[3]
        {
             Corridor.CreateCorridor(edgesId[0], edgesId[1],posPoint1, posPoint2 ),
             Corridor.CreateCorridor(edgesId[1], edgesId[2],posPoint2, posPoint3) ,
             Corridor.CreateCorridor(edgesId[2], edgesId[0],posPoint3, posPoint1 )
        };
        circle = new CircumCircle(posPoint1, posPoint2, posPoint3);
    }

    /// <summary>
    /// Конструктор треугольника с указанием позиций точек.
    /// </summary>
    /// <param name="posPoint1">Позиция первой точки.</param>
    /// <param name="posPoint2">Позиция второй точки.</param>
    /// <param name="posPoint3">Позиция третьей точки.</param>
    public Triangle(Vector2Int posPoint1, Vector2Int posPoint2, Vector2Int posPoint3)
    {
        edges = new Corridor[3]
        {
            Corridor.CreateCorridor(-1, -2,posPoint1, posPoint2 ),
            Corridor.CreateCorridor(-2, -3,posPoint2, posPoint3) ,
            Corridor.CreateCorridor(-3, -1,posPoint3, posPoint1 )
        };
        circle = new CircumCircle(posPoint1, posPoint2, posPoint3);

    }

    /// <summary>
    /// Конструктор треугольника с указанием ребра и позиции точки.
    /// </summary>
    /// <param name="edge">Ребро треугольника.</param>
    /// <param name="point">Позиция точки.</param>
    /// <param name="vertexId">Идентификатор вершины.</param>
    public Triangle(GraphEdge edge, Vector2Int point, int vertexId)
    {
        edges = new Corridor[3]
        {
            Corridor.CreateCorridor(edge.idRoomFirst, edge.idRoomSecond,edge.posPoint1, edge.posPoint2),
            Corridor.CreateCorridor(edge.idRoomSecond, vertexId,edge.posPoint2, point) ,
            Corridor.CreateCorridor(vertexId, edge.idRoomFirst,point, edge.posPoint1)
        };
        circle = new CircumCircle(edge.posPoint1, edge.posPoint2, point);

    }

    /// <summary>
    /// Создает супер-треугольник, охватывающий всю карту.
    /// </summary>
    /// <param name="maxWidth">Максимальная ширина карты.</param>
    /// <param name="maxHeight">Максимальная высота карты.</param>
    /// <returns>Супер-треугольник.</returns>
    public static Triangle SuperTriangle(int maxWidth, int maxHeigth)
    {
        int margin = 1000;
        Vector2Int point1 = new Vector2Int(maxWidth / 2, -2 * maxWidth - margin);
        Vector2Int point2 = new Vector2Int(-2 * maxHeigth - margin, 2 * maxHeigth + margin);
        Vector2Int point3 = new Vector2Int(2 * maxWidth + maxHeigth + margin, 2 * maxHeigth + margin);
        return new Triangle(point1, point2, point3);

    }

    /// <summary>
    /// Проверяет, находится ли точка внутри описанной окружности треугольника.
    /// </summary>
    /// <param name="vertex">Позиция точки.</param>
    /// <returns>True - точка находится внутри, иначе - False.</returns>
    public bool InCircle(Vector2Int vertex)
    {
        if ((vertex.x - circle.center.x) * (vertex.x - circle.center.x) + (vertex.y - circle.center.y) * (vertex.y - circle.center.y) <= circle.radius * circle.radius)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Проверяет, содержит ли треугольник указанное ребро.
    /// </summary>
    /// <param name="edge">Ребро.</param>
    /// <returns>True, - треугольник содержит ребро, иначе - False.</returns>
    public bool Contains(GraphEdge edge)
    {
        foreach (Corridor edgeTrianlge in edges)
        {
            if (edgeTrianlge == edge)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Проверяет, является ли треугольник супер-треугольником.
    /// </summary>
    /// <returns>True - треугольник является супер-треугольником, иначе - False.</returns>
    public bool SuperTriangleCheck()
    {
        foreach (Corridor edge in edges)
        {
            if (edge.idRoomFirst < 0 || edge.idRoomSecond < 0)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Проверяет, содержит ли треугольник указанную точку.
    /// </summary>
    /// <param name="point">Точка.</param>
    /// <returns>True - если треугольник содержит точку, иначе - False.</returns>
    public bool ContainsPoint(Vector2Int point)
    {
        return circle.ContainsPoint(point);
    }
}

/// <summary>
/// Класс, представляющий описанную окружность треугольника.
/// </summary>
public class CircumCircle
{
    /// <summary>
    /// Центр окружности.
    /// </summary>
    public Vector2 center;

    /// <summary>
    /// Радиус окружности.
    /// </summary>
    public float radius;

    /// <summary>
    /// Конструктор окружности, описывающей треугольник.
    /// </summary>
    /// <param name="posPoint1">Позиция первой точки.</param>
    /// <param name="posPoint2">Позиция второй точки.</param>
    /// <param name="posPoint3">Позиция третьей точки.</param>
    public CircumCircle(Vector2 posPoint1, Vector2 posPoint2, Vector2 posPoint3)
    {
        var dA = posPoint1.x * posPoint1.x + posPoint1.y * posPoint1.y;
        var dB = posPoint2.x * posPoint2.x + posPoint2.y * posPoint2.y;
        var dC = posPoint3.x * posPoint3.x + posPoint3.y * posPoint3.y;

        var aux1 = (dA * (posPoint3.y - posPoint2.y) + dB * (posPoint1.y - posPoint3.y) + dC * (posPoint2.y - posPoint1.y));
        var aux2 = -(dA * (posPoint3.x - posPoint2.x) + dB * (posPoint1.x - posPoint3.x) + dC * (posPoint2.x - posPoint1.x));
        var div = (2 * (posPoint1.x * (posPoint3.y - posPoint2.y) + posPoint2.x * (posPoint1.y - posPoint3.y) + posPoint3.x * (posPoint2.y - posPoint1.y)));

        if (div == 0)
        {
            throw new DivideByZeroException();
        }
        center = new Vector2(aux1 / div, aux2 / div);
        radius = Vector2.Distance(center, posPoint1);
    }

    /// <summary>
    /// Проверяет, содержит ли окружность указанную точку.
    /// </summary>
    /// <param name="point">Точка.</param>
    /// <returns>True - окружность содержит точку, иначе - False.</returns>
    public bool ContainsPoint(Vector2 point)
    {
        if (Vector2.Distance(center, point) <= radius)
            return true;
        else
            return false;
    }
}
