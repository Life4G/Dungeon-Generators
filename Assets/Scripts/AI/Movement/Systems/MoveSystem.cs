using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;
using Scellecs.Morpeh;

namespace ECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(MoveSystem))]
    public sealed class MoveSystem : UpdateSystem
    {

        public bool[,] map; // Ссылка на ваш игровой массив

        public override void OnAwake()
        {
            // Поиск объекта с компонентом GridManager
            var gridManager = GameObject.FindObjectOfType<GridManager>();

            if (gridManager != null)
            {
                // Получение карты подземелья
                var dungeonMap = gridManager.GetDungeonMap();

                if (dungeonMap != null)
                {
                    // Конвертация карты в булевый массив
                    map = MapConverter.ConvertToBoolArray(dungeonMap);

                    Debug.Log("Карта успешно конвертирована в системе перемещения");
                }
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            // Создание фильтров
            var moveRequestFilter = this.World.Filter.With<MoveRequest>().Build();
            var movingFilter = this.World.Filter.With<MovingFlag>().Build();

            // Получение стэшей компонентов
            var moveRequestStash = this.World.GetStash<MoveRequest>();
            var movingFlagStash = this.World.GetStash<MovingFlag>();
            var positionStash = this.World.GetStash<PositionComponent>();

            // Обработка новых запросов на перемещение
            foreach (var entity in moveRequestFilter)
            {
                ref var moveRequest = ref moveRequestStash.Get(entity);

                // Генерация пути
                List<Vector2Int> path = Pathfinding.FindPath(moveRequest.start, moveRequest.target, map);

                if (path != null && path.Count > 0)
                {
                    var movingFlag = new MovingFlag
                    {
                        path = path,
                        currentIndex = 0
                    };

                    // Добавление компонента MovingFlag
                    movingFlagStash.Set(entity, movingFlag);
                }

                // Удаление компонента MoveRequest после обработки
                moveRequestStash.Remove(entity);
            }

            // Обработка сущностей, находящихся в процессе перемещения
            foreach (var entity in movingFilter)
            {
                ref var movingFlag = ref movingFlagStash.Get(entity);
                ref var positionComponent = ref positionStash.Get(entity);

                if (movingFlag.currentIndex < movingFlag.path.Count)
                {
                    // Перемещение сущности к следующей точке пути
                    MoveEntityToNextPoint(ref positionComponent, movingFlag.path[movingFlag.currentIndex]);
                    movingFlag.currentIndex++;

                    if (movingFlag.currentIndex >= movingFlag.path.Count)
                    {
                        // Удаление флага перемещения при достижении цели
                        movingFlagStash.Remove(entity);
                    }
                }
            }
        }

        private void MoveEntityToNextPoint(ref PositionComponent positionComponent, Vector2Int nextPoint)
        {
            // Обновление позиции сущности
            positionComponent.position = nextPoint;
        }
    }
}