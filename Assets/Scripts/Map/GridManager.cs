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
    
    //Тайловая карта стен
    [SerializeField]
    private Tilemap wallTilemap;
    //Тайл которым будем закрашивать стены
    [SerializeField]
    private TileBase wallTile;
    
    //Генератор данжей который юзаем
    [SerializeField]
    public GeneratorBase generator;
    //Генератор стен
    [SerializeField]
    public WallsGenerator wallsGenerator;

    //Передаем позиции
    public void PaintTiles(IEnumerable<Vector2Int> floorPositions, IEnumerable<Vector2Int> wallPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
        PaintTiles(wallPositions, wallTilemap, wallTile);
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
        floorTilemap.ClearAllTiles();
    }

    //Генерим карту заново
    public void Reload(int seed)
    {
        Clear();
        var floorPositions = generator.CreateDungeon(seed);
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }
    public void Reload()
    {
        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }

    //Генерим карту первый раз
    public void Start()
    {
        Clear();
        var floorPositions = generator.CreateDungeon();
        //Генерирует позиции стен на основе позиций пола
        var wallPositions = wallsGenerator.CreateWalls(floorPositions);
        PaintTiles(floorPositions, wallPositions);
    }

}
