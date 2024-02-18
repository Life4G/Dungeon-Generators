using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public class DungeonMap
    {
        private DungeonTile[,] tiles; // массив плиток подземелья

        // конструктор по размеру карты
        public DungeonMap(int width, int height)
        {
            tiles = new DungeonTile[width, height];
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

        // установка содержимого
        public void SetTileContent(int x, int y, object content)
        {
            DungeonTile tile = GetTile(x, y);
            if (tile != null)
            {
                tile.SetContent(content);
            }
        }

        // конструктор по массиву тайлов пола
        public DungeonMap(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);
            tiles = new DungeonTile[width, height];
            InitializeMapWithRoomIndices(roomIndices);
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
                    tiles[x, y].SetRoomIndex(roomIndices[x, y]);
                    tiles[x, y].SetHasAdjacentWall(isAdjacentToWall);
                    tiles[x, y].SetIsCorner(isCorner);
                    tiles[x, y].SetPassability(isPassable);
                    tiles[x, y].SetPassageCost(isPassable ? 1 : 999);
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
        public void UpdateMapWithRoomIndices_(int[,] roomIndices)
        {
            int width = roomIndices.GetLength(0);
            int height = roomIndices.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].SetRoomIndex(roomIndices[x, y]);
                    }
                }
            }
        }

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
                        tiles[x, y].SetRoomIndex(roomIndices[x, y]);
                        tiles[x, y].SetHasAdjacentWall(isAdjacentToWall);
                        tiles[x, y].SetIsCorner(isCorner);
                        tiles[x, y].SetPassability(isPassable);
                        tiles[x, y].SetPassageCost(isPassable ? 1 : 999);
                    }
                }
            }
        }

    }

}
