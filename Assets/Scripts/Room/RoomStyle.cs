using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomStyle : MonoBehaviour
{
    public string         styleName;                  // имя стиля комнаты 

    public List<TileBase> floorTile;                  // тайл пола
    public List<TileBase> leftFloorTile;              // тайл левого пола
    public List<TileBase> topFloorTile;               // тайл верхнего пола
    public List<TileBase> rightFloorTile;             // тайл правого пола
    public List<TileBase> bottomFloorTile;            // тайл нижнего пола
    public List<TileBase> topLeftFloorTile;           // тайл верхнего левого угла пола
    public List<TileBase> topRightFloorTile;          // тайл верхнего правого угла пола
    public List<TileBase> bottomLeftFloorTile;        // тайл нижнего левого угла пола
    public List<TileBase> bottomRightFloorTile;       // тайл нижнего правого угла пола


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

        floorTile = null;
        leftFloorTile = null;
        topFloorTile = null;
        rightFloorTile = null;
        bottomFloorTile = null;
        topLeftFloorTile = null;
        topRightFloorTile = null;
        bottomLeftFloorTile = null;
        bottomRightFloorTile = null;

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
    public RoomStyle(string name, List<TileBase> floor, List<TileBase> leftWall, List<TileBase> rightWall, List<TileBase> bottomWall, List<TileBase> topWall,
            List<TileBase> topLeftCorner, List<TileBase> topRightCorner, List<TileBase> bottomLeftCorner, List<TileBase> bottomRightCorner, List<TileBase> leftFloor,
            List<TileBase> topFloor, List<TileBase> rightFloor, List<TileBase> bottomFloor, List<TileBase> topLeftFloor, List<TileBase> topRightFloor,
            List<TileBase> bottomLeftFloor, List<TileBase> bottomRightFloor)
    {
        styleName = name;

        floorTile = floor;
        leftFloorTile = leftFloor;
        topFloorTile = topFloor;
        rightFloorTile = rightFloor;
        bottomFloorTile = bottomFloor;
        topLeftFloorTile = topLeftFloor;
        topRightFloorTile = topRightFloor;
        bottomLeftFloorTile = bottomLeftFloor;
        bottomRightFloorTile = bottomRightFloor;

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
