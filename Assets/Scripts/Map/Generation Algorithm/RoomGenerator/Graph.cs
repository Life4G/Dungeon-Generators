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
public class Graph : ScriptableObject
{
    [SerializeField]
    private List<Room> vertices;
    [SerializeField]
    private List<Corridor> edges;
    [SerializeField]
    private List<Vector3> gizmoCentres;
    [SerializeField]
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
    public bool IsAdjucent(int room, int roomOther)
    {
        return edges.Contains(Corridor.CreateCorridor(room, roomOther));
    }
    public bool IsRoom(int id)
    {
        return id < vertices.Count;
    }
    public bool IsCorridor(int id)
    {
        return id >= vertices.Count;
    }
    public int[,] GetGraphMap()
    {
        return graphMap;
    }
    public List<Room> GetRooms()
    {
        return vertices;
    }
    public List<Vector3> GetGizmos()
    { return gizmoCentres; }
    public Vector3 GetGizmoById(int id)
    {
        if (id < vertices.Count)
            return gizmoCentres[id];
        else
            return Vector3.back;
    }
    public List<Corridor> GetCorridors()
    {
        return edges;
    }
    public Tuple<int, int> GetRoomsFromEdge(int id)
    {
        return edges[id - vertices.Count].GetRoomsIds();
    }
    public Corridor GetCorridor(int id)
    {
        if (id - vertices.Count < 0)
            return null;
        else
            return edges[id - vertices.Count];
    }
    public Room GetRoom(int id)
    {
        if (id >= vertices.Count)

            return null;
        else
            return vertices[id];
    }
    public Tuple<Vector2Int, Vector2Int> GetEdgeCoords(int id)
    {
        return edges[id - vertices.Count].GetCoords();
    }
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
    private void AdjustEdges()
    {
        foreach (Corridor edge in edges)
        {
            edge.AdjustPos(vertices[edge.idRoomFirst], vertices[edge.idRoomSecond]);
        }
    }
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
    private void FinalizeMap()
    {
        for (int i = 0; i < vertices.Count; i++)
            for (int j = 0; j < vertices.Count; j++)
                if (graphMap[i, j] != -1)
                    graphMap[i, j] += vertices.Count();
    }

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
    private List<Corridor> SpanningTree(List<Corridor> edges)
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
    private List<Corridor> Triangulation(int maxWidth, int maxHeigth)
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


public class Corridor : ScriptableObject
{
    [SerializeField]
    public int idRoomFirst;
    [SerializeField]
    public int idRoomSecond;
    [SerializeField]
    public Vector2Int posPoint1;
    [SerializeField]
    public Vector2Int posPoint2;
    [SerializeField]
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
    private void Init(int idFirst, int idSecond)
    {
        idRoomFirst = idFirst;
        idRoomSecond = idSecond;
        posPoint1 = Vector2Int.zero;
        posPoint2 = Vector2Int.zero;
        length = -1;
    }
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

    public static bool operator ==(Corridor first, Corridor second)
    {
        return first.idRoomFirst == second.idRoomFirst && first.idRoomSecond == second.idRoomSecond
            || first.idRoomFirst == second.idRoomSecond && first.idRoomSecond == second.idRoomFirst;
    }
    public static bool operator !=(Corridor first, Corridor second)
    {
        return !(first == second);
    }
    public override bool Equals(object other)
    {
        if (!(other is Corridor))
        {
            return false;
        }

        return Equals((Corridor)other);
    }
    public bool Equals(Corridor other)
    {
        return idRoomFirst == other.idRoomFirst && idRoomSecond == other.idRoomSecond
            || idRoomFirst == other.idRoomSecond && idRoomSecond == other.idRoomFirst;
    }
    public override int GetHashCode()
    {
        return idRoomFirst.GetHashCode() ^ (idRoomSecond.GetHashCode() << 2);
    }
    public bool Contains(int idRoom)
    {
        return idRoom == idRoomFirst || idRoom == idRoomSecond;
    }
    public int GetRoomNeighbor(int idRoom)
    {
        if (idRoom == idRoomFirst)
            return idRoomSecond;
        if (idRoom == idRoomSecond)
            return idRoomFirst;
        return -1;
    }
    public Tuple<int, int> GetRoomsIds()
    {
        return new Tuple<int, int>(idRoomFirst, idRoomSecond);
    }
    public Tuple<Vector2Int, Vector2Int> GetCoords()
    {
        return new Tuple<Vector2Int, Vector2Int>(posPoint1, posPoint2);
    }
    public float GetLength()
    {
        return length;
    }
    public Vector2Int GetPosById(int id)
    {
        if (id == idRoomFirst)
            return posPoint1;
        if (id == idRoomSecond)
            return posPoint2;
        return Vector2Int.zero;
    }
    public void SetPoses(Vector2Int posPoint1, Vector2Int posPoint2)
    {
        this.posPoint1 = posPoint1;
        this.posPoint2 = posPoint2;
    }
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
public class Triangle
{
    public Corridor[] edges;
    public CircumCircle circle;

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
    public Triangle(Corridor edge, Vector2Int point, int vertexId)
    {
        edges = new Corridor[3]
        {
            Corridor.CreateCorridor(edge.idRoomFirst, edge.idRoomSecond,edge.posPoint1, edge.posPoint2),
            Corridor.CreateCorridor(edge.idRoomSecond, vertexId,edge.posPoint2, point) ,
            Corridor.CreateCorridor(vertexId, edge.idRoomFirst,point, edge.posPoint1)
        };
        circle = new CircumCircle(edge.posPoint1, edge.posPoint2, point);

    }
    public static Triangle SuperTriangle(int maxWidth, int maxHeigth)
    {
        int margin = 1000;
        Vector2Int point1 = new Vector2Int(maxWidth / 2, -2 * maxWidth - margin);
        Vector2Int point2 = new Vector2Int(-2 * maxHeigth - margin, 2 * maxHeigth + margin);
        Vector2Int point3 = new Vector2Int(2 * maxWidth + maxHeigth + margin, 2 * maxHeigth + margin);
        return new Triangle(point1, point2, point3);

    }
    public bool InCircle(Vector2Int vertex)
    {
        if ((vertex.x - circle.center.x) * (vertex.x - circle.center.x) + (vertex.y - circle.center.y) * (vertex.y - circle.center.y) <= circle.radius * circle.radius)
            return true;
        else
            return false;
    }
    public bool Contains(Corridor edge)
    {
        foreach (Corridor edgeTrianlge in edges)
        {
            if (edgeTrianlge == edge)
                return true;
        }
        return false;
    }
    public bool SuperTriangleCheck()
    {
        foreach (Corridor edge in edges)
        {
            if (edge.idRoomFirst < 0 || edge.idRoomSecond < 0)
                return true;
        }
        return false;
    }
    public bool ContainsPoint(Vector2Int point)
    {
        return circle.ContainsPoint(point);
    }
}
public class CircumCircle
{
    public Vector2 center;
    public float radius;
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
    public bool ContainsPoint(Vector2 point)
    {
        if (Vector2.Distance(center, point) <= radius)
            return true;
        else
            return false;
    }
}
