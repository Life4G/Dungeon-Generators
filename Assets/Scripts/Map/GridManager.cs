using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap;
    [SerializeField]
    private TileBase floorTile;
    [SerializeField]
    public GeneratorBase generator;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintFloorTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PaintFloorTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
    }

    public void Reload(int seed)
    {
        Clear();
        PaintFloorTiles(generator.CreateDungeon(seed));
    }
    public void Reload()
    {
        Clear();
        PaintFloorTiles(generator.CreateDungeon());
    }

    public void Start()
    {
        Clear();
        PaintFloorTiles(generator.CreateDungeon());
    }

}
