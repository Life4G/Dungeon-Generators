using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Room
{
    /// <summary>
    /// Комната подземелья.
    /// </summary>
    public class DungeonRoom
    {
        /// <summary>
        /// Идентификатор комнаты.
        /// </summary>
        public int id;

        /// <summary>
        /// Имя комнаты.
        /// </summary>
        public string name;

        /// <summary>
        /// Размер комнаты (количество плиток пола).
        /// </summary>
        public int size;

        /// <summary>
        /// Ширина комнаты.
        /// </summary>
        public int width;

        /// <summary>
        /// Высота комнаты.
        /// </summary>
        public int height;

        /// <summary>
        /// Идентификатор стиля комнаты.
        /// </summary>
        public int styleId;

        /// <summary>
        /// X координата центра комнаты.
        /// </summary>
        public float centerX;

        /// <summary>
        /// Y координата центра комнаты.
        /// </summary>
        public float centerY;

        /// <summary>
        /// Является ли комната коридором.
        /// </summary>
        public bool isCorridor;

        /// <summary>
        /// Индекс фракции, которой принадлежит комната.
        /// </summary>
        public int fractionIndex;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public DungeonRoom()
        {
            id = 0;
            name = "";
            size = 0;
            width = 0;
            height = 0;
            styleId = 0;
            centerX = 0;
            centerY = 0;
            isCorridor = false;
            fractionIndex = -1;
        }

        /// <summary>
        /// Конструктор присваивания.
        /// </summary>
        /// <param name="id">Идентификатор комнаты.</param>
        /// <param name="name">Имя комнаты.</param>
        /// <param name="size">Размер комнаты (кол-во плиток пола).</param>
        /// <param name="width">Ширина комнаты.</param>
        /// <param name="height">Высота комнаты.</param>
        /// <param name="styleId">Идентификатор  стиля комнаты.</param>
        /// <param name="centerX">X координата центра комнаты.</param>
        /// <param name="centerY">Y координата центра комнаты.</param>
        /// <param name="isCorridor">Является ли коридором.</param>
        /// <param name="fractionIndex">Индекс фракции.</param>
        public DungeonRoom(int id, string name, int size, int width, int height, int styleId, float centerX, float centerY, bool isCorridor, int fractionIndex)
        {
            this.id = id;
            this.name = name;
            this.size = size;
            this.width = width;
            this.height = height;
            this.styleId = styleId;
            this.centerX = centerX;
            this.centerY = centerY;
            this.isCorridor = isCorridor;
            this.fractionIndex = fractionIndex;
        }
    }
}
