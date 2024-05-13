using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class SelectorNode : BehaviorTreeNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

        public SelectorNode(IEnumerable<IBehaviorTreeNode> children)
        {
            this.children.AddRange(children);
        }

        public override NodeState Execute()
        {
            foreach (var child in children)
            {
                var state = child.Execute();
                if (state == NodeState.SUCCESS)
                {
                    currentState = NodeState.SUCCESS;
                    return currentState;
                }
                if (state == NodeState.RUNNING)
                {
                    currentState = NodeState.RUNNING;
                    return currentState;
                }
            }
            currentState = NodeState.FAILURE;
            return currentState;
        }
    }
}
