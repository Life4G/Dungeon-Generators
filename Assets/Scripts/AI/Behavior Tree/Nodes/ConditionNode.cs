using System;
using Scellecs.Morpeh;
using XNode;
using UnityEngine;

[CreateNodeMenu("BehaviorTree/ConditionNode")]
public class ConditionNode : BehaviorTreeNode
{
    [Input(connectionType = ConnectionType.Override)] public bool input; // Входной порт для соединений

    [SerializeField] private ConditionDelegate conditionDelegate;

    public override NodeState Execute(Entity entity)
    {
        if (conditionDelegate == null)
        {
            return NodeState.FAILURE;
        }
        else
        {
            return conditionDelegate.Check(entity) ? NodeState.SUCCESS : NodeState.FAILURE;
        }
    }
}
