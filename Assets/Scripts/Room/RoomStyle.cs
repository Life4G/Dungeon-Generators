using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GridManager;
using UnityEngine.Tilemaps;

[System.Serializable]
public class RoomStyle : MonoBehaviour
{
    /// <summary>
    /// Имя стиля комнаты.
    /// </summary>
    public string styleName;

    /// <summary>
    /// Тайл пола.
    /// </summary>
    public List<TileBase> floorTile;

    /// <summary>
    /// Тайл левого пола.
    /// </summary>
    public List<TileBase> leftFloorTile;

    /// <summary>
    /// Тайл верхнего пола.
    /// </summary>
    public List<TileBase> topFloorTile;

    /// <summary>
    /// Тайл правого пола.
    /// </summary>
    public List<TileBase> rightFloorTile;

    /// <summary>
    /// Тайл нижнего пола.
    /// </summary>
    public List<TileBase> bottomFloorTile;

    /// <summary>
    /// Тайл верхнего левого угла пола.
    /// </summary>
    public List<TileBase> topLeftFloorTile;

    /// <summary>
    /// Тайл верхнего правого угла пола.
    /// </summary>
    public List<TileBase> topRightFloorTile;

    /// <summary>
    /// Тайл нижнего левого угла пола.
    /// </summary>
    public List<TileBase> bottomLeftFloorTile;

    /// <summary>
    /// Тайл нижнего правого угла пола.
    /// </summary>
    public List<TileBase> bottomRightFloorTile;

    /// <summary>
    /// Тайл левой стены.
    /// </summary>
    public List<TileBase> leftWallTile;

    /// <summary>
    /// Тайл правой стены.
    /// </summary>
    public List<TileBase> rightWallTile;

    /// <summary>
    /// Тайл нижней стены.
    /// </summary>
    public List<TileBase> bottomWallTile;

    /// <summary>
    /// Тайл верхней стены.
    /// </summary>
    public List<TileBase> topWallTile;

    /// <summary>
    /// Тайл левого верхнего угла.
    /// </summary>
    public List<TileBase> topLeftCornerTile;

    /// <summary>
    /// Тайл правого верхнего угла.
    /// </summary>
    public List<TileBase> topRightCornerTile;

    /// <summary>
    /// Тайл левого нижнего угла.
    /// </summary>
    public List<TileBase> bottomLeftCornerTile;

    /// <summary>
    /// Тайл правого нижнего угла.
    /// </summary>
    public List<TileBase> bottomRightCornerTile;

    /// <summary>
    /// Конструктор по умолчанию.
    /// Инициализирует все поля списками тайлов и стилей как null.
    /// </summary>
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

    /// <summary>
    /// Конструктор присваивания для RoomStyle.
    /// </summary>
    /// <param name="name">Имя стиля комнаты.</param>
    /// <param name="floor">Список тайлов пола.</param>
    /// <param name="leftWall">Список тайлов левой стены.</param>
    /// <param name="rightWall">Список тайлов правой стены.</param>
    /// <param name="bottomWall">Список тайлов нижней стены.</param>
    /// <param name="topWall">Список тайлов верхней стены.</param>
    /// <param name="topLeftCorner">Список тайлов левого верхнего угла.</param>
    /// <param name="topRightCorner">Список тайлов правого верхнего угла.</param>
    /// <param name="bottomLeftCorner">Список тайлов левого нижнего угла.</param>
    /// <param name="bottomRightCorner">Список тайлов правого нижнего угла.</param>
    /// <param name="leftFloor">Список тайлов левого пола.</param>
    /// <param name="topFloor">Список тайлов верхнего пола.</param>
    /// <param name="rightFloor">Список тайлов правого пола.</param>
    /// <param name="bottomFloor">Список тайлов нижнего пола.</param>
    /// <param name="topLeftFloor">Список тайлов верхнего левого угла пола.</param>
    /// <param name="topRightFloor">Список тайлов верхнего правого угла пола.</param>
    /// <param name="bottomLeftFloor">Список тайлов нижнего левого угла пола.</param>
    /// <param name="bottomRightFloor">Список тайлов нижнего правого угла пола.</param>
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
