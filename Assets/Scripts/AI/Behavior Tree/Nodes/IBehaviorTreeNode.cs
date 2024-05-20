using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;

public interface IBehaviorTreeNode
{
    NodeState Execute(Entity entity); // Метод для выполнения действия или проверки условия
}