using System;
using System.Collections;
using System.Collections.Generic;


namespace BehaviourTree
{
    // Узел условия
    public class ConditionNode : BehaviorTreeNode
    {
        private Func<bool> condition; // Делегат для условия

        public ConditionNode(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override NodeState Execute()
        {
            if (condition())
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
}
