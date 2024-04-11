using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using Assets.Scripts.Map;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private RoomStyleManager roomStyleManager;

    [SerializeField]
    public DungeonGeneratorBase generator;

    [SerializeField]
    public WallsGenerator wallsGenerator;

    [SerializeField]
    public Tilemap tilemap;

    private RoomStyle currentRoomStyle;
    private DungeonRoomManager roomManager;
    private DungeonMap map;

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
                if (roomIndex >= 0)
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(roomIndex));
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = isWall ? DetermineWallTile(x, y, array, rand) : currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                    tilemap.SetTile(tilePosition, tileToUse);
                }
            }
        }
    }

    private void PaintFromDungeonMap(DungeonMap dungeonMap)
    {
        System.Random rand = new System.Random();
        for (int y = 0; y < dungeonMap.GetHeight(); y++)
        {
            for (int x = 0; x < dungeonMap.GetWidth(); x++)
            {
                int textureType = dungeonMap.tiles[x, y].textureType;
                if (dungeonMap.tiles[x, y].roomIndex >= 0)
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(dungeonMap.tiles[x, y].roomIndex));
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = null;

                    switch (textureType)
                    {
                        case 1:
                            tileToUse = currentRoomStyle.topLeftCornerTile[rand.Next(currentRoomStyle.topLeftCornerTile.Count)];
                            break;
                        case 2:
                            tileToUse = currentRoomStyle.topRightCornerTile[rand.Next(currentRoomStyle.topRightCornerTile.Count)];
                            break;
                        case 3:
                            tileToUse = currentRoomStyle.bottomLeftCornerTile[rand.Next(currentRoomStyle.bottomLeftCornerTile.Count)];
                            break;
                        case 4:
                            tileToUse = currentRoomStyle.bottomRightCornerTile[rand.Next(currentRoomStyle.bottomRightCornerTile.Count)];
                            break;
                        case 5:
                            tileToUse = currentRoomStyle.leftWallTile[rand.Next(currentRoomStyle.leftWallTile.Count)];
                            break;
                        case 6:
                            tileToUse = currentRoomStyle.rightWallTile[rand.Next(currentRoomStyle.rightWallTile.Count)];
                            break;
                        case 7:
                            tileToUse = currentRoomStyle.topWallTile[rand.Next(currentRoomStyle.topWallTile.Count)];
                            break;
                        case 8:
                            tileToUse = currentRoomStyle.bottomWallTile[rand.Next(currentRoomStyle.bottomWallTile.Count)];
                            break;
                        default:
                            tileToUse = currentRoomStyle.floorTile[rand.Next(currentRoomStyle.floorTile.Count)];
                            break;
                    }
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

        if (!top && !left) possibleTiles = currentRoomStyle.topLeftCornerTile;
        else if (!top && !right) possibleTiles = currentRoomStyle.topRightCornerTile;
        else if (!bottom && !left) possibleTiles = currentRoomStyle.bottomLeftCornerTile;
        else if (!bottom && !right) possibleTiles = currentRoomStyle.bottomRightCornerTile;

        else if (top && bottom && !left) possibleTiles = currentRoomStyle.leftWallTile;
        else if (top && bottom && !right) possibleTiles = currentRoomStyle.rightWallTile;
        else if (left && right && !top) possibleTiles = currentRoomStyle.topWallTile;
        else if (left && right && !bottom) possibleTiles = currentRoomStyle.bottomWallTile;

        return possibleTiles.Any() ? possibleTiles[rand.Next(possibleTiles.Count)] : null;
    }

    public void Clear()
    {
        tilemap.ClearAllTiles();
    }

    public void Reload()
    {
        Clear();

        map = new DungeonMap(generator.CreateDungeon());

        roomManager = new DungeonRoomManager(map);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.PrintRoomsInfo();
        roomManager.ClearRoomsInfoFromMap();
        roomManager.DisplayRoomsInfoOnMap();

        PaintFromDungeonMap(map);

        //----------------------------

    }

    public void Reload(int seed)
    {
        Clear();

        map = new DungeonMap(generator.CreateDungeon(seed));

        roomManager = new DungeonRoomManager(map);
        roomManager.AssignRandomStylesToRooms(roomStyleManager);
        roomManager.PrintRoomsInfo();
        roomManager.ClearRoomsInfoFromMap();
        roomManager.DisplayRoomsInfoOnMap();

        PaintFromDungeonMap(map);
    }

    void Start()
    {
        Reload();
    }
}