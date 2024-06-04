using Assets.Scripts.Fraction;
using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SceneObject 
{
    public string name;
    public ScriptableObject type;
    public MonoBehaviour prefab;
    public int min;
    public int max;
    public int fractionsId;

    public SceneObject()
    {
        name = "New Object";
        type = null; prefab = null;
        min = 0; max = 0;
        fractionsId = 0;
    }

    public List<int>GetFractionIds()
    {
        List<int> result = new List<int>();
        for (int i = 0; i < sizeof(int) * 8; i++)
        {
            if ((fractionsId & (1 << i)) > 0)
                result.Add(i);
        }
        return result;
    }

    //public static SceneObject CreateSceneObject()
    //{
    //    SceneObject sceneObject = CreateInstance<SceneObject>();
    //    sceneObject.Init();
    //    return sceneObject;
    //}
    //private void Init()
    //{
    //    this.name = "New Object";
    //    type = null; prefab = null;
    //    min = 0; max = 0;
    //    fractions = new List<Fraction>();
    //}

}