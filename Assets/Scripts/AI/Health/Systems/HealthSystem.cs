using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;

namespace ECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(HealthSystem))]
    public sealed class HealthSystem : UpdateSystem
    {
        private Filter healthFilter;
        private Stash<HealthComponent> healthStash;
        private Stash<DeadFlagComponent> deadFlagStash;

        public override void OnAwake()
        {
            this.healthFilter = this.World.Filter.With<HealthComponent>().Build();
        }

        public override void OnUpdate(float deltaTime)
        {
            this.healthStash = this.World.GetStash<HealthComponent>();
            this.deadFlagStash = this.World.GetStash<DeadFlagComponent>();

            foreach (var entity in this.healthFilter)
            {
                ref var healthComponent = ref this.healthStash.Get(entity);

                if (healthComponent.currentHealth <= 0)
                {
                    if (!this.deadFlagStash.Has(entity))
                    {
                        ref var deadFlagComponent = ref this.deadFlagStash.Add(entity);
                        //Debug.Log($"Entity {entity.ID} is dead.");
                    }
                }
            }
        }
    }
}
