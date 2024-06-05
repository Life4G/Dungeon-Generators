using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;

[CreateAssetMenu(fileName = "SayHello", menuName = "BehaviorTree/ActionDelegates/SayHello")]
public class SayHello : ActionDelegate
{
    public override NodeState Execute(Entity entity)
    {
        Debug.Log("Hello");

        return NodeState.SUCCESS;
    }
}