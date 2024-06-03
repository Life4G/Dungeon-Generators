using System.Collections.Generic;
using Scellecs.Morpeh;
using XNode;
using UnityEngine;

public abstract class BehaviorTreeNode : Node, IBehaviorTreeNode
{
    protected List<BehaviorTreeNode> connectedNodes;

    public abstract NodeState Execute(Entity entity); // Обязательная реализация метода выполнения

    public void Initialize()
    {
        connectedNodes = GetConnectedNodes();
    }

    // Метод для получения значений дочерних узлов
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
        return nodes;
    }
}
