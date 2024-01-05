using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Класс генерации квадратной комнаты
//В процессе она квадратной быть перестанет
public class PreGenSquareRoom : Room
{
    public int widthMin = 5;
    public int widthMax = 16;
    public int heighthMin = 5;
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

}