//Класс генерации круглой комнаты
//В процессе она круглой быть перестанет
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreGenCircleRoom : Room
{

    public int radiusMin = 4;
    public int radiusMax = 8;

    protected int radius;

    public void SetSize(int radius)
    {
       this.radius = radius;
    }

}