using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "MoveToFirstVisibleTarget", menuName = "BehaviorTree/ActionDelegates/MoveToFirstVisibleTarget")]
public class MoveToFirstVisibleTarget : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        var visionStash = World.Default.GetStash<VisionComponent>();
        var moveRequestStash = World.Default.GetStash<MoveRequest>();
        var positionStash = World.Default.GetStash<PositionComponent>();
        var movingFlagStash = World.Default.GetStash<MovingFlag>();

        if (!visionStash.Has(entity) || !positionStash.Has(entity))
        {
            return NodeState.FAILURE;
        }

        ref var visionComponent = ref visionStash.Get(entity);
        ref var positionComponent = ref positionStash.Get(entity);

        if (visionComponent.visibleEntities.Count == 0)
        {
            return NodeState.FAILURE;
        }

        var targetEntity = visionComponent.visibleEntities[0];
        ref var targetPosition = ref positionStash.Get(targetEntity);

        if (movingFlagStash.Has(entity))
        {
            ref var currentPosition = ref positionStash.Get(entity);

            if (currentPosition.position == targetPosition.position)
            {
                movingFlagStash.Remove(entity);
                return NodeState.SUCCESS;
            }
            else
            {
                return NodeState.RUNNING;
            }
        }
        else
        {
            var moveRequest = new MoveRequest
            {
                start = positionComponent.position,
                target = targetPosition.position,
                entity = entity
            };

            moveRequestStash.Set(entity, moveRequest);
            return NodeState.RUNNING;
        }
    }
}
