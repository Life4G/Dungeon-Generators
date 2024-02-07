using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomStyle : MonoBehaviour
{
    public string   styleName;                  // имя стиля комнаты 
    public Tilemap  styleTilemap;
    public TileBase floorTile;                  // тайл пола
    public TileBase leftWallTile;               // тайл левой стены
    public TileBase rightWallTile;              // тайл правой стены
    public TileBase bottomWallTile;             // тайл нижней стены
    public TileBase topWallTile;                // тайл верхней стены
    public TileBase topLeftCornerTile;          // тайл левого верхнего угла
    public TileBase topRightCornerTile;         // тайл правого верхнего угла
    public TileBase bottomLeftCornerTile;       // тайл левого нижнего угла
    public TileBase bottomRightCornerTile;      // тайл правого нижнего угла

    public RoomStyle(string name, Tilemap tilemap, TileBase floor, TileBase leftWall, TileBase rightWall, TileBase bottomWall, TileBase topWall, TileBase topLeftCorner, TileBase topRightCorner, TileBase bottomLeftCorner, TileBase bottomRightCorner)
    {
        styleName = name;
        styleTilemap = tilemap;
        floorTile = floor;
        leftWallTile = leftWall;
        rightWallTile = rightWall;
        bottomWallTile = bottomWall;
        topWallTile = topWall;
        topLeftCornerTile = topLeftCorner;
        topRightCornerTile = topRightCorner;
        bottomLeftCornerTile = bottomLeftCorner;
        bottomRightCornerTile = bottomRightCorner;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
