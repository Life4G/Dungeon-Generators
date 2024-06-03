using System.Collections.Generic;
using Scellecs.Morpeh;
using XNode;

[CreateNodeMenu("BehaviorTree/SelectorNode")]
public class SelectorNode : BehaviorTreeNode
{
    [Input(connectionType = ConnectionType.Override)] public bool input; // Входной порт для соединений
    [Output(connectionType = ConnectionType.Multiple)] public bool output; // Выходной порт для соединений

    public override NodeState Execute(Entity entity)
    {
        foreach (var child in connectedNodes)
        {
            var state = child.Execute(entity);
            if (state != NodeState.FAILURE)
            {
                return state; // Возвращает то же состояние, что и дочерний узел
            }
        }
        return NodeState.FAILURE; // Если все узлы возвращают Failure, возвращает Failure
    }
}
