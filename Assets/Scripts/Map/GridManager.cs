using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;


public class GridManager : MonoBehaviour
{
    [SerializeField]
    private RoomStyleManager roomStyleManager;
    private RoomStyle currentRoomStyle;

    private DungeonRoomManager roomManager;

    [SerializeField]
    public DungeonGeneratorBase generator;

    [SerializeField]
    public WallsGenerator wallsGenerator;

    [SerializeField]
    public Tilemap tilemap;

    public void SetRoomStyle(string styleName)
    {
        currentRoomStyle = roomStyleManager.GetRoomStyle(styleName);
    }

    public void SetRoomStyle(int styleIndex)
    {
        currentRoomStyle = roomStyleManager.GetRoomStyle(styleIndex);
    }

    public void SetRandomRoomStyle()
    {
        SetRoomStyle(roomStyleManager.GetRandomStyleIndex());
    }

    public int GetStylesCount()
    {
        return roomStyleManager.GetStylesCount();
    }

    private void PaintFromArray(int[,] array, bool isWall)
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < array.GetLength(1); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int roomIndex = array[x, y];
                if (roomIndex != -1)
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(roomIndex));
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = isWall ? DetermineWallTile(x, y, array, rand) : currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                    tilemap.SetTile(tilePosition, tileToUse);
                }
            }
        }
    }

    private TileBase DetermineWallTile(int x, int y, int[,] array, System.Random rand)
    {
        bool top = y + 1 < array.GetLength(1) && array[x, y + 1] != -1;
        bool bottom = y - 1 >= 0 && array[x, y - 1] != -1;
        bool left = x - 1 >= 0 && array[x - 1, y] != -1;
        bool right = x + 1 < array.GetLength(0) && array[x + 1, y] != -1;

        List<TileBase> possibleTiles = new List<TileBase>();

        if (!top && !left)          possibleTiles = currentRoomStyle.topLeftCornerTile;
        else if (!top && !right)    possibleTiles = currentRoomStyle.topRightCornerTile;
        else if (!bottom && !left)  possibleTiles = currentRoomStyle.bottomLeftCornerTile;
        else if (!bottom && !right) possibleTiles = currentRoomStyle.bottomRightCornerTile;

        else if (top && bottom && !left)    possibleTiles = currentRoomStyle.leftWallTile;
        else if (top && bottom && !right)   possibleTiles = currentRoomStyle.rightWallTile;
        else if (left && right && !top)     possibleTiles = currentRoomStyle.topWallTile;
        else if (left && right && !bottom)  possibleTiles = currentRoomStyle.bottomWallTile;

        return possibleTiles.Any() ? possibleTiles[rand.Next(possibleTiles.Count)] : null;
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    public void Reload()
    {
        Clear();

        int[,] floorArray = generator.CreateDungeon();
        int[,] wallArray = wallsGenerator.GenerateWallsFromFloor(floorArray);

        roomManager = new DungeonRoomManager(floorArray);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.PrintRoomsInfo();

        PaintFromArray(floorArray, false);
        PaintFromArray(wallArray, true);
    }

    public void Reload(int seed)
    {
        Clear();

        int[,] floorArray = generator.CreateDungeon(seed);
        int[,] wallArray = wallsGenerator.GenerateWallsFromFloor(floorArray);

        roomManager = new DungeonRoomManager(floorArray);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.PrintRoomsInfo();

        PaintFromArray(floorArray, false);
        PaintFromArray(wallArray, true);
    }

    void Start()
    {
        Reload();
    }
}
