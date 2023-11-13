using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CircleRoom : RoomBase
{

    public int radiusMin = 3;
    public int radiusMax = 8;

    protected int radius;

    public void SetSize(int radius)
    {
       this.radius = radius;
    }

    public override bool Validate()
    {
        return true;
    }
}