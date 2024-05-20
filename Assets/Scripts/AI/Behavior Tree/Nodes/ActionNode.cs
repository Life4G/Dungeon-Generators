using System;
using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;

// Узел действия с поддержкой различных состояний выполнения
public class ActionNode : BehaviorTreeNode
{
    private Func<Entity, NodeState> action; // Делегат для действия с возвратом состояния

    public ActionNode(Func<Entity, NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Execute(Entity entity)
    {
        currentState = action(entity);
        return currentState;
    }
}

