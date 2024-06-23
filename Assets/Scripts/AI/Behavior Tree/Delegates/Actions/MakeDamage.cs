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
        var fractionStash = World.Default.GetStash<FractionComponent>();

        if (!attackStash.Has(entity) || !attackTargetsStash.Has(entity) || !fractionStash.Has(entity))
        {
            return NodeState.FAILURE;
        }

        ref var attackComponent = ref attackStash.Get(entity);
        ref var attackTargetsComponent = ref attackTargetsStash.Get(entity);
        ref var fractionComponent = ref fractionStash.Get(entity);

        Entity targetEntity = null;

        foreach (var potentialTarget in attackTargetsComponent.targetsInRange)
        {
            if (fractionStash.Has(potentialTarget) && fractionStash.Get(potentialTarget).fractionName != fractionComponent.fractionName)
            {
                targetEntity = potentialTarget;
                break;
            }
        }

        if (targetEntity == null)
        {
            return NodeState.FAILURE;
        }

        // Создаем сущность-событие для нанесения урона
        var damageRequestEntity = World.Default.CreateEntity();
        var damageRequestStash = World.Default.GetStash<DamageRequestComponent>();
        ref var damageRequestComponent = ref damageRequestStash.Add(damageRequestEntity);
        damageRequestComponent.targetEntity = targetEntity;
        damageRequestComponent.damageAmount = attackComponent.attackDamage;

        return NodeState.SUCCESS;
    }
}
