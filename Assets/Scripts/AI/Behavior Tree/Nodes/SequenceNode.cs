using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;

public class SequenceNode : BehaviorTreeNode
{
    private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

    public SequenceNode(IEnumerable<IBehaviorTreeNode> children)
    {
        this.children.AddRange(children);
    }

    public override NodeState Execute(Entity entity)
    {
        bool isAnyChildRunning = false;

        foreach (var child in children)
        {
            var state = child.Execute(entity);
            if (state == NodeState.FAILURE)
            {
                currentState = NodeState.FAILURE;
                return currentState;
            }
            if (state == NodeState.RUNNING)
            {
                isAnyChildRunning = true;
            }
        }

        currentState = isAnyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return currentState;
    }
}
