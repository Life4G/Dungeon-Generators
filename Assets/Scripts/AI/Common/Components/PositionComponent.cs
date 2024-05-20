using Scellecs.Morpeh;
using UnityEngine;
using Unity.IL2CPP.CompilerServices;


namespace ECS
{
    [System.Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct PositionComponent : IComponent
    {
        public Vector2Int position;
    }

}
