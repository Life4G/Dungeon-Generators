using Scellecs.Morpeh.Systems;
using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using ECS;

[Il2CppSetOption(Option.NullChecks, false)]
[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
[Il2CppSetOption(Option.DivideByZeroChecks, false)]
[CreateAssetMenu(menuName = "ECS/Systems/" + nameof(RemoveDeadEntitiesSystem))]
public sealed class RemoveDeadEntitiesSystem : UpdateSystem
{
    private Filter deadEntitiesFilter;
    private Stash<DeadFlagComponent> deadFlagStash;
    private Stash<EntityProviderComponent> entityProviderStash;

    public override void OnAwake()
    {
        this.deadEntitiesFilter = this.World.Filter.With<DeadFlagComponent>().With<EntityProviderComponent>().Build();
        this.deadFlagStash = this.World.GetStash<DeadFlagComponent>();
        this.entityProviderStash = this.World.GetStash<EntityProviderComponent>();
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var entity in this.deadEntitiesFilter)
        {
            ref var providerComponent = ref this.entityProviderStash.Get(entity);
            if (providerComponent.entityProvider != null)
            {
                GameObject.Destroy(providerComponent.entityProvider.gameObject);
            }
            entity.Dispose();
        }
    }
}