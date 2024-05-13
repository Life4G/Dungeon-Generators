using System.Collections;
using System.Collections.Generic;


namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    // Интерфейс для всех узлов дерева поведения
    public interface IBehaviorTreeNode
    {
        NodeState Execute(); // Метод для выполнения действия или проверки условия
    }
}
