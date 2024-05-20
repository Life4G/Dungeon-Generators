using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;

namespace ECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(BehaviorSystem))]
    public sealed class BehaviorSystem : UpdateSystem
    {
        private Filter behaviorFilter;
        private Stash<BehaviorComponent> behaviorStash;

        public override void OnAwake()
        {
            // Создание фильтра для выборки всех сущностей с компонентом BehaviorComponent
            this.behaviorFilter = this.World.Filter.With<BehaviorComponent>().Build();
            // Получение стэша компонентов BehaviorComponent
            this.behaviorStash = this.World.GetStash<BehaviorComponent>();
        }

        public override void OnUpdate(float deltaTime)
        {

            // Выполнение дерева поведения для каждой сущности с BehaviorComponent
            foreach (var entity in this.behaviorFilter)
            {
                ref var behaviorComponent = ref this.behaviorStash.Get(entity);

                if (behaviorComponent.rootNode == null)
                {
                    Debug.LogError("RootNode is null for entity: " + entity.ID);
                    continue; // Пропустить сущность, если rootNode не инициализирован
                }

                behaviorComponent.rootNode.Execute(entity);
            }
        }
    }
}