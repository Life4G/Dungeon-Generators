using UnityEngine;
public class SquareRoom : RoomBase
{
    public int widthMin = 2;
    public int widthMax = 16;
    public int heighthMin = 2;
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

    public override bool Validate()
    {
        return true;
        //return width * height > widthMin * heighthMin;
    }
    
    //Code GetStartPoint or smth
}