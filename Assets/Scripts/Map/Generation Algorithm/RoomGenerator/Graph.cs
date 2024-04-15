using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph
{
    public List<Room> vertices;
    public List<GraphEdge> edges;
    public int[,] graphMap;

    public Graph(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        vertices = rooms.ToList();
        graphMap = new int[vertices.Count, vertices.Count];
        edges = Triangulation(vertices, maxWidth, maxHeigth);
        for (int i = 0; i < vertices.Count; i++)
            for (int j = 0; j < vertices.Count; j++)
                graphMap[i, j] = -1;
        for (int i =0; i <edges.Count; i++)
        {
            graphMap[edges[i].idRoomFirst, edges[i].idRoomSecond] = i;
            graphMap[edges[i].idRoomSecond, edges[i].idRoomFirst] = i;
        }
        edges = SpanningTree();
        for (int i = 0; i < vertices.Count; i++)
            for (int j = 0; j < vertices.Count; j++)
                graphMap[i, j] = -1;
        for (int i = 0; i < edges.Count; i++)
        {
            graphMap[edges[i].idRoomFirst, edges[i].idRoomSecond] = i;
            graphMap[edges[i].idRoomSecond, edges[i].idRoomFirst] = i;
        }
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
    public Tuple<int, int> GetRoomsFromEdge(int id)
    {
        return edges[id].GetRooms();
    }
    public List<int> GetNeighbors(int room)
    {
        List<int> neighbors = new List<int>();
        for(int i =0; i < vertices.Count;i++) 
        {
            if (graphMap[room,i] != -1)
                neighbors.Add(i);
        }
        return neighbors;
    }
    int minKey(int[] key, bool[] mstSet)
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
    private List<GraphEdge> Triangulation(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        List<GraphEdge> edges = new List<GraphEdge>();

        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < rooms.Count; i++)
            points.Add(rooms[i].GetPosCenter());

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

}

public class GraphEdge
{
    public int idRoomFirst;
    public int idRoomSecond;
    public Vector2 posPointFirst;
    public Vector2 posPointSecond;
    private int length;
    public GraphEdge(int idFirst, int idSecond)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
    }
    public GraphEdge(int idFirst, int idSecond, Vector2 posPointFirst, Vector2 posPointSecond)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
        this.length = (int)Vector2.Distance(posPointFirst, posPointSecond);

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
    public Tuple<int, int> GetRooms()
    {
        return new Tuple<int, int>(idRoomFirst, idRoomSecond);
    }
    public int GetLength()
    {
        return (int)Vector2.Distance(posPointFirst, posPointSecond);
    }
    public void SetPoses(Vector2 posPointFirst, Vector2 posPointSecond)
    {
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
    }

}
public class Triangle
{
    public GraphEdge[] edges;
    public CircumCircle circle;

    public Triangle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird, int[] edgesId)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(edgesId[0], edgesId[1],posPointFirst, posPointSecond ),
            new GraphEdge(edgesId[1], edgesId[2],posPointSecond, posPointThird) ,
            new GraphEdge(edgesId[2], edgesId[0],posPointThird, posPointFirst )
        };
        circle = new CircumCircle(posPointFirst, posPointSecond, posPointThird);
    }
    public Triangle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(-1, -2,posPointFirst, posPointSecond ),
            new GraphEdge(-2, -3,posPointSecond, posPointThird) ,
            new GraphEdge(-3, -1,posPointThird, posPointFirst )
        };
        circle = new CircumCircle(posPointFirst, posPointSecond, posPointThird);

    }
    public Triangle(GraphEdge edge, Vector2 point, int vertexId)
    {
        edges = new GraphEdge[3]
        {
            new GraphEdge(edge.idRoomFirst, edge.idRoomSecond,edge.posPointFirst, edge.posPointSecond),
            new GraphEdge(edge.idRoomSecond, vertexId,edge.posPointSecond, point) ,
            new GraphEdge(vertexId, edge.idRoomFirst,point, edge.posPointFirst)
        };
        circle = new CircumCircle(edge.posPointFirst, edge.posPointSecond, point);

    }
    public static Triangle SuperTriangle(int maxWidth, int maxHeigth)
    {
        int margin = 1000;
        Vector2 point1 = new Vector2(0.5f * maxWidth, -2 * maxWidth - margin);
        Vector2 point2 = new Vector2(-2 * maxHeigth - margin, 2 * maxHeigth + margin);
        Vector2 point3 = new Vector2(2 * maxWidth + maxHeigth + margin, 2 * maxHeigth + margin);
        return new Triangle(point1, point2, point3);

    }
    public bool InCircle(Vector2 vertex)
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
    public bool ContainsPoint(Vector2 point)
    {
        return circle.ContainsPoint(point);
    }
}
public class CircumCircle
{
    public Vector2 center;
    public float radius;
    public CircumCircle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
    {
        var p0 = posPointFirst;
        var p1 = posPointSecond;
        var p2 = posPointThird;
        var dA = p0.x * p0.x + p0.y * p0.y;
        var dB = p1.x * p1.x + p1.y * p1.y;
        var dC = p2.x * p2.x + p2.y * p2.y;

        var aux1 = (dA * (p2.y - p1.y) + dB * (p0.y - p2.y) + dC * (p1.y - p0.y));
        var aux2 = -(dA * (p2.x - p1.x) + dB * (p0.x - p2.x) + dC * (p1.x - p0.x));
        var div = (2 * (p0.x * (p2.y - p1.y) + p1.x * (p0.y - p2.y) + p2.x * (p1.y - p0.y)));

        if (div == 0)
        {
            throw new DivideByZeroException();
        }
        center = new Vector2(aux1 / div, aux2 / div);
        radius = Vector2.Distance(center, posPointFirst);
    }
    public bool ContainsPoint(Vector2 point)
    {
        if (Vector2.Distance(center, point) <= radius)
            return true;
        else
            return false;
    }
}
