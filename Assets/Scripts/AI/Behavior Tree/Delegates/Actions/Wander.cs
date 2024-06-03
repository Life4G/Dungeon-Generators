using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "Wander", menuName = "BehaviorTree/ActionDelegates/Wander")]
public class Wander : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        var wanderStash = World.Default.GetStash<WanderComponent>();

        if (!wanderStash.Has(entity))
        {
            wanderStash.Add(entity, new WanderComponent());
        }

        return NodeState.RUNNING;
    }
}
