using System;
using System.Collections.Generic;

public class Graph
{
    private List<Room> vertices;
    private List<GraphEdge> edges;

    public Graph(List<Room> rooms, List<RoomConnection> connections)
    {
        vertices = new List<Room>();
        edges = new List<GraphEdge>();
        foreach (Room room in rooms)
            vertices.Add(room);
        foreach (RoomConnection connection in connections)
        {
            GraphEdge edge = new GraphEdge(connection.roomFirst, connection.roomSecond);
            if (!edges.Contains(edge))
            edges.Add(edge);
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
    public Tuple<int,int> GetConnectedRooms(int id)
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
    private class GraphEdge
    {
        private int idRoomFirst;
        private int idRoomSecond;

        public GraphEdge(int idFirst, int idSecond)
        {
            this.idRoomFirst = idFirst;
            this.idRoomSecond = idSecond;
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
            return new Tuple<int,int>(idRoomFirst, idRoomSecond);
        }
    }
}