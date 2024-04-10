using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public class DungeonTile
    {
        public int roomIndex { get; set; }           // индекс комнаты
        public int  textureType {  get; set; }       // тип текстуры
        public bool hasAdjacentWall { get; set; }    // наличиие стенки рядом
        public bool isCorner { get; set; }           // угловая ли плитка
        public bool isPassable { get; set; }         // проходима ли плитка

        // конструктор по умолчанию
        public DungeonTile()
        {
            roomIndex = -1;
            textureType = -1;
            hasAdjacentWall = false;
            isCorner = false;
            isPassable = false;
        }

        // конструктор присваивания
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
