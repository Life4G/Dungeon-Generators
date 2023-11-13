using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator : GeneratorBase
{
    [SerializeField]
    private int radius = 50;
    [SerializeField]
    private int roomNumberMin = 10;
    [SerializeField]
    private int roomNumberMax = 32;

    private List<RoomBase> rooms;
    protected override HashSet<Vector2Int> GenerateDungeon()
    {
        return Run();
    }
    protected HashSet<Vector2Int> Run()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        //Rewrite this c++ shit blyat
        rooms = new List<RoomBase>();
        int roomNumber = Random.Range(roomNumberMin, roomNumberMax);
        for (int i = 0; i < roomNumber; i++)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    rooms.Add(GenerateCircleRoom());
                    break;
                case 1:
                    rooms.Add(GenerateSquareRoom());
                    break;
            }
            
        }
        for (int i = 0; i < rooms.Count - 1; i++)
            for (int j = i + 1; j < rooms.Count; j++)
            {
                   
                if (rooms[i].CheckIntersection(rooms[j]))
                {
                    //You know whats wrong - rewrite a fucking do oper function and don't fuck my brains bitch
                    if (rooms[i].IsProperSubsetOf(rooms[j]))
                    {
                        switch (Random.Range(0, 2))
                            {
                                case 0:
                                    rooms[i].Union(rooms[j]);
                                    break;
                                case 1:
                                    rooms[i].Difference(rooms[j]);
                                    break;
                            }
                        
                        rooms.Remove(rooms[j]);
                    }
                    else
                    {
                        switch (Random.Range(0, 4))
                            {
                                case 0:
                                    rooms[i].Intersect(rooms[j]);
                                    break;
                                case 1:
                                    rooms[i].Union(rooms[j]);
                                    break;
                                case 2:
                                    rooms[i].Difference(rooms[j]);
                                    break;
                                case 3:
                                    rooms[i].SymmetricDifference(rooms[j]);
                                    break;
                            }

                        rooms.Remove(rooms[j]);
                    }
                }
            }
        for (int i = 0; i < rooms.Count; i++)
        {
            floorPositions.UnionWith(rooms[i].GetTilesPos());
        }
        return floorPositions;
    }
    protected SquareRoom GenerateSquareRoom()
    {
        SquareRoom room = new SquareRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        int roomWidth = Random.Range(room.widthMin, room.widthMax);
        int roomHeight = Random.Range(room.heighthMin, room.heighthMax);
        room.SetSize(roomWidth, roomHeight);

        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        int roomX = roomCenterPos.x - roomWidth / 2;
        int roomY = roomCenterPos.y - roomHeight / 2;
        for (int x = roomX; x < roomX + roomWidth; x++)
            for (int y = roomY; y < roomY + roomHeight; y++)
                tilePositions.Add(new Vector2Int(x, y));
        room.SetTilesPos(tilePositions);

        room.SetStyle(Styles.Style1); //Temp Shit just let it be there
        return room;
    }
    protected CircleRoom GenerateCircleRoom()
    {
        CircleRoom room = new CircleRoom();
        Vector2Int roomCenterPos = CalculateRoomPos();
        room.SetPos(roomCenterPos);

        int roomRadius = Random.Range(room.radiusMin, room.radiusMax);
        room.SetSize(roomRadius);

        HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
        for (int y = -roomRadius; y < roomRadius; y++)
        {
            int half_row_width = Mathf.RoundToInt(Mathf.Sqrt(roomRadius * roomRadius - y * y));
            for (int x = -half_row_width; x < half_row_width; x++)
                tilePositions.Add(new Vector2Int(roomCenterPos.x + x, roomCenterPos.y + y));
        }
        room.SetTilesPos(tilePositions);

        room.SetStyle(Styles.Style1); //Temp Shit just let it be there
        return room;
    }
    private Vector2Int CalculateRoomPos()
    {
        float t = 2 * Mathf.PI * Random.Range(0f, 0.9f);
        float u = Random.Range(0f, 0.9f) + Random.Range(0f, 0.9f);
        float r;
        if (u > 1)
            r = 2 - u;
        else
            r = u;
        return new Vector2Int(Mathf.RoundToInt(radius * r * Mathf.Cos(t)), Mathf.RoundToInt(radius * r * Mathf.Sin(t)));
    }
}