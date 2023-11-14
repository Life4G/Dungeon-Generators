using System.Collections.Generic; using UnityEngine; using UnityEngine.Tilemaps;  public class GridManager : MonoBehaviour {
    //Наша тайловая карта     [SerializeField]     private Tilemap floorTilemap;
    //Тайл которым будем все закрашивать (потом мы логику этого перепишем)     [SerializeField]     private TileBase floorTile;
    //Генератор данжей который юзаем     [SerializeField]     public GeneratorBase generator; 
    //Передаем позиции     public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)     {         PaintFloorTiles(floorPositions, floorTilemap, floorTile);     } 
    //Закрашиваем каждый тайл     private void PaintFloorTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)     {         foreach (var position in positions)         {             PaintSingleTile(tilemap, tile, position);         }     }     //Закраска     private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)     {
        //Вектор3 т.к. по факту мир 3D хоть у нас и 2D (живите с этим :| )         var tilePosition = tilemap.WorldToCell((Vector3Int)position);         tilemap.SetTile(tilePosition, tile);     } 
    //Очистка тайлов     public void Clear()     {         floorTilemap.ClearAllTiles();     } 
    //Генерим карту заново     public void Reload(int seed)     {         Clear();         PaintFloorTiles(generator.CreateDungeon(seed));     }     public void Reload()     {         Clear();         PaintFloorTiles(generator.CreateDungeon());     } 
    //Генерим карту первый раз     public void Start()     {         Clear();         PaintFloorTiles(generator.CreateDungeon());     }  } 