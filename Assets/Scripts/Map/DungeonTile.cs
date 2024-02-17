using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public class DungeonTile
    {
        public int roomIndex;           // индекс комнаты
        public bool hasAdjacentWall;    // наличиие стенки рядом
        public bool isCorner;           // угловая ли плитка
        public bool isPassable;         // проходима ли плитка
        public object content;          // содержимое плитки
        public int passageCost;         // стоимость прохождения через плитку

        // конструктор по умолчанию
        public DungeonTile()
        {
            roomIndex = -1;
            hasAdjacentWall = false;
            isCorner = false;
            isPassable = true;
            content = null;
            passageCost = 1;
        }

        // конструктор присваивания
        public DungeonTile(int roomIndex, bool hasAdjacentWall, bool isCorner, bool isPassable, object content, int passageCost)
        {
            this.roomIndex = roomIndex;
            this.hasAdjacentWall = hasAdjacentWall;
            this.isCorner = isCorner;
            this.isPassable = isPassable;
            this.content = content;
            this.passageCost = passageCost;
        }


        // установка содержимого тайла
        public void SetContent(object newContent)
        {
            content = newContent;
        }

        // проверка на наличие содержимого
        public bool HasContent()
        {
            return content != null;
        }

        // изменение проходимости тайла
        public void SetPassability(bool passable)
        {
            isPassable = passable;
        }

        // получение стоимости прохода
        public int GetPassageCost()
        {
            return passageCost;
        }

        // проверка проходимости плитки
        public bool IsPassable()
        {
            return isPassable;
        }

        // установка стоимости прохода
        public void SetPassageCost(int cost)
        {
            passageCost = cost;
        }

        // установка индекса комнаты
        public void SetRoomIndex(int index)
        {
            roomIndex = index;
        }

        // получение индекса комнаты
        public int GetRoomIndex()
        {
            return roomIndex;
        }

        // установка признака наличия соседней стенки
        public void SetHasAdjacentWall(bool hasWall)
        {
            hasAdjacentWall = hasWall;
        }

        // проверка наличия соседней стенки
        public bool HasAdjacentWall()
        {
            return hasAdjacentWall;
        }

        // установка признака угловой плитки
        public void SetIsCorner(bool corner)
        {
            isCorner = corner;
        }

        // проверка, является ли плитка угловой
        public bool IsCorner()
        {
            return isCorner;
        }
    }
}
