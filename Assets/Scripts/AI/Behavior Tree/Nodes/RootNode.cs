using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;
using XNode;

[CreateNodeMenu("BehaviorTree/RootNode")]
public class RootNode : BehaviorTreeNode
{
    [Output(connectionType = ConnectionType.Multiple)] public bool output; // Выходной порт для соединений

    public override NodeState Execute(Entity entity)
    {
        foreach (var child in connectedNodes)
        {
            return child.Execute(entity); // Начинаем выполнение с первого дочернего узла
        }
        return NodeState.FAILURE;
    }
}
