using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Room
{
    public class DungeonRoom
    {
        public int id;              // идентификатор комнаты
        public string name;         // имя комнаты
        public int size;            // размер комнаты (кол-во плиток пола)
        public int width;           // ширина комнаты
        public int height;          // высота комнаты
        public int styleId;         // идентификатор стиля комнаты

        // конструктор по умолчанию
        public DungeonRoom()
        {
            id = 0;
            name = "";
            size = 0;
            width = 0;
            height = 0;
            styleId = 0;
        }

        // конструктор присваивания
        public DungeonRoom(int id, string name, int size, int width, int height, int styleId)
        {
            this.id = id;
            this.name = name;
            this.size = size;
            this.width = width;
            this.height = height;
            this.styleId = styleId;
        }
    }
}
