using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "HasVisibleTargets", menuName = "BehaviorTree/ConditionDelegates/HasVisibleTargets")]
public class HasVisibleTargets : ConditionDelegate
{
    public override bool Check(Entity entity)
    {
        //Debug.Log("Check Visible Targets");
        var visionStash = World.Default.GetStash<VisionComponent>();

        if (!visionStash.Has(entity))
        {
            return false;
        }

        ref var visionComponent = ref visionStash.Get(entity);
        return visionComponent.visibleEntities.Count > 0;
    }
}
