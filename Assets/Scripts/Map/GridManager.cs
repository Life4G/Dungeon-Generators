using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    //Управление текущим стилем
    [SerializeField]
    private RoomStyleManager roomStyleManager;
    private RoomStyle currentRoomStyle;
    
    //Генератор данжей который юзаем
    [SerializeField]
    public GeneratorBase generator;
    //Генератор стен
    [SerializeField]
    public WallsGenerator wallsGenerator;

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

    public void PaintWalls(IEnumerable<Vector2Int> wallPositionsEnumerable)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>(wallPositionsEnumerable);
        foreach (Vector2Int pos in wallPositions)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            TileBase tileToUse = DetermineTiles(pos, wallPositions);
            if (tileToUse != null)
            {
                currentRoomStyle.styleTilemap.SetTile(tilePosition, tileToUse);
            }
        }
    }

    private TileBase DetermineTiles(Vector2Int position, HashSet<Vector2Int> wallPositions)
    {
        bool top = wallPositions.Contains(position + Vector2Int.up);
        bool bottom = wallPositions.Contains(position + Vector2Int.down);
        bool left = wallPositions.Contains(position + Vector2Int.left);
        bool right = wallPositions.Contains(position + Vector2Int.right);

        // Углы
        if (!top && !left) return currentRoomStyle.topLeftCornerTile;
        if (!top && !right) return currentRoomStyle.topRightCornerTile;
        if (!bottom && !left) return currentRoomStyle.bottomLeftCornerTile;
        if (!bottom && !right) return currentRoomStyle.bottomRightCornerTile;

        // Стороны
        if (top && bottom && !left) return currentRoomStyle.leftWallTile;
        if (top && bottom && !right) return currentRoomStyle.rightWallTile;
        if (left && right && !top) return currentRoomStyle.topWallTile;
        if (left && right && !bottom) return currentRoomStyle.bottomWallTile;

        // По умолчанию
        return currentRoomStyle.floorTile;
    }

    //Передаем позиции
    public void PaintTiles(IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> wallPositions)
    {
        PaintTiles(floorPositions, currentRoomStyle.styleTilemap, currentRoomStyle.floorTile);
        PaintWalls(wallPositions);
    }

    //Закрашиваем каждый тайл
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }
    //Закраска
    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        //Вектор3 т.к. по факту мир 3D хоть у нас и 2D (живите с этим :| )
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    //Очистка тайлов
    public void Clear()
    {
        currentRoomStyle.styleTilemap.ClearAllTiles();
    }

    //Генерим карту заново
    public void Reload(int seed)
    {
        SetRandomRoomStyle();

        Clear();
        var floorPositions = generator.CreateDungeon(seed);
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }
    public void Reload()
    {
        SetRandomRoomStyle();

        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }

    //Генерим карту первый раз
    public void Start()
    {
        SetRandomRoomStyle();

        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }

}
