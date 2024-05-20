using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;
using ECS;

public static class ActionDelegates
{
    public static NodeState Wander(Entity entity)
    {
        var wanderStash = World.Default.GetStash<WanderComponent>();

        if (!wanderStash.Has(entity))
        {
            wanderStash.Add(entity, new WanderComponent());
        }

        return NodeState.RUNNING;
    }




    public static NodeState MoveToFirstVisibleTarget(Entity entity)
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
            // Проверяем текущее состояние перемещения
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
            // Инициализация запроса на перемещение
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
