//Класс генерации круглой комнаты
//В процессе она круглой быть перестанет
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PreGenCircleRoom : RoomBase
{

    public int radiusMin = 3;
    public int radiusMax = 8;

    protected int radius;

    public void SetSize(int radius)
    {
       this.radius = radius;
    }

    //Проверка генерации когда-то будет :)
    public override bool Validate()
    {

        if (positionRoomTiles.Count > radiusMin * radiusMin)
            return true;

        List<Vector2Int> OpenStates = new List<Vector2Int>();
        List<Vector2Int> ClosedStates = new List<Vector2Int>();
        if (GetTilesPos().Count == 0)
            return false;
        OpenStates.Add(GetTilesPos().First());
        while (OpenStates.Count > 0)
        {
            Vector2Int currentPos = OpenStates.First();
            OpenStates.Remove(OpenStates.First());
            ClosedStates.Add(currentPos);
            List<Vector2Int> newPoses = Direction2D.GetNewCardinalPosesFromPos(currentPos);

            if(newPoses.Count == 0 && OpenStates.Count != 0)
                return false;
            
            foreach (var pos in newPoses)
            {
                if (!(OpenStates.Contains(pos) || ClosedStates.Contains(pos)) && GetTilesPos().Contains(pos))
                {
                    OpenStates.Add(pos);
                }
            }
        }
        return true;
    }
}