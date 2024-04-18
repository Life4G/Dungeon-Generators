using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    private readonly List<Room> vertices;
    private readonly List<GraphEdge> edges;
    private readonly int[,] graphMap;

    public Graph(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        vertices = rooms.ToList();
        edges = Triangulation(maxWidth, maxHeigth);
        graphMap = CalcMap();
        edges = SpanningTree();
        graphMap = CalcMap();
        FinalizeMap();
    }
    public bool IsAdjucent(int room, int roomOther)
    {
        return edges.Contains(new GraphEdge(room, roomOther));
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
    public List<GraphEdge> GetCorridors()
    {
        return edges;
    }
    public Tuple<int, int> GetRoomsFromEdge(int id)
    {
        return edges[id - vertices.Count].GetRoomsIds();
    }
    public GraphEdge GetCorridor(int id)
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
    private int minKey(int[] key, bool[] mstSet)
    {
        // Initialize min value
        int min = int.MaxValue, min_index = -1;

        for (int v = 0; v < vertices.Count; v++)
            if (mstSet[v] == false && key[v] < min)
            {
                min = key[v];
                min_index = v;
            }

        return min_index;
    }
    private List<GraphEdge> SpanningTree()
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
                    key[v] = edges[graphMap[u, v]].GetLength();
                }
        }
        List<GraphEdge> edgesNew = new List<GraphEdge>();
        for (int i = 1; i < vertices.Count; i++)
            edgesNew.Add(edges[graphMap[i, parent[i]]]);
        return edgesNew;
    }
    private List<GraphEdge> Triangulation(int maxWidth, int maxHeigth)
    {
        List<GraphEdge> edges = new List<GraphEdge>();

        List<Vector2Int> points = new List<Vector2Int>();
        for (int i = 0; i < vertices.Count; i++)
            points.Add(vertices[i].GetPosCenter());

        Triangle superTriangle = Triangle.SuperTriangle(maxWidth, maxHeigth);
        List<Triangle> triangles = new List<Triangle> { superTriangle };
        for (int vertId = 0; vertId < points.Count; vertId++)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            List<GraphEdge> badEdges = new List<GraphEdge>();
            foreach (Triangle triangle in triangles)
            {
                if (triangle.ContainsPoint(points[vertId]))
                {
                    badTriangles.Add(triangle);
                    foreach (GraphEdge badEdge in triangle.edges)
                        badEdges.Add(badEdge);
                }
            }
            List<GraphEdge> polygon = new List<GraphEdge>();
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
            foreach (GraphEdge edge in polygon)
            {
                triangles.Add(new Triangle(edge, points[vertId], vertId));
            }
        }
        foreach (Triangle triangle in triangles)
            foreach (GraphEdge edge in triangle.edges)
                if (!edge.Contains(-1) && !edge.Contains(-2) && !edge.Contains(-3) && !edges.Contains(edge))
                    edges.Add(edge);


        return edges;
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
}

public class GraphEdge
{
    public int idRoomFirst;
    public int idRoomSecond;
    public Vector2Int posPoint1;
    public Vector2Int posPoint2;
    private int length;
    public GraphEdge(int idFirst, int idSecond)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
    }
    public GraphEdge(int idFirst, int idSecond, Vector2Int posPoint1, Vector2Int posPoint2)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
        this.posPoint1 = posPoint1;
        this.posPoint2 = posPoint2;
        this.length = (int)Vector2.Distance(posPoint1, posPoint2);

    }
    public static bool operator ==(GraphEdge first, GraphEdge second)
    {
        return first.idRoomFirst == second.idRoomFirst && first.idRoomSecond == second.idRoomSecond
            || first.idRoomFirst == second.idRoomSecond && first.idRoomSecond == second.idRoomFirst;
    }
    public static bool operator !=(GraphEdge first, GraphEdge second)
    {
        return !(first == second);
    }
    public override bool Equals(object other)
    {
        if (!(other is GraphEdge))
        {
            return false;
        }

        return Equals((GraphEdge)other);
    }
    public bool Equals(GraphEdge other)
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
    public int GetLength()
    {
        return (int)Vector2.Distance(posPoint1, posPoint2);
    }
    public void SetPoses(Vector2Int posPoint1, Vector2Int posPoint2)
    {
        this.posPoint1 = posPoint1;
        this.posPoint2 = posPoint2;
    }

}
public class Triangle
{
    public GraphEdge[] edges;
    public CircumCircle circle;

    public Triangle(Vector2Int posPoint1, Vector2Int posPoint2, Vector2Int posPoint3, int[] edgesId)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(edgesId[0], edgesId[1],posPoint1, posPoint2 ),
            new GraphEdge(edgesId[1], edgesId[2],posPoint2, posPoint3) ,
            new GraphEdge(edgesId[2], edgesId[0],posPoint3, posPoint1 )
        };
        circle = new CircumCircle(posPoint1, posPoint2, posPoint3);
    }
    public Triangle(Vector2Int posPoint1, Vector2Int posPoint2, Vector2Int posPoint3)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(-1, -2,posPoint1, posPoint2 ),
            new GraphEdge(-2, -3,posPoint2, posPoint3) ,
            new GraphEdge(-3, -1,posPoint3, posPoint1 )
        };
        circle = new CircumCircle(posPoint1, posPoint2, posPoint3);

    }
    public Triangle(GraphEdge edge, Vector2Int point, int vertexId)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(edge.idRoomFirst, edge.idRoomSecond,edge.posPoint1, edge.posPoint2),
            new GraphEdge(edge.idRoomSecond, vertexId,edge.posPoint2, point) ,
            new GraphEdge(vertexId, edge.idRoomFirst,point, edge.posPoint1)
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
    public bool Contains(GraphEdge edge)
    {
        foreach (GraphEdge edgeTrianlge in edges)
        {
            if (edgeTrianlge == edge)
                return true;
        }
        return false;
    }
    public bool SuperTriangleCheck()
    {
        foreach (GraphEdge edge in edges)
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
