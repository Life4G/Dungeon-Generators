
using System;
using System.Collections.Generic;

public class MinimumSpanningTree
{
    // Function to find sum of weights of edges of the Minimum Spanning Tree.
    public static int SpanningTree(int vertexNum, int edgeNum, int[,] edges)
    {
        // Create an adjacency list representation of the graph
        List<List<int[]>> adjacencyList = new List<List<int[]>>();
        for (int i = 0; i < vertexNum; i++)
        {
            adjacencyList.Add(new List<int[]>());
        }

        // Fill the adjacency list with edges and their weights
        for (int i = 0; i < edgeNum; i++)
        {
            int u = edges[i, 0];
            int v = edges[i, 1];
            int wt = edges[i, 2];
            adjacencyList[u].Add(new int[] { v, wt });
            adjacencyList[v].Add(new int[] { u, wt });
        }

        // Create a priority queue to store edges with their weights
        PriorityQueue<(int, int)> pq = new PriorityQueue<(int, int)>();

        // Create a visited array to keep track of visited vertices
        bool[] visited = new bool[vertexNum];

        // Variable to store the result (sum of edge weights)
        int res = 0;

        // Start with vertex 0
        pq.Enqueue((0, 0));

        // Perform Prim's algorithm to find the Minimum Spanning Tree
        while (pq.Count > 0)
        {
            var p = pq.Dequeue();
            int wt = p.Item1;  // Weight of the edge
            int u = p.Item2;  // Vertex connected to the edge

            if (visited[u])
            {
                continue;  // Skip if the vertex is already visited
            }

            res += wt;  // Add the edge weight to the result
            visited[u] = true;  // Mark the vertex as visited

            // Explore the adjacent vertices
            foreach (var v in adjacencyList[u])
            {
                // v[0] represents the vertex and v[1] represents the edge weight
                if (!visited[v[0]])
                {
                    pq.Enqueue((v[1], v[0]));  // Add the adjacent edge to the priority queue
                }
            }
        }

        return res;  // Return the sum of edge weights of the Minimum Spanning Tree
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