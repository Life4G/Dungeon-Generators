using System;
using System.Collections.Generic;
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
    public List<GraphEdge> SpanningTree(List<GraphEdge> edges)
    {
        // Create an adjacency list representation of the graph
        List<List<int[]>> adj = new List<List<int[]>>();
        List<GraphEdge> edgesSpanned = new List<GraphEdge>();
        for (int i = 0; i < vertices.Count; i++)
        {
            adj.Add(new List<int[]>());
        }

        // Fill the adjacency list with edges and their weights
        for (int i = 0; i < edges.Count; i++)
        {
            int u = edges[i].idRoomFirst;
            int v = edges[i].idRoomSecond;
            int wt = edges[i].GetLength();
            adj[u].Add(new int[] { v, wt });
            adj[v].Add(new int[] { u, wt });
        }

        // Create a priority queue to store edges with their weights
        PriorityQueue<(int, int)> pq = new PriorityQueue<(int, int)>();

        // Create a visited array to keep track of visited vertices
        bool[] visited = new bool[vertices.Count];

        // Variable to store the result (sum of edge weights)
        int res = 0;

        // Start with vertex 0
        pq.Enqueue((0, 0));
        int prev = 0;

        // Perform Prim's algorithm to find the Minimum Spanning Tree
        while (pq.Count > 0)
        {
            var p = pq.Dequeue();
            int wt = p.Item1; // Weight of the edge
            int u = p.Item2; // Vertex connected to the edge

            if (visited[u])
            {
                continue; // Skip if the vertex is already visited
            }

            res += wt; // Add the edge weight to the result
            edgesSpanned.Add(new GraphEdge(prev, u, wt));
            visited[u] = true; // Mark the vertex as visited

            // Explore the adjacent vertices
            foreach (var v in adj[u])
            {
                // v[0] represents the vertex and v[1] represents the edge weight
                if (!visited[v[0]])
                {
                    pq.Enqueue((v[1], v[0])); // Add the adjacent edge to the priority queue
                }
            }
        }

        return edgesSpanned; // Return the sum of edge weights of the Minimum Spanning Tree
    }
    private List<GraphEdge> Triangulation(List<Room> rooms, int maxWidth, int maxHeigth)
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < rooms.Count; i++)
            points.Add(rooms[i].GetPosCenter());

        List<Triangle> triangles = new List<Triangle> { Triangle.SuperTriangle(points, maxWidth, maxHeigth) };

        for (int i = 0; i < points.Count; i++)
        {
            triangles = AddVertex(points[i], i, triangles);
        }
        List<GraphEdge> roomConnections = new List<GraphEdge>();
        for (int i = 0; i < triangles.Count; i++)
        {

            GraphEdge connection1 = new GraphEdge(triangles[i].edges[0], triangles[i].edges[1]);
            GraphEdge connection2 = new GraphEdge(triangles[i].edges[1], triangles[i].edges[2]);
            GraphEdge connection3 = new GraphEdge(triangles[i].edges[2], triangles[i].edges[0]);

            if (connection1.idRoomFirst > -1 && connection1.idRoomSecond > -1)
            {
                connection1.SetPoses(vertices[connection1.idRoomFirst].GetPosCenter(), vertices[connection1.idRoomSecond].GetPosCenter());
                roomConnections.Add(connection1);
            }
            if (connection2.idRoomFirst > -1 && connection2.idRoomSecond > -1)
            {
                connection2.SetPoses(vertices[connection2.idRoomFirst].GetPosCenter(), vertices[connection2.idRoomSecond].GetPosCenter());
                roomConnections.Add(connection2);
            }
            if (connection3.idRoomFirst > -1 && connection3.idRoomSecond > -1)
            {
                connection3.SetPoses(vertices[connection3.idRoomFirst].GetPosCenter(), vertices[connection3.idRoomSecond].GetPosCenter());
                roomConnections.Add(connection3);
            }

        };
        return roomConnections;
    }
    private List<Triangle> AddVertex(Vector2 vertex, int roomId, List<Triangle> triangles)
    {
        List<GraphEdge> edges = new List<GraphEdge>();
        List<Triangle> badTriangles = new List<Triangle>();
        for (int i = 0; i < triangles.Count; i++)
        {
            if (triangles[i].circle.radius != -1)
            {
                if (triangles[i].InCircle(vertex))
                {
                    badTriangles.Add(triangles[i]);
                }
            }
        }
        foreach (Triangle badTriangle in badTriangles)
        {
            edges.Add(new GraphEdge(badTriangle.edges[0], badTriangle.edges[1], badTriangle.posPointFirst, badTriangle.posPointSecond));
            edges.Add(new GraphEdge(badTriangle.edges[1], badTriangle.edges[2], badTriangle.posPointSecond, badTriangle.posPointThird));
            edges.Add(new GraphEdge(badTriangle.edges[2], badTriangle.edges[0], badTriangle.posPointThird, badTriangle.posPointFirst));
            triangles.Remove(badTriangle);
        }
        List<GraphEdge> uniqueEdges = new List<GraphEdge>();

        for (int i = 0; i < edges.Count; i++)
        {
            bool isUnique = true;
            for (int j = 0; j < edges.Count; j++)
            {
                if (i != j && edges[i] == edges[j])
                {
                    isUnique = false;
                    break;
                }
            }
            if (isUnique)
                uniqueEdges.Add(edges[i]);
        }

        foreach (GraphEdge edge in uniqueEdges)
        {
            triangles.Add(new Triangle(edge.posPointFirst, edge.posPointSecond, vertex, new int[3] { edge.idRoomFirst, edge.idRoomSecond, roomId }));
        }
        return triangles;
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
    public GraphEdge(int idFirst, int idSecond, int length)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
        this.length = length;
    }
    public GraphEdge(int idFirst, int idSecond, Vector2 posPointFirst, Vector2 posPointSecond)
    {
        this.idRoomFirst = idFirst;
        this.idRoomSecond = idSecond;
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
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
    public int[] edges;
    public Vector2 posPointFirst;
    public Vector2 posPointSecond;
    public Vector2 posPointThird;
    public CircumCircle circle;

    public Triangle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird, int[] edgesId)
    {
        this.posPointFirst = posPointFirst;
        this.posPointSecond = posPointSecond;
        this.posPointThird = posPointThird;
        circle = new CircumCircle(posPointFirst, posPointSecond, posPointThird);
        edges = edgesId;
    }
    public static Triangle SuperTriangle(List<Vector2> pointsList, int maxWidth, int maxHeigth)
    {
        int offsetX = maxWidth * 4;
        int offsetY = maxHeigth * 4;
        return new Triangle(new Vector2(0 - offsetX * 2, 0 - offsetY * 2), new Vector2(maxWidth + offsetX, 0 - offsetY * 2), new Vector2(maxWidth + offsetX, maxHeigth + offsetY), new int[3] { -1, -2, -3 });
    }
    public bool InCircle(Vector2 vertex)
    {
        if ((vertex.x - circle.center.x) * (vertex.x - circle.center.x) + (vertex.y - circle.center.y) * (vertex.y - circle.center.y) <= circle.radius * circle.radius)
            return true;
        else
            return false;
    }
}

