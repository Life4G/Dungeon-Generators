using System;
using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
// Узел действия с поддержкой различных состояний выполнения
public class ActionNode : BehaviorTreeNode
    {
        private Func<NodeState> action; // Делегат для действия с возвратом состояния

        public ActionNode(Func<NodeState> action)
        {
            this.action = action;
        }

        public override NodeState Execute()
        {
            currentState = action();
            return currentState;
        }
    }
}
