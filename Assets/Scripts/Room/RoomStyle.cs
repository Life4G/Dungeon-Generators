using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomStyle : MonoBehaviour
{
    public string         styleName;                  // имя стиля комнаты 
    public Tilemap        styleTilemap;
    public List<TileBase> floorTile;                  // тайл пола
    public List<TileBase> leftWallTile;               // тайл левой стены
    public List<TileBase> rightWallTile;              // тайл правой стены
    public List<TileBase> bottomWallTile;             // тайл нижней стены
    public List<TileBase> topWallTile;                // тайл верхней стены
    public List<TileBase> topLeftCornerTile;          // тайл левого верхнего угла
    public List<TileBase> topRightCornerTile;         // тайл правого верхнего угла
    public List<TileBase> bottomLeftCornerTile;       // тайл левого нижнего угла
    public List<TileBase> bottomRightCornerTile;      // тайл правого нижнего угла

    // конструктор по умолчанию
    public RoomStyle()
    {
        styleName = "";
        styleTilemap = null;
        floorTile = null;
        leftWallTile = null;
        rightWallTile = null;
        bottomWallTile = null;
        topWallTile = null;
        topLeftCornerTile = null;
        topRightCornerTile = null;
        bottomLeftCornerTile = null;
        bottomRightCornerTile = null;
    }

    // конструктор присваивания
    public RoomStyle(string name, Tilemap tilemap, List<TileBase> floor, List<TileBase> leftWall, List<TileBase> rightWall, List<TileBase> bottomWall, List<TileBase> topWall, List<TileBase> topLeftCorner, List<TileBase> topRightCorner, List<TileBase> bottomLeftCorner, List<TileBase> bottomRightCorner)
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
}
