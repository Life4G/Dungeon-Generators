using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class DungeonMap
    {
        public DungeonTile[,] tiles; // массив плиток подземелья

        /// <summary>
        /// Конструктор по размеру карты.
        /// </summary>
        /// <param name="width">Ширина карты.</param>
        /// <param name="height">Высота карты.</param>
        public DungeonMap(int width, int height)
        {
            this.tiles = new DungeonTile[width, height];
            InitializeTiles();
        }

        /// <summary>
        /// Инициализация всех плиток в массиве значением -1.
        /// </summary>
        private void InitializeTiles()
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[y, x] = new DungeonTile();    // roomIndex = -1
                }
            }
        }

        /// <summary>
        /// Вернуть тайл подземелья
        /// </summary>
        /// <param name="x">X координата тайла.</param>
        /// <param name="y">Y координата тайла.</param>
        /// <returns>Тайл.</returns>
        public DungeonTile GetTile(int x, int y)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return tiles[y, x];
            }
            return null; // если координаты вне карты
        }

        /// <summary>
        /// Получить высоту карты.
        /// </summary>
        /// <returns>Высота карты.</returns>
        /// 
        public int GetWidth()
        {
            return tiles.GetLength(0);
        }

        /// <summary>
        /// Получить ширинну карты.
        /// </summary>
        /// <returns>Ширина карты.</returns>
        public int GetHeight()
        {
            return tiles.GetLength(1);
        }

        /// <summary>
        /// Конструктор по массиву тайлов пола.
        /// </summary>
        /// <param name="roomIndices">Массив тайлов пола.</param>
        public DungeonMap(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);
            this.tiles = new DungeonTile[width, height];
            InitializeMapWithRoomIndices(roomIndices);
            AddWalls();
        }

        /// <summary>
        /// Инициализация карты массивом тайлов пола.
        /// </summary>
        /// <param name="roomIndices">Массив тайлов пола.</param>
        private void InitializeMapWithRoomIndices(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);

            tiles = new DungeonTile[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isAdjacentToWall = IsAdjacentToWall(roomIndices, x, y, width, height);
                    bool isCorner = IsCorner(roomIndices, x, y, width, height);
                    bool isPassable = roomIndices[y, x] != -1;

                    tiles[y, x] = new DungeonTile();
                    tiles[y, x].textureType = 0;    // пол
                    tiles[y, x].roomIndex = roomIndices[y, x];
                    tiles[y, x].hasAdjacentWall = isAdjacentToWall;
                    tiles[y, x].isCorner = isCorner;
                    tiles[y, x].isPassable = isPassable;
                }
            }
        }

        /// <summary>
        /// Проверка на нахождение у стены.
        /// </summary>
        /// <param name="roomIndices">Массив тайлов пола.</param>
        /// <param name="x">X координата тайла.</param>
        /// <param name="y">Y координата тайла.</param>
        /// <param name="width">Ширина  карты.</param>
        /// <param name="height">Высота карты.</param>
        /// <returns>True - тайл находится у стены. False - тайл не находится у стены.</returns>
        private bool IsAdjacentToWall(int[,] roomIndices, int y, int x, int width, int height)
        {
            return (x > 0 && roomIndices[x - 1, y] == -1) ||
                   (x < width - 1 && roomIndices[x + 1, y] == -1) ||
                   (y > 0 && roomIndices[x, y - 1] == -1) ||
                   (y < height - 1 && roomIndices[x, y + 1] == -1);
        }

        /// <summary>
        /// Проверка на угол.
        /// </summary>
        /// <param name="roomIndices">Массив тайлов пола.</param>
        /// <param name="x">X координата тайла.</param>
        /// <param name="y">Y координата тайла.</param>
        /// <param name="width">Ширина  карты.</param>
        /// <param name="height">Высота карты.</param>
        /// <returns>True - тайл угловой. False - тайл не угловой.</returns>
        private bool IsCorner(int[,] roomIndices, int y, int x, int width, int height)
        {
            return (x > 0 && y > 0 && roomIndices[x - 1, y - 1] == -1) ||
                   (x < width - 1 && y > 0 && roomIndices[x + 1, y - 1] == -1) ||
                   (x > 0 && y < height - 1 && roomIndices[x - 1, y + 1] == -1) ||
                   (x < width - 1 && y < height - 1 && roomIndices[x + 1, y + 1] == -1);
        }

        /// <summary>
        /// Обновление карты подземелья из массива тайлов пола.
        /// </summary>
        /// <param name="roomIndices">Массив тайлов пола.</param>
        public void UpdateMapWithRoomIndices(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool isAdjacentToWall = IsAdjacentToWall(roomIndices, x, y, width, height);
                    bool isCorner = IsCorner(roomIndices, x, y, width, height);
                    bool isPassable = roomIndices[y, x] != -1;

                    if (tiles[y, x] != null)
                    {
                        tiles[y, x].roomIndex = roomIndices[y, x];
                        tiles[y, x].hasAdjacentWall = isAdjacentToWall;
                        tiles[y, x].isCorner = isCorner;
                        tiles[y, x].isPassable = isPassable;
                    }
                }
            }
        }

        /// <summary>
        /// Добавить тайлы стен.
        /// </summary>
        public void AddWalls()
        {
            WallsGenerator wallsGenerator = new WallsGenerator();
            int[,] walls = wallsGenerator.GenerateWallsFromDungeonMap(this);

            int width = GetWidth();
            int height = GetHeight();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (walls[y, x] != -1)
                    { tiles[y, x].roomIndex = walls[y, x]; tiles[y, x].isPassable = false; }
                    else tiles[y, x].isPassable = true;
                    tiles[y, x].hasAdjacentWall = false;
                    tiles[y, x].isCorner = false;

                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (walls[y, x] != -1)
                    {
                        tiles[y, x].textureType = 0; // пол

                        bool top = y + 1 < height && walls[y + 1, x] == -1;
                        bool bottom = y - 1 >= 0 && walls[y - 1, x] == -1;
                        bool left = x - 1 >= 0 && walls[y, x - 1] == -1;
                        bool right = x + 1 < width && walls[y, x + 1] == -1;

                        if (left && top) tiles[y, x].textureType = 1;           // верхний левый угол
                        else if (right && top) tiles[y, x].textureType = 2;     // верхний правый угол
                        else if (left && bottom) tiles[y, x].textureType = 3;   // нижний левый угол
                        else if (right && bottom) tiles[y, x].textureType = 4;  // нижний правый угол

                        else if (left) tiles[y, x].textureType = 5;             // левая стена
                        else if (right) tiles[y, x].textureType = 6;            // правая стена
                        else if (top) tiles[y, x].textureType = 7;              // верхняя стена
                        else if (bottom) tiles[y, x].textureType = 8;           // нижняя стена
                    }

                }
            }

        }

        /// <summary>
        /// Проверка на наличие тайла по координатам.
        /// </summary>
        /// <param name="x">X координата тайла.</param>
        /// <param name="y">Y координата тайла.</param>
        /// <returns>True - тайл есть. False - тайл отсутствует.</returns>
        public bool HasTile(int x, int y)
        {
            if (x >= 0 && x < this.tiles.GetLength(0) && y >= 0 && y < this.tiles.GetLength(1))
            {
                return this.tiles[y, x].roomIndex >= 0;
            }
            return false;
        }

    }

}
