using UnityEngine;
using Scellecs.Morpeh;
using Scellecs.Morpeh.Providers;
using ECS;
using System.Collections.Generic;

public class EntityInitializer : MonoBehaviour
{
    void Start()
    {
        InitializeEntity();
    }

    private void InitializeEntity()
    {
        // Получение сущности из EntityProvider
        var entityProvider = GetComponent<EntityProvider>();

        if (entityProvider == null)
        {
            Debug.LogError("EntityProvider not found on GameObject!");
            return;
        }

        var entity = entityProvider.Entity;

        // Инициализация компонента позиции
        var positionStash = World.Default.GetStash<PositionComponent>();
        if (positionStash.Has(entity))
        {
            ref var positionComponent = ref positionStash.Get(entity);
            positionComponent.position = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            positionStash.Set(entity, positionComponent);
        }

        // Инициализация компонента здоровья, если он существует
        var healthStash = World.Default.GetStash<HealthComponent>();
        if (healthStash.Has(entity))
        {
            ref var healthComponent = ref healthStash.Get(entity);
            healthComponent.currentHealth = healthComponent.maxHealth;
            healthStash.Set(entity, healthComponent);
        }

        // Добавляем EntityProviderComponent
        var providerStash = World.Default.GetStash<EntityProviderComponent>();
        ref var providerComponent = ref providerStash.Add(entity);
        providerComponent.entityProvider = entityProvider;

        // Инициализация компонента AttackTargetsComponent, если существует AttackComponent
        var attackStash = World.Default.GetStash<AttackComponent>();
        if (attackStash.Has(entity))
        {
            var attackTargetsStash = World.Default.GetStash<AttackTargetsComponent>();
            ref var attackTargetsComponent = ref attackTargetsStash.Add(entity);
            attackTargetsComponent.targetsInRange = new List<Entity>();
            attackTargetsStash.Set(entity, attackTargetsComponent);
        }
    }
}
