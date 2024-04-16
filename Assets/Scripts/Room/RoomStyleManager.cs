using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Room
{

    public class RoomStyleManager : MonoBehaviour
    {
        [SerializeField]
        private List<RoomStyle> roomStyles;

        /// <summary>
        /// Возвращает стиль комнаты по имени.
        /// </summary>
        /// <param name="name">Имя стиля комнаты.</param>
        /// <returns>Найденный стиль комнаты или null, если стиль не найден.</returns>
        public RoomStyle GetRoomStyle(string name)
        {
            try
            {
                RoomStyle style = roomStyles.Find(s => s.styleName == name);
                if (style == null)
                {
                    Debug.LogError($"Стиль с названием {name} не найден.");
                    return null;
                }
                return style;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Ошибка при попытке получить стиль: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Возвращает стиль комнаты по индексу в списке.
        /// </summary>
        /// <param name="index">Индекс стиля в списке.</param>
        /// <returns>Стиль комнаты по указанному индексу или null при выходе индекса за пределы списка.</returns>
        public RoomStyle GetRoomStyle(int index)
        {
            try
            {
                return roomStyles[index];
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                Debug.LogError($"Индекс {index} вне диапазона. Ошибка: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Возвращает общее количество стилей комнат.
        /// </summary>
        /// <returns>Количество стилей комнат.</returns>
        public int GetStylesCount()
        {
            Debug.Log("Всего стилей: " + roomStyles.Count);
            return roomStyles.Count;
        }

        /// <summary>
        /// Возвращает случайный индекс стиля из списка стилей.
        /// </summary>
        /// <returns>Индекс случайного стиля комнаты или -1, если список стилей пуст.</returns>
        public int GetRandomStyleIndex()
        {
            if (roomStyles.Count == 0)
            {
                Debug.LogWarning("Список стилей пуст.");
                return -1;
            }
            int index = Random.Range(0, roomStyles.Count);
            //Debug.Log("Случайный индекс стиля: " + index);
            return index;
        }

        /// <summary>
        /// Возвращает случайное имя стиля из списка стилей.
        /// </summary>
        /// <returns>Имя случайного стиля комнаты или null, если список стилей пуст.</returns>
        public string GetRandomStyleName()
        {
            if (roomStyles.Count == 0)
            {
                Debug.LogWarning("Список стилей пуст.");
                return null;
            }
            int index = GetRandomStyleIndex();
            string styleName = roomStyles[index].styleName;
            Debug.Log("Случайное название стиля: " + styleName);
            return styleName;
        }

        /// <summary>
        /// Возвращает имя стиля комнаты по индексу.
        /// </summary>
        /// <param name="index">Индекс стиля в списке.</param>
        /// <returns>Имя стиля комнаты по указанному индексу или пустую строку при некрректном индексе.</returns>
        public string GetStyleNameByIndex(int index)
        {
            if (index >= 0 && index < roomStyles.Count)
            {
                return roomStyles[index].styleName;
            }
            Debug.LogError($"Индекс {index} вне диапазона.");
            return "";
        }

    }
}
