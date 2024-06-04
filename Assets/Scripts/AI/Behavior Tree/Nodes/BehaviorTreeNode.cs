using System.Collections.Generic;
using Scellecs.Morpeh;
using XNode;
using UnityEngine;

public abstract class BehaviorTreeNode : Node, IBehaviorTreeNode
{
    public int executionOrder; // Порядок выполнения узла

    protected List<BehaviorTreeNode> connectedNodes;

    public BehaviorTreeNode()
    {
        // Инициализация, если необходимо
    }

    public abstract NodeState Execute(Entity entity); // Обязательная реализация метода выполнения

    public void Initialize()
    {
        connectedNodes = GetConnectedNodes();
    }

    // Метод для получения и сортировки дочерних узлов по порядку выполнения
    protected List<BehaviorTreeNode> GetConnectedNodes()
    {
        List<BehaviorTreeNode> nodes = new List<BehaviorTreeNode>();
        NodePort outputPort = GetOutputPort("output");
        if (outputPort != null)
        {
            foreach (NodePort connection in outputPort.GetConnections())
            {
                if (connection.node is BehaviorTreeNode node)
                {
                    nodes.Add(node);
                }
            }
        }
        // Сортировка узлов по порядку выполнения
        nodes.Sort((x, y) => x.executionOrder.CompareTo(y.executionOrder));
        return nodes;
    }

    public override object GetValue(NodePort port)
    {
        return null; // Вернуть null, так как узлы не возвращают значений
    }
}
