using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;

public class SelectorNode : BehaviorTreeNode
{
    private List<IBehaviorTreeNode> children = new List<IBehaviorTreeNode>();

    public SelectorNode(IEnumerable<IBehaviorTreeNode> children)
    {
        this.children.AddRange(children);
    }

    public override NodeState Execute(Entity entity)
    {
        bool isAnyChildRunning = false;

        foreach (var child in children)
        {
            var state = child.Execute(entity);
            if (state == NodeState.SUCCESS)
            {
                currentState = NodeState.SUCCESS;
                return currentState;
            }
            if (state == NodeState.RUNNING)
            {
                isAnyChildRunning = true;
            }
        }

        currentState = isAnyChildRunning ? NodeState.RUNNING : NodeState.FAILURE;
        return currentState;
    }
}
