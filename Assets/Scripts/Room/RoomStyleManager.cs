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

    internal class RoomStyleManager : MonoBehaviour
    {
        [SerializeField]
        private List<RoomStyle> roomStyles;

        // Получить стиль по названию
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
        // Получить стиль по индексу
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
        // Количество стилей
        public int GetStylesCount()
        {
            Debug.Log("Всего стилей: " + roomStyles.Count);
            return roomStyles.Count;
        }
        // Случайный индекс стиля
        public int GetRandomStyleIndex()
        {
            if (roomStyles.Count == 0)
            {
                Debug.LogWarning("Список стилей пуст.");
                return -1;
            }
            int index = Random.Range(0, roomStyles.Count);
            Debug.Log("Случайный индекс стиля: " + index);
            return index;
        }
        // Случайное название стиля
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
    }
}
