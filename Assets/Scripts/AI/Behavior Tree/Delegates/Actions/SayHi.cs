using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "SayHi", menuName = "BehaviorTree/ActionDelegates/SayHi")]
public class SayHi : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        Debug.Log("Hi, my name is " + entity.ID);

        return NodeState.SUCCESS;
    }
}