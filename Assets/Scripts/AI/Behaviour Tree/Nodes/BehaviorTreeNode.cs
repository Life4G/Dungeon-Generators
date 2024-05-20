using System.Collections;
using System.Collections.Generic;


namespace BehaviourTree
{
    // Базовый абстрактный класс для всех узлов дерева
    public abstract class BehaviorTreeNode : IBehaviorTreeNode
    {
        protected NodeState currentState;

        public BehaviorTreeNode()
        {
            currentState = NodeState.RUNNING;
        }

        public abstract NodeState Execute(); // Обязательная реализация метода выполнения
    }
}
