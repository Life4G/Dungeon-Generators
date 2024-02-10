using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    //Тайловая карта пола
    [SerializeField]
    private Tilemap floorTilemap;
    //Тайл которым будем закрашивать пол
    [SerializeField]
    private TileBase floorTile;

    //Генератор данжей который юзаем
    [SerializeField]
    public DungeonGeneratorBase generator;

    public Tilemap wallTilemap;

    [System.Serializable]
    public struct WallTile
    {
        public string name;
        public TileBase tile;
    }

    // Список стен с названиями
    public List<WallTile> wallTilesList;

    private Dictionary<string, TileBase> wallTilesDictionary;

    private void InitializeWallTilesDictionary()
    {
        wallTilesDictionary = new Dictionary<string, TileBase>();
        foreach (WallTile wallTile in wallTilesList)
        {
            if (!wallTilesDictionary.ContainsKey(wallTile.name))
            {
                wallTilesDictionary.Add(wallTile.name, wallTile.tile);
            }
            else
            {
                // Ошибка
            }
        }
    }

    public void PaintWalls(IEnumerable<Vector2Int> wallPositionsEnumerable)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>(wallPositionsEnumerable);
        foreach (Vector2Int pos in wallPositions)
        {
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            TileBase tileToUse = DetermineTileType(pos, wallPositions);
            if (tileToUse != null)
            {
                wallTilemap.SetTile(tilePosition, tileToUse);
            }
        }
    }

    private TileBase DetermineTileType(Vector2Int position, HashSet<Vector2Int> wallPositions)
    {
        bool top = wallPositions.Contains(position + Vector2Int.up);
        bool bottom = wallPositions.Contains(position + Vector2Int.down);
        bool left = wallPositions.Contains(position + Vector2Int.left);
        bool right = wallPositions.Contains(position + Vector2Int.right);

        if (top && bottom && left && right && wallTilesDictionary.TryGetValue("defaultWallTile", out TileBase tile)) return tile;

        // Углы
        if (!top && !left && wallTilesDictionary.TryGetValue("topLeftCorner", out tile)) return tile;
        if (!top && !right && wallTilesDictionary.TryGetValue("topRightCorner", out tile)) return tile;
        if (!bottom && !left && wallTilesDictionary.TryGetValue("bottomLeftCorner", out tile)) return tile;
        if (!bottom && !right && wallTilesDictionary.TryGetValue("bottomRightCorner", out tile)) return tile;

        // Стороны
        if (top && bottom && !left && !right && wallTilesDictionary.TryGetValue("rightWall", out tile)) return tile;
        if (top && bottom && !left && !right && wallTilesDictionary.TryGetValue("leftWall", out tile)) return tile;
        if (left && right && !top && !bottom && wallTilesDictionary.TryGetValue("bottomWall", out tile)) return tile;
        if (left && right && !top && !bottom && wallTilesDictionary.TryGetValue("topWall", out tile)) return tile;


        if (wallTilesDictionary.TryGetValue("defaultWallTile", out tile)) return tile;

        return null;
    }

    //Передаем позиции
    public void PaintTiles(int[,] floorPositions, IEnumerable<Vector2Int> wallPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
        PaintWalls(wallPositions);
    }

    //Закрашиваем каждый тайл
    private void PaintTiles(int[,] positions, Tilemap tilemap, TileBase tile)
    {
        for (int y = 0; y < positions.GetLength(0); y++)
        {
            for (int x = 0; x < positions.GetLength(1); x++)
                if (positions[y, x] != -1)
                    PaintSingleTile(tilemap, tile, new Vector2Int(x, y));
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
        floorTilemap.ClearAllTiles();
    }

    //Генерим карту заново
    public void Reload(int seed)
    {
        Clear();
        var floorPositions = generator.CreateDungeon(seed);
        //Генерирует позиции стен на основе позиций пола
        //var wallPositions = generator.wallsGenerator.CreateWalls(floorPositions);
        //PaintTiles(floorPositions, wallPositions);
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }
    public void Reload()
    {
        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        //var wallPositions = generator.wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    //Генерим карту первый раз
    public void Start()
    {
        InitializeWallTilesDictionary();

        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        //var wallPositions = generator.wallsGenerator.CreateWalls(floorPositions);
        //PaintTiles(floorPositions, wallPositions);
        PaintTiles(floorPositions, floorTilemap, floorTile);

    }

}
