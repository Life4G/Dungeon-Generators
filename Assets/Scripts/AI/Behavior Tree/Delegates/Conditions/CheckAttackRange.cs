using Scellecs.Morpeh;
using UnityEngine;
using System.Collections.Generic;
using ECS;

[CreateAssetMenu(fileName = "CheckAttackRange", menuName = "BehaviorTree/ConditionDelegates/CheckAttackRange")]
public class CheckAttackRange : ConditionDelegate
{
    public override bool Check(Entity entity)
    {
        var attackStash = World.Default.GetStash<AttackComponent>();
        var positionStash = World.Default.GetStash<PositionComponent>();
        var attackTargetsStash = World.Default.GetStash<AttackTargetsComponent>();
        var deadFlagStash = World.Default.GetStash<DeadFlagComponent>();

        if (!attackStash.Has(entity) || !positionStash.Has(entity))
        {
            return false;
        }

        ref var attackComponent = ref attackStash.Get(entity);
        ref var positionComponent = ref positionStash.Get(entity);

        List<Entity> targetsInRange = new List<Entity>();

        var positionFilter = World.Default.Filter.With<PositionComponent>().Build();

        foreach (var targetEntity in positionFilter)
        {
            if (targetEntity != entity && positionStash.Has(targetEntity) && !deadFlagStash.Has(targetEntity))
            {
                ref var targetPositionComponent = ref positionStash.Get(targetEntity);
                float distance = Vector2Int.Distance(positionComponent.position, targetPositionComponent.position);

                if (distance <= attackComponent.attackRange)
                {
                    targetsInRange.Add(targetEntity);
                }
            }
        }

        if (targetsInRange.Count > 0)
        {
            ref var attackTargetsComponent = ref attackTargetsStash.Add(entity);
            attackTargetsComponent.targetsInRange = targetsInRange;
            return true;
        }

        return false;
    }
}
