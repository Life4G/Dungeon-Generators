using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;
using ECS;

public static class ConditionDelegates
{
    public static bool HasVisibleTargets(Entity entity)
    {
        var visionStash = World.Default.GetStash<VisionComponent>();

        if (!visionStash.Has(entity))
        {
            return false;
        }

        ref var visionComponent = ref visionStash.Get(entity);
        return visionComponent.visibleEntities.Count > 0;
    }
}
