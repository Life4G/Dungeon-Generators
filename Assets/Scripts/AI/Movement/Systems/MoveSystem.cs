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
        private Filter moveRequestFilter;
        private Filter movingFilter;

        private Stash<MoveRequest> moveRequestStash;
        private Stash<MovingFlag> movingFlagStash;
        private Stash<PositionComponent> positionStash;

        public bool[,] map;

        public override void OnAwake()
        {
            // Инициализация фильтров
            this.moveRequestFilter = this.World.Filter.With<MoveRequest>().Build();
            this.movingFilter = this.World.Filter.With<MovingFlag>().Build();

            // Инициализация стэшей компонентов
            this.moveRequestStash = this.World.GetStash<MoveRequest>();
            this.movingFlagStash = this.World.GetStash<MovingFlag>();
            this.positionStash = this.World.GetStash<PositionComponent>();

            var mapConverter = GameObject.FindObjectOfType<MapConverter>();

            if (mapConverter != null)
            {
                map = mapConverter.Map;
                if (map != null)
                {
                    Debug.Log("Карта успешно получена в MoveSystem");
                }
                else
                {
                    Debug.LogError("Карта подземелья не найдена в MapConverter!");
                }
            }
            else
            {
                Debug.LogError("MapConverter не найден!");
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            // Обработка новых запросов на перемещение
            foreach (var entity in this.moveRequestFilter)
            {
                ref var moveRequest = ref this.moveRequestStash.Get(entity);

                // Генерация пути
                List<Vector2Int> path = PathfindingAStar.FindPath(moveRequest.start, moveRequest.target, map);

                if (path != null && path.Count > 0)
                {
                    var movingFlag = new MovingFlag
                    {
                        path = path,
                        currentIndex = 0
                    };

                    // Добавление компонента MovingFlag
                    this.movingFlagStash.Set(entity, movingFlag);
                }

                // Удаление компонента MoveRequest после обработки
                this.moveRequestStash.Remove(entity);
            }

            // Обработка сущностей, находящихся в процессе перемещения
            foreach (var entity in this.movingFilter)
            {
                ref var movingFlag = ref this.movingFlagStash.Get(entity);
                ref var positionComponent = ref this.positionStash.Get(entity);

                if (movingFlag.currentIndex < movingFlag.path.Count)
                {
                    // Перемещение сущности к следующей точке пути
                    MoveEntityToNextPoint(ref positionComponent, movingFlag.path[movingFlag.currentIndex]);
                    movingFlag.currentIndex++;

                    if (movingFlag.currentIndex >= movingFlag.path.Count)
                    {
                        // Удаление флага перемещения при достижении цели
                        this.movingFlagStash.Remove(entity);
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
