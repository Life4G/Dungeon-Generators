using Scellecs.Morpeh;
using UnityEngine;
using ECS;
using System.Collections.Generic;
using Assets.Scripts.Fraction;

[CreateAssetMenu(fileName = "MakeDamage", menuName = "BehaviorTree/ActionDelegates/MakeDamage")]
public class MakeDamage : ActionDelegate
{
    private FractionManager fractionManager;

    public override NodeState Execute(Entity entity)
    {
        if (fractionManager == null)
        {
            fractionManager = FindObjectOfType<FractionManager>();
        }

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
            if (fractionStash.Has(potentialTarget))
            {
                ref var targetFractionComponent = ref fractionStash.Get(potentialTarget);
                if (IsEnemy(fractionComponent.fractionIndex, targetFractionComponent.fractionIndex))
                {
                    targetEntity = potentialTarget;
                    break;
                }
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

    private bool IsEnemy(int fractionIndex1, int fractionIndex2)
    {
        foreach (var relationship in fractionManager.relationships)
        {
            if ((relationship.fraction1Index == fractionIndex1 && relationship.fraction2Index == fractionIndex2) ||
                (relationship.fraction1Index == fractionIndex2 && relationship.fraction2Index == fractionIndex1))
            {
                return relationship.relationshipType == RelationshipType.Enemy;
            }
        }
        return false;
    }
}
