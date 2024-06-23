using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MoveToFirstVisibleTarget", menuName = "BehaviorTree/ActionDelegates/MoveToFirstVisibleTarget")]
public class MoveToFirstVisibleTarget : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        Debug.Log("Move to visible target");
        var visionStash = World.Default.GetStash<VisionComponent>();
        var moveStash = World.Default.GetStash<MoveComponent>();
        var positionStash = World.Default.GetStash<PositionComponent>();
        var fractionStash = World.Default.GetStash<FractionComponent>();

        if (!visionStash.Has(entity) || !positionStash.Has(entity) || !fractionStash.Has(entity))
        {
            return NodeState.FAILURE;
        }

        ref var visionComponent = ref visionStash.Get(entity);
        ref var positionComponent = ref positionStash.Get(entity);
        ref var fractionComponent = ref fractionStash.Get(entity);

        Entity targetEntity = null;

        foreach (var potentialTarget in visionComponent.visibleEntities)
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

        ref var targetPosition = ref positionStash.Get(targetEntity);

        List<Vector2Int> path = PathfindingAStar.FindPath(positionComponent.position, targetPosition.position, MoveSystem.map);

        if (path == null || path.Count == 0)
        {
            return NodeState.FAILURE;
        }

        var moveComponent = new MoveComponent
        {
            path = path
        };

        moveStash.Set(entity, moveComponent);
        return NodeState.SUCCESS;
    }
}
