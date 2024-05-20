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
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(VisionSystem))]
    public sealed class VisionSystem : UpdateSystem
    {
        private Filter visionFilter;
        private Filter positionFilter;

        private Stash<VisionComponent> visionStash;
        private Stash<PositionComponent> positionStash;

        private bool[,] map;

        public override void OnAwake()
        {
            var mapConverter = GameObject.FindObjectOfType<MapConverter>();

            if (mapConverter != null)
            {
                map = mapConverter.Map;
                if (map != null)
                {
                    Debug.Log("Карта успешно получена в VisionSystem");
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
            this.visionFilter = this.World.Filter.With<VisionComponent>().With<PositionComponent>().Build();
            this.positionFilter = this.World.Filter.With<PositionComponent>().Build();

            this.visionStash = this.World.GetStash<VisionComponent>();
            this.positionStash = this.World.GetStash<PositionComponent>();

            foreach (var visionEntity in this.visionFilter)
            {
                ref var visionComponent = ref this.visionStash.Get(visionEntity);
                ref var visionPosition = ref this.positionStash.Get(visionEntity);

                visionComponent.visibleEntities.Clear();

                foreach (var targetEntity in this.positionFilter)
                {
                    if (visionEntity == targetEntity)
                    {
                        continue;
                    }

                    ref var targetPosition = ref this.positionStash.Get(targetEntity);

                    if (IsWithinVisionRange(visionPosition.position, targetPosition.position, visionComponent.visionRange))
                    {
                        // Проверка прямой видимости (опционально)
                        visionComponent.visibleEntities.Add(targetEntity);
                    }
                }
            }
        }

        private bool IsWithinVisionRange(Vector2Int visionPosition, Vector2Int targetPosition, float visionRange)
        {
            return Vector2Int.Distance(visionPosition, targetPosition) <= visionRange;
        }
    }
}