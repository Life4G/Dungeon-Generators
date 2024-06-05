using System.Collections.Generic;
using XNode;
using UnityEngine;

[CreateAssetMenu]
public class BehaviorTreeGraph : NodeGraph
{
    private RootNode rootNode;

    private void OnEnable()
    {
        InitializeGraph();
    }
    private void Init()
    {
        InitializeGraph();
    }

    public void InitializeGraph()
    {
        foreach (var node in nodes)
        {
            if (node is BehaviorTreeNode behaviorNode)
            {
                behaviorNode.Initialize();
            }

            if (node is RootNode root)
            {
                rootNode = root;
            }
        }

        if (rootNode == null)
        {
            Debug.LogError("RootNode not found in the BehaviorTreeGraph!");
        }
    }

    public RootNode GetRootNode()
    {
        return rootNode;
    }
}
