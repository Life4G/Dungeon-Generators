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
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                for (int x = 0; x < tiles.GetLength(0); x++)
                {
                    tiles[x, y] = new DungeonTile();    // roomIndex = -1
                }
            }
        }

        // вернуть тайл подземелья
        public DungeonTile GetTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < tiles.GetLength(0) && y < tiles.GetLength(1))
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

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tiles[x, y] = new DungeonTile
                    {
                        roomIndex = roomIndices[x, y],
                        hasAdjacentWall = false,            // сделать проверки !!!
                        isCorner = false,
                        isPassable = true,
                        content = null,
                        passageCost = 1
                    };
                }
            }
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
                    if (tiles[x, y] != null)
                    {
                        tiles[x, y].roomIndex = roomIndices[x, y];
                    }
                }
            }
        }
    }

}
