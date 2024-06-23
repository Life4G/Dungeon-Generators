using Scellecs.Morpeh;
using Scellecs.Morpeh.Systems;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;

namespace ECS
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [CreateAssetMenu(menuName = "ECS/Systems/" + nameof(DamageSystem))]
    public sealed class DamageSystem : UpdateSystem
    {
        private Stash<HealthComponent> healthStash;

        public override void OnAwake()
        {
            this.healthStash = this.World.GetStash<HealthComponent>();
        }

        public override void OnUpdate(float deltaTime)
        {
            var damageRequestFilter = this.World.Filter.With<DamageRequestComponent>().Build();
            var damageRequestStash = this.World.GetStash<DamageRequestComponent>();

            foreach (var damageRequestEntity in damageRequestFilter)
            {
                ref var damageRequestComponent = ref damageRequestStash.Get(damageRequestEntity);

                if (this.healthStash.Has(damageRequestComponent.targetEntity))
                {
                    ref var healthComponent = ref this.healthStash.Get(damageRequestComponent.targetEntity);
                    healthComponent.currentHealth -= damageRequestComponent.damageAmount;
                }

                // Удаляем сущность-событие
                damageRequestEntity.Dispose();
            }
        }
    }
}
