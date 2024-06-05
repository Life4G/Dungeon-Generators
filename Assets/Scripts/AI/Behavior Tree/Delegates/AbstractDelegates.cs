using System;
using Scellecs.Morpeh;
using UnityEngine;

public abstract class ActionDelegate : ScriptableObject
{
    public abstract NodeState Execute(Entity entity);
}

public abstract class ConditionDelegate : ScriptableObject
{
    public abstract bool Check(Entity entity);
}