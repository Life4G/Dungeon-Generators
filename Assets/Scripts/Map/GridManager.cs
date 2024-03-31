using Assets.Scripts.Room;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
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
        for (int y = 0; y < array.GetLength(1); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                int roomIndex = array[x, y];
                if (roomIndex != -1)
                {
                    currentRoomStyle = roomStyleManager.GetRoomStyle(roomManager.GetRoomStyleId(roomIndex));    // определение стиля плитки/комнаты (текущего стиля отрисовки)
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tileToUse = isWall ? DetermineWallTile(x, y, array) : currentRoomStyle.floorTile;
                    tilemap.SetTile(tilePosition, tileToUse);
                }
            }
        }
    }

    private TileBase DetermineWallTile(int x, int y, int[,] array)
    {
        bool top = y + 1 < array.GetLength(1) && array[x, y + 1] != -1;
        bool bottom = y - 1 >= 0 && array[x, y - 1] != -1;
        bool left = x - 1 >= 0 && array[x - 1, y] != -1;
        bool right = x + 1 < array.GetLength(0) && array[x + 1, y] != -1;

        if (!top && !left) return currentRoomStyle.topLeftCornerTile;
        if (!top && !right) return currentRoomStyle.topRightCornerTile;
        if (!bottom && !left) return currentRoomStyle.bottomLeftCornerTile;
        if (!bottom && !right) return currentRoomStyle.bottomRightCornerTile;

        if (top && bottom && !left) return currentRoomStyle.leftWallTile;
        if (top && bottom && !right) return currentRoomStyle.rightWallTile;
        if (left && right && !top) return currentRoomStyle.topWallTile;
        if (left && right && !bottom) return currentRoomStyle.bottomWallTile;

        return null;
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

    void Awake()
    {
        Reload();
    }

}