public class CircumCircle
{
    public Vector2 center;
    public float radius;
    public CircumCircle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
    {
        if (!IsPerpendicular(posPointFirst, posPointSecond, posPointThird))
            getCircumCircle(posPointFirst, posPointSecond, posPointThird);
        else if (!IsPerpendicular(posPointFirst, posPointThird, posPointSecond))
            getCircumCircle(posPointFirst, posPointThird, posPointSecond);
        else if (!IsPerpendicular(posPointSecond, posPointFirst, posPointThird))
            getCircumCircle(posPointSecond, posPointFirst, posPointThird);
        else if (!IsPerpendicular(posPointSecond, posPointThird, posPointFirst))
            getCircumCircle(posPointSecond, posPointThird, posPointFirst);
        else if (!IsPerpendicular(posPointThird, posPointSecond, posPointFirst))
            getCircumCircle(posPointThird, posPointSecond, posPointFirst);
        else if (!IsPerpendicular(posPointThird, posPointFirst, posPointSecond))
            getCircumCircle(posPointThird, posPointFirst, posPointSecond);
        else
        {
            //The three points are perpendicular to axis
            radius = -1;
        }
    }
    private bool IsPerpendicular(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
    {
        float deltaYSecondFirst = Math.Abs(posPointSecond.y - posPointFirst.y);
        float deltaXSecondFirst = Math.Abs(posPointSecond.x - posPointFirst.x);
        float deltaYThirdSecond = Math.Abs(posPointThird.y - posPointSecond.y);
        float deltaXThirdSecond = Math.Abs(posPointThird.x - posPointSecond.x);
        //The points are pependicular and parallel to x-y axis
        if (deltaXSecondFirst <= 0.0000001f && deltaYThirdSecond <= 0.0000001f)
            return false;
        //A line of two point are perpendicular to x - axis
        if (deltaYSecondFirst <= 0.0000001f)
            return true;
        //A line of two point are perpendicular to x - axis
        else if (deltaYThirdSecond <= 0.0000001f)
            return true;
        //A line of two point are perpendicular to y - axis
        else if (deltaXSecondFirst <= 0.0000001f)
            return true;
        //A line of two point are perpendicular to y - axis
        else if (deltaXThirdSecond <= 0.0000001f)
            return true;
        return false;
    }

    private void getCircumCircle(Vector2 posPointFirst, Vector2 posPointSecond, Vector2 posPointThird)
    {
        float deltaYSecondFirst = Math.Abs(posPointSecond.y - posPointFirst.y);
        float deltaXSecondFirst = Math.Abs(posPointSecond.x - posPointFirst.x);
        float deltaYThirdSecond = Math.Abs(posPointThird.y - posPointSecond.y);
        float deltaXThirdSecond = Math.Abs(posPointThird.x - posPointSecond.x);
        if (deltaXSecondFirst <= 0.0000001f && deltaYThirdSecond <= 0.0000001f)
        {
            center = new Vector2(posPointSecond.x + posPointThird.x, posPointFirst.y + posPointSecond.y) * 0.5f;
            radius = Math.Max(Math.Max(Vector2.Distance(center, posPointFirst), Vector2.Distance(center, posPointSecond)), Vector2.Distance(center, posPointThird));
            return;
        }
        float slopeFirst = deltaYSecondFirst / deltaXSecondFirst;
        float slopeSecond = deltaYThirdSecond / deltaXThirdSecond;
        if (Math.Abs(slopeFirst - slopeSecond) <= 0.0000001f)
        {
            radius = -1;
            return;
        }
        float x = (slopeFirst * slopeSecond * (posPointFirst.y - posPointThird.y) + slopeSecond * (posPointFirst.x + posPointSecond.x) - slopeFirst * (posPointSecond.x + posPointThird.x))
            / (2 * (slopeSecond - slopeFirst));
        float y = -1 * (x - (posPointFirst.x + posPointSecond.x) / 2) / slopeFirst + (posPointFirst.y + posPointSecond.y) / 2;
        center = new Vector2(x, y);
        float dx = center.x - posPointFirst.x;
        float dy = center.y - posPointFirst.y;
        radius = Mathf.Sqrt(dx * dx + dy * dy);
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
