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

        // конструктор по размеру карты
        public DungeonMap(int width, int height)
        {
            this.tiles = new DungeonTile[width, height];
            InitializeTiles();
        }

        // инициализация всех плиток в массиве значением -1
        private void InitializeTiles()
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new DungeonTile();    // roomIndex = -1
                }
            }
        }

        // вернуть тайл подземелья
        public DungeonTile GetTile(int x, int y)
        {
            int width = tiles.GetLength(0);
            int height = tiles.GetLength(1);

            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return tiles[x, y];
            }
            return null; // если координаты вне карты
        }
        public int GetWidth()
        {
            return tiles.GetLength(0);
        }

        public int GetHeight()
        {
            return tiles.GetLength(1);
        }

        // конструктор по массиву тайлов пола
        public DungeonMap(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);
            this.tiles = new DungeonTile[width, height];
            InitializeMapWithRoomIndices(roomIndices);
            AddWalls();
        }

        // инициализация карты массивом тайлов пола
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
                    bool isPassable = roomIndices[x, y] != -1;

                    tiles[x, y] = new DungeonTile();
                    tiles[x, y].textureType = 0;    // пол
                    tiles[x, y].roomIndex = roomIndices[x, y];
                    tiles[x, y].hasAdjacentWall = isAdjacentToWall;
                    tiles[x, y].isCorner = isCorner;
                    tiles[x, y].isPassable = isPassable;
                }
            }
        }

        // проверка на нахождение у стены
        private bool IsAdjacentToWall(int[,] roomIndices, int x, int y, int width, int height)
        {
            return (x > 0 && roomIndices[x - 1, y] == -1) ||
                   (x < width - 1 && roomIndices[x + 1, y] == -1) ||
                   (y > 0 && roomIndices[x, y - 1] == -1) ||
                   (y < height - 1 && roomIndices[x, y + 1] == -1);
        }

        // проверка на угол
        private bool IsCorner(int[,] roomIndices, int x, int y, int width, int height)
        {
            return (x > 0 && y > 0 && roomIndices[x - 1, y - 1] == -1) ||
                   (x < width - 1 && y > 0 && roomIndices[x + 1, y - 1] == -1) ||
                   (x > 0 && y < height - 1 && roomIndices[x - 1, y + 1] == -1) ||
                   (x < width - 1 && y < height - 1 && roomIndices[x + 1, y + 1] == -1);
        }

        // обновление карты подземелья из массива тайлов пола
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
                    bool isPassable = roomIndices[x, y] != -1;

                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].roomIndex = roomIndices[x, y];
                        tiles[x, y].hasAdjacentWall = isAdjacentToWall;
                        tiles[x, y].isCorner = isCorner;
                        tiles[x, y].isPassable = isPassable;
                    }
                }
            }
        }

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
                    //tiles[x, y].roomIndex = FindAdjacentPassableTileRoomIndex(x, y);
                    if (walls[x, y] != -1) tiles[x, y].roomIndex = walls[x, y];
                    tiles[x, y].hasAdjacentWall = false;
                    tiles[x, y].isCorner = false;
                    tiles[x, y].isPassable = false;
                }
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bool top = y + 1 < height && walls[x, y] != -1;
                    bool bottom = y - 1 >= 0 && walls[x, y] != -1;
                    bool left = x - 1 >= 0 && walls[x, y] != -1;
                    bool right = x + 1 < width && walls[x, y] != -1;

                    if (!top && !left) tiles[x, y].textureType = 1;
                    else if (!top && !right) tiles[x, y].textureType = 2;
                    else if (!bottom && !left) tiles[x, y].textureType = 3;
                    else if (!bottom && !right) tiles[x, y].textureType = 4;

                    else if (top && bottom && !left) tiles[x, y].textureType = 5;
                    else if (top && bottom && !right) tiles[x, y].textureType = 6;
                    else if (left && right && !top) tiles[x, y].textureType = 7;
                    else if (left && right && !bottom) tiles[x, y].textureType = 8;
                }
            }
        }

        private int FindAdjacentPassableTileRoomIndex(int x, int y)
        {
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (newX >= 0 && newX < GetWidth() && newY >= 0 && newY < GetHeight())
                {
                    DungeonTile adjacentTile = GetTile(newX, newY);
                    if (adjacentTile != null && adjacentTile.isPassable)
                    {
                        return adjacentTile.roomIndex;
                    }
                }
            }

            return -1;
        }


        public bool HasTile(int x, int y)
        {
            if (x >= 0 && x < this.tiles.GetLength(0) && y >= 0 && y < this.tiles.GetLength(1))
            {
                return this.tiles[x, y].roomIndex >= 0;
            }
            return false;
        }

    }

}
