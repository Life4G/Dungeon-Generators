using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField]
    GridManager grid;
    [SerializeField]
    int seed;
    [SerializeField]
    bool isNew = true;


    public void Press()
    {
        if (isNew)
            grid.Reload();
        else
            grid.Reload(seed);
        seed = grid.generator.GetSeed();
    }

}
