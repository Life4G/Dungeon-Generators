using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class SequenceNode : BehaviorTreeNode
    {
        private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

        public SequenceNode(IEnumerable<IBehaviorTreeNode> children)
        {
            this.children.AddRange(children);
        }

        public override NodeState Execute()
        {
            foreach (var child in children)
            {
                var state = child.Execute();
                if (state == NodeState.FAILURE)
                {
                    currentState = NodeState.FAILURE;
                    return currentState;
                }
                if (state == NodeState.RUNNING)
                {
                    currentState = NodeState.RUNNING;
                    return currentState;
                }
            }
            currentState = NodeState.SUCCESS;
            return currentState;
        }
    }
}
