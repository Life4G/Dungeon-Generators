using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using System.Collections.Generic;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;

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
        private Filter wanderFilter;

        private Stash<MoveRequest> moveRequestStash;
        private Stash<MovingFlag> movingFlagStash;
        private Stash<PositionComponent> positionStash;
        private Stash<WanderComponent> wanderStash;
        private Stash<EntityProviderComponent> providerStash;

        public bool[,] map;

        public override void OnAwake()
        {
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
            // Инициализация фильтров
            this.moveRequestFilter = this.World.Filter.With<MoveRequest>().Build();
            this.movingFilter = this.World.Filter.With<MovingFlag>().Build();
            this.wanderFilter = this.World.Filter.With<WanderComponent>().Build();

            // Инициализация стэшей компонентов
            this.moveRequestStash = this.World.GetStash<MoveRequest>();
            this.movingFlagStash = this.World.GetStash<MovingFlag>();
            this.positionStash = this.World.GetStash<PositionComponent>();
            this.wanderStash = this.World.GetStash<WanderComponent>();
            this.providerStash = this.World.GetStash<EntityProviderComponent>();

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

                // Удаление компонента блуждания, если он есть
                if (this.wanderStash.Has(entity))
                {
                    this.wanderStash.Remove(entity);
                }
            }

            // Обработка сущностей, находящихся в процессе перемещения
            foreach (var entity in this.movingFilter)
            {
                ref var movingFlag = ref this.movingFlagStash.Get(entity);
                ref var positionComponent = ref this.positionStash.Get(entity);

                if (movingFlag.currentIndex < movingFlag.path.Count)
                {
                    // Перемещение сущности к следующей точке пути
                    MoveEntityToNextPoint(entity, ref positionComponent, movingFlag.path[movingFlag.currentIndex]);
                    movingFlag.currentIndex++;

                    if (movingFlag.currentIndex >= movingFlag.path.Count)
                    {
                        // Удаление флага перемещения при достижении цели
                        this.movingFlagStash.Remove(entity);
                    }
                }
            }

            // Обработка сущностей, находящихся в состоянии блуждания
            foreach (var entity in this.wanderFilter)
            {
                ref var positionComponent = ref this.positionStash.Get(entity);

                // Выбор случайной доступной точки вокруг сущности
                Vector2Int randomPosition = GetRandomAdjacentPosition(positionComponent.position);
                if (randomPosition != positionComponent.position)
                {
                    MoveEntityToNextPoint(entity, ref positionComponent, randomPosition);
                }
            }
        }

        private void MoveEntityToNextPoint(Entity entity, ref PositionComponent positionComponent, Vector2Int nextPoint)
        {
            // Обновление позиции сущности в координатах карты
            positionComponent.position = nextPoint;
            this.positionStash.Set(entity, positionComponent);

            // Получение EntityProvider из компонента
            if (this.providerStash.Has(entity))
            {
                ref var providerComponent = ref this.providerStash.Get(entity);
                var entityProvider = providerComponent.entityProvider;
                if (entityProvider != null)
                {
                    var transform = entityProvider.transform;

                    // Обновление позиции сущности в мировых координатах Unity
                    transform.position = MapToWorldPosition(nextPoint);
                }
            }
        }


        private Vector2Int GetRandomAdjacentPosition(Vector2Int currentPosition)
        {
            List<Vector2Int> adjacentPositions = new List<Vector2Int>();

            // Проверка доступных точек вокруг текущей позиции
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue; // Пропуск текущей позиции
                    Vector2Int newPosition = new Vector2Int(currentPosition.x + x, currentPosition.y + y);
                    if (IsPositionValid(newPosition))
                    {
                        adjacentPositions.Add(newPosition);
                    }
                }
            }

            if (adjacentPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, adjacentPositions.Count);
                return adjacentPositions[randomIndex];
            }

            return currentPosition; // Возвращение текущей позиции, если нет доступных точек
        }

        private bool IsPositionValid(Vector2Int position)
        {
            if (position.x >= 0 && position.x < map.GetLength(0) &&
                position.y >= 0 && position.y < map.GetLength(1) &&
                map[position.x, position.y])
            {
                return true;
            }
            return false;
        }

        private Vector3 MapToWorldPosition(Vector2Int mapPosition)
        {
            // Преобразование координат карты в мировые координаты Unity
            return new Vector3(mapPosition.x, mapPosition.y, 0);
        }

        private Vector2Int WorldToMapPosition(Vector3 worldPosition)
        {
            // Преобразование мировых координат Unity в координаты карты
            return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y));
        }


    }
}
