//Класс круглой комнаты
public class CircleRoom : RoomBase
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
        return true;
    }
}