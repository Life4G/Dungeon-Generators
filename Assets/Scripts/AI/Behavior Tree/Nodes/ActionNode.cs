using System;
using Scellecs.Morpeh;
using XNode;
using UnityEngine;

[CreateNodeMenu("BehaviorTree/ActionNode")]
public class ActionNode : BehaviorTreeNode
{
    [Input(connectionType = ConnectionType.Override)] public bool input; // Входной порт для соединений

    [SerializeField] private ActionDelegate actionDelegate;

    public override NodeState Execute(Entity entity)
    {
        if (actionDelegate == null)
        {
            return NodeState.FAILURE;
        }
        else
        {
            return actionDelegate.Execute(entity);
        }
    }
}
