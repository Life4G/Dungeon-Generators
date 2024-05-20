using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;


// Базовый абстрактный класс для всех узлов дерева
public abstract class BehaviorTreeNode : IBehaviorTreeNode
{
    protected NodeState currentState;

    public BehaviorTreeNode()
    {
        currentState = NodeState.RUNNING;
    }

    public abstract NodeState Execute(Entity entity); // Обязательная реализация метода выполнения
}

