using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;
using XNode;

namespace ECS
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct BehaviorComponent : IComponent
    {
        public BehaviorTreeGraph behaviorTree; // Ссылка на граф дерева поведения
    }
}