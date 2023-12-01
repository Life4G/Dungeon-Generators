using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Класс генерации квадратной комнаты
//В процессе она квадратной быть перестанет
public class PreGenSquareRoom : RoomBase
{
    public int widthMin = 3;
    public int widthMax = 16;
    public int heighthMin = 3;
    public int heighthMax = 16;

    protected int width;
    protected int height;

    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    public void SetSize(Vector2Int size)
    {
        width = size.x; height = size.y;
    }

    //Проверка генерации когда-то будет :)
    public override bool Validate()
    {

        if (positionRoomTiles.Count > widthMin * heighthMin)
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

            if (newPoses.Count == 0 && OpenStates.Count != 0)
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