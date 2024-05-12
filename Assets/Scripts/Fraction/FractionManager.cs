using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Fraction
{
    public enum RelationshipType
    {
        Neutral,
        Ally,
        Enemy
    }

    /// <summary>
    /// Менеджер фракций.
    /// </summary>
    public class FractionManager : MonoBehaviour
    {
        public List<Fraction> fractions;
        public List<FractionRelationship> relationships;

        /// <summary>
        /// Получает максимальное количество комнат для указанной фракции.
        /// </summary>
        /// <param name="fractionIndex">Индекс фракции в списке.</param>
        /// <param name="totalRooms">Общее количество некоридорных комнат.</param>
        /// <returns>Максимальное количество комнат, которое может быть отведено фракции.</returns>
        public int CalculateRoomsForFraction(int totalRooms, int fractionIndex)
        {
            // количество комнат для всех фракций
            var roomsDistribution = CalculateRoomsForAllFractions(totalRooms);

            // количество комнат для конкретной фракции
            if (roomsDistribution.ContainsKey(fractionIndex))
            {
                return roomsDistribution[fractionIndex];
            }
            else
            {
                Debug.LogError($"Fraction with index {fractionIndex} not found.");
                return 0;
            }
        }

        public Dictionary<int, int> CalculateRoomsForAllFractions(int totalRooms)
        {
            float totalCoefficients = fractions.Sum(f => f.territoryCoefficient);
            Dictionary<int, int> roomsPerFraction = new Dictionary<int, int>();
            float remainingRooms = totalRooms;

            // базовое количество комнат для каждой фракции
            foreach (var fraction in fractions.Select((value, index) => new { value, index }))
            {
                float exactRooms = (fraction.value.territoryCoefficient / totalCoefficients) * totalRooms;
                int baseRooms = (int)exactRooms;
                roomsPerFraction[fraction.index] = baseRooms;
                remainingRooms -= baseRooms;
            }

            // оставшиеся комнаты в зависимости от дробных частей
            var fractionsWithRemainders = fractions.Select((fraction, index) => new
            {
                Index = index,
                Remainder = (fraction.territoryCoefficient / totalCoefficients) * totalRooms - roomsPerFraction[index]
            })
            .OrderByDescending(x => x.Remainder)
            .ToList();

            int additionalRooms = (int)remainingRooms;
            for (int i = 0; i < additionalRooms; i++)
            {
                roomsPerFraction[fractionsWithRemainders[i % fractionsWithRemainders.Count].Index]++;
            }

            return roomsPerFraction;
        }

    }
}
