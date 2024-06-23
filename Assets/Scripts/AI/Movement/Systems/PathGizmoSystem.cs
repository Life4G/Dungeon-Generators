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
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(PathGizmoSystem))]
    public sealed class PathGizmoSystem : UpdateSystem
    {
        private Filter moveFilter;
        private Stash<MoveComponent> moveStash;
        private Stash<PositionComponent> positionStash;
        private PathVisualizer pathVisualizer;

        public override void OnAwake()
        {
            this.moveFilter = this.World.Filter.With<MoveComponent>().Build();
            this.moveStash = this.World.GetStash<MoveComponent>();
            this.positionStash = this.World.GetStash<PositionComponent>();

            // Найти или создать объект PathVisualizer
            GameObject visualizerObject = GameObject.Find("PathVisualizer");
            if (visualizerObject == null)
            {
                visualizerObject = new GameObject("PathVisualizer");
                pathVisualizer = visualizerObject.AddComponent<PathVisualizer>();
            }
            else
            {
                pathVisualizer = visualizerObject.GetComponent<PathVisualizer>();
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            // Очистка предыдущих путей
            pathVisualizer.ClearPaths();

            foreach (var entity in this.moveFilter)
            {
                if (!this.moveStash.Has(entity) || !this.positionStash.Has(entity))
                    continue;

                ref var moveComponent = ref this.moveStash.Get(entity);
                ref var positionComponent = ref this.positionStash.Get(entity);

                if (moveComponent.path == null || moveComponent.path.Count == 0)
                    continue;

                List<Vector3> path = new List<Vector3>();
                path.Add(new Vector3(positionComponent.position.x, positionComponent.position.y, 0));

                foreach (var step in moveComponent.path)
                {
                    path.Add(new Vector3(step.x, step.y, 0));
                }

                pathVisualizer.AddPath(path.ToArray());
            }
        }
    }
}
