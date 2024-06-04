using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public class DungeonTile
    {
        /// <summary>
        /// Индекс комнаты которой принадлежит плитка.
        /// </summary>
        public int roomIndex { get; set; }

        /// <summary>
        /// Тип текстуры плитки.
        /// </summary>
        public int textureType { get; set; }

        /// <summary>
        /// Указывает, есть ли стена рядом с плиткой.
        /// </summary>
        public bool hasAdjacentWall { get; set; }

        /// <summary>
        /// Угловая ли это плитка.
        /// </summary>
        public bool isCorner { get; set; }

        /// <summary>
        /// Проходима ли плитка.
        /// </summary>
        public bool isPassable { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public DungeonTile()
        {
            roomIndex = -1;
            textureType = -1;
            hasAdjacentWall = false;
            isCorner = false;
            isPassable = false;
        }

        /// <summary>
        /// Конструктор присваивания.
        /// </summary>
        /// <param name="roomIndex">Инндекс комнаты.</param>
        /// <param name="textureType">Тип текстуры.</param>
        /// <param name="hasAdjacentWall">Наличие стены рядом.</param>
        /// <param name="isCorner">Угловая ли плитка.</param>
        /// <param name="isPassable">Прооходима ли плитка.</param>
        public DungeonTile(int roomIndex, int textureType, bool hasAdjacentWall, bool isCorner, bool isPassable)
        {
            this.roomIndex = roomIndex;
            this.textureType = textureType;
            this.hasAdjacentWall = hasAdjacentWall;
            this.isCorner = isCorner;
            this.isPassable = isPassable;
        }
    }
}
