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
        public static bool[,] map;

        private Filter moveFilter;
        private Stash<MoveComponent> moveStash;
        private Stash<PositionComponent> positionStash;
        private Stash<EntityProviderComponent> providerStash;

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
            this.moveFilter = this.World.Filter.With<MoveComponent>().Build();

            // Инициализация стэшей компонентов
            this.moveStash = this.World.GetStash<MoveComponent>();
            this.positionStash = this.World.GetStash<PositionComponent>();
            this.providerStash = this.World.GetStash<EntityProviderComponent>();

            // Обработка сущностей, находящихся в процессе перемещения
            foreach (var entity in this.moveFilter)
            {
                ref var moveComponent = ref this.moveStash.Get(entity);
                ref var positionComponent = ref this.positionStash.Get(entity);

                if (moveComponent.path.Count > 1)
                {
                    // Перемещение сущности к следующей точке пути (индекс 1)
                    MoveEntityToNextPoint(entity, ref positionComponent, moveComponent.path[1]);
                }
                else if (moveComponent.path.Count == 1)
                {
                    // Перемещение сущности к конечной точке пути (индекс 0)
                    MoveEntityToNextPoint(entity, ref positionComponent, moveComponent.path[0]);
                }

                // Удаление компонента перемещения после обработки
                this.moveStash.Remove(entity);
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
                    //Debug.Log("Move Entity " + entity.ID + " To Next Point " + nextPoint.ToString());
                }
            }
        }

        private Vector3 MapToWorldPosition(Vector2Int mapPosition)
        {
            // Преобразование координат карты в мировые координаты Unity
            return new Vector3(mapPosition.x, mapPosition.y, 0);
        }
    }
}
