using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Graph
{
    public List<Room> vertices;
    public List<GraphEdge> edges;

    public Graph(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        vertices = rooms.ToList();
        edges = Triangulation(vertices, maxWidth, maxHeigth);
        //edges = SpanningTree(vertices, edges);
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
    public Tuple<int, int> GetConnectedRooms(int id)
    {
        return edges[id].GetRooms();
    }
    public List<int> GetNeighbors(int room)
    {
        List<int> neighbors = new List<int>();
        foreach (GraphEdge edge in edges)
        {
            if (edge.Contains(room))
                neighbors.Add(edge.GetRoomNeighbor(room));
        }
        return neighbors;

    }
    public List<GraphEdge> SpanningTree(List<Room> rooms, List<GraphEdge> edges)
    {

        List<List<int[]>> adj = new List<List<int[]>>();
        List<GraphEdge> edgesSpanned = new List<GraphEdge>();
        for (int i = 0; i < vertices.Count; i++)
        {
            adj.Add(new List<int[]>());
        }


        for (int i = 0; i < edges.Count; i++)
        {
            int u = edges[i].idRoomFirst;
            int v = edges[i].idRoomSecond;
            int wt = edges[i].GetLength();
            adj[u].Add(new int[] { v, wt });
            adj[v].Add(new int[] { u, wt });
        }


        PriorityQueue<(int, int)> pq = new PriorityQueue<(int, int)>();


        bool[] visited = new bool[vertices.Count];


        int res = 0;


        pq.Enqueue((0, 0));
        int prev = 0;


        while (pq.Count > 0)
        {
            var p = pq.Dequeue();
            int wt = p.Item1;
            int u = p.Item2;

            if (visited[u])
            {
                continue;
            }

            res += wt;
            edgesSpanned.Add(new GraphEdge(prev, u, rooms[prev].GetPosCenter(), rooms[u].GetPosCenter()));
            prev = u;
            visited[u] = true;

            foreach (var v in adj[u])
            {
                if (!visited[v[0]])
                {
                    pq.Enqueue((v[1], v[0]));
                }
            }
        }

        return edgesSpanned;
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

    //    List<Vector2> points = new List<Vector2>();
    //    for (int i = 0; i < rooms.Count; i++)
    //        points.Add(rooms[i].GetPosCenter());
    //    List<Triangle> triangles = new List<Triangle> { Triangle.SuperTriangle(points, maxWidth, maxHeigth) };
    //    for (int i = 0; i < points.Count; i++)
    //    {
    //        triangles = AddVertex(points[i], i, triangles);
    //    }
    //    List<GraphEdge> roomConnections = new List<GraphEdge>();
    //    for (int i = 0; i < triangles.Count; i++)
    //    {
    //        GraphEdge connection1 = new GraphEdge(triangles[i].edges[0], triangles[i].edges[1]);
    //        GraphEdge connection2 = new GraphEdge(triangles[i].edges[1], triangles[i].edges[2]);
    //        GraphEdge connection3 = new GraphEdge(triangles[i].edges[2], triangles[i].edges[0]);
    //        if (connection1.idRoomFirst > -1 && connection1.idRoomSecond > -1)
    //        {
    //            connection1.SetPoses(vertices[connection1.idRoomFirst].GetPosCenter(), vertices[connection1.idRoomSecond].GetPosCenter());
    //            roomConnections.Add(connection1);
    //        }
    //        if (connection2.idRoomFirst > -1 && connection2.idRoomSecond > -1)
    //        {
    //            connection2.SetPoses(vertices[connection2.idRoomFirst].GetPosCenter(), vertices[connection2.idRoomSecond].GetPosCenter());
    //            roomConnections.Add(connection2);
    //        }
    //        if (connection3.idRoomFirst > -1 && connection3.idRoomSecond > -1)
    //        {
    //            connection3.SetPoses(vertices[connection3.idRoomFirst].GetPosCenter(), vertices[connection3.idRoomSecond].GetPosCenter());
    //            roomConnections.Add(connection3);
    //        }
    //    };
    //    return roomConnections;
    //}
    //private List<Triangle> AddVertex(Vector2 vertex, int roomId, List<Triangle> triangles)
    //{
    //    List<GraphEdge> edges = new List<GraphEdge>();
    //    List<Triangle> badTriangles = new List<Triangle>();
    //    for (int i = 0; i < triangles.Count; i++)
    //    {
    //        if (triangles[i].circle.radius != -1)
    //        {
    //            if (triangles[i].InCircle(vertex))
    //            {
    //                badTriangles.Add(triangles[i]);
    //            }
    //        }
    //    }
    //    foreach (Triangle badTriangle in badTriangles)
    //    {
    //        edges.Add(new GraphEdge(badTriangle.edges[0], badTriangle.edges[1], badTriangle.posPointFirst, badTriangle.posPointSecond));
    //        edges.Add(new GraphEdge(badTriangle.edges[1], badTriangle.edges[2], badTriangle.posPointSecond, badTriangle.posPointThird));
    //        edges.Add(new GraphEdge(badTriangle.edges[2], badTriangle.edges[0], badTriangle.posPointThird, badTriangle.posPointFirst));
    //        triangles.Remove(badTriangle);
    //    }
    //    List<GraphEdge> uniqueEdges = new List<GraphEdge>();
    //    for (int i = 0; i < edges.Count; i++)
    //    {
    //        bool isUnique = true;
    //        for (int j = 0; j < edges.Count; j++)
    //        {
    //            if (i != j && edges[i] == edges[j])
    //            {
    //                isUnique = false;
    //                break;
    //            }
    //        }
    //        if (isUnique)
    //            uniqueEdges.Add(edges[i]);
    //    }
    //    foreach (GraphEdge edge in uniqueEdges)
    //    {
    //        triangles.Add(new Triangle(edge.posPointFirst, edge.posPointSecond, vertex, new int[3] { edge.idRoomFirst, edge.idRoomSecond, roomId }));
    //    }
    //    return triangles;
    //}
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

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> heap = new List<T>();

    public int Count => heap.Count;

    public void Enqueue(T item)
    {
        heap.Add(item);
        int i = heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent].CompareTo(heap[i]) <= 0)
                break;

            Swap(parent, i);
            i = parent;
        }
    }

    public T Dequeue()
    {
        int lastIndex = heap.Count - 1;
        T frontItem = heap[0];
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);

        --lastIndex;
        int parent = 0;
        while (true)
        {
            int leftChild = parent * 2 + 1;
            if (leftChild > lastIndex)
                break;

            int rightChild = leftChild + 1;
            if (rightChild <= lastIndex && heap[leftChild].CompareTo(heap[rightChild]) > 0)
                leftChild = rightChild;

            if (heap[parent].CompareTo(heap[leftChild]) <= 0)
                break;

            Swap(parent, leftChild);
            parent = leftChild;
        }

        return frontItem;
    }

    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }
}
