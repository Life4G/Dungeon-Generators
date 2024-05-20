using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scellecs.Morpeh;


public class BehaviorTreeBuilder : MonoBehaviour
{
    private Dictionary<string, IBehaviorTreeNode> behaviorTrees;

    void Awake()
    {
        behaviorTrees = new Dictionary<string, IBehaviorTreeNode>();

        // Создание дерева поведения для паука
        var spiderBehaviorTree = CreateSpiderBehaviorTree();
        behaviorTrees.Add("Spider", spiderBehaviorTree);

        // Здесь можно добавить создание деревьев для других классов существ
    }

    private IBehaviorTreeNode CreateSpiderBehaviorTree()
    {
        // Создание узлов дерева
        var moveToEnemy = new ActionNode(ActionDelegates.MoveToFirstVisibleTarget);
        var wander = new ActionNode(ActionDelegates.Wander);
        var hasVisibleEnemies = new ConditionNode(entity => ConditionDelegates.HasVisibleTargets(entity));

        var selector = new SelectorNode(new List<IBehaviorTreeNode> {
            new SequenceNode(new List<IBehaviorTreeNode> {
                hasVisibleEnemies,
                moveToEnemy
            }),
            wander
        });

        return selector;
    }

    public IBehaviorTreeNode GetBehaviorTree(string creatureClass)
    {
        if (behaviorTrees.TryGetValue(creatureClass, out var behaviorTree))
        {
            return behaviorTree;
        }

        return null;
    }
}

