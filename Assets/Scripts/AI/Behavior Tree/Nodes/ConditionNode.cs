using System;
using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;

// Узел условия
public class ConditionNode : BehaviorTreeNode
{
    private Func<Entity, bool> condition; // Делегат для условия

    public ConditionNode(Func<Entity, bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Execute(Entity entity)
    {
        if (condition(entity))
        {
            currentState = NodeState.SUCCESS;
        }
        else
        {
            currentState = NodeState.FAILURE;
        }
        return currentState;
    }
}
