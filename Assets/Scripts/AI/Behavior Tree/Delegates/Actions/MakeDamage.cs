using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "MakeDamage", menuName = "BehaviorTree/ActionDelegates/MakeDamage")]
public class MakeDamage : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        var attackStash = World.Default.GetStash<AttackComponent>();
        var attackTargetsStash = World.Default.GetStash<AttackTargetsComponent>();

        if (!attackStash.Has(entity) || !attackTargetsStash.Has(entity))
        {
            return NodeState.FAILURE;
        }

        ref var attackComponent = ref attackStash.Get(entity);
        ref var attackTargetsComponent = ref attackTargetsStash.Get(entity);

        if (attackTargetsComponent.targetsInRange == null || attackTargetsComponent.targetsInRange.Count == 0)
        {
            return NodeState.FAILURE;
        }

        var targetEntity = attackTargetsComponent.targetsInRange[0];

        // Создаем сущность-событие для нанесения урона
        var damageRequestEntity = World.Default.CreateEntity();
        var damageRequestStash = World.Default.GetStash<DamageRequestComponent>();
        ref var damageRequestComponent = ref damageRequestStash.Add(damageRequestEntity);
        damageRequestComponent.targetEntity = targetEntity;
        damageRequestComponent.damageAmount = attackComponent.attackDamage;

        return NodeState.SUCCESS;
    }
}
