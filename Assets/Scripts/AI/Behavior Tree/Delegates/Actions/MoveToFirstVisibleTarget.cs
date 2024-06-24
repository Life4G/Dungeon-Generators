using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;
using System.Collections.Generic;
using Assets.Scripts.Fraction;

[CreateAssetMenu(fileName = "MoveToFirstVisibleTarget", menuName = "BehaviorTree/ActionDelegates/MoveToFirstVisibleTarget")]
public class MoveToFirstVisibleTarget : ActionDelegate
{
    private FractionManager fractionManager;

    public override NodeState Execute(Entity entity)
    {
        //Debug.Log("Move to visible target");

        if (fractionManager == null)
        {
            fractionManager = FindObjectOfType<FractionManager>();
        }

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
