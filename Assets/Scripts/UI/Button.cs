using UnityEngine;

public class Button : MonoBehaviour
{
    //Наша тайловое поле
    [SerializeField]
    GridManager grid;
    //Сид
    [SerializeField]
    int seed;
    //Пересоздавать заново?
    [SerializeField]
    bool isNew = true;


    //Функция которая вызывается по нажатию на кнопку
    public void Press()
    {
        if (isNew)
            grid.Reload();
        else
            grid.Reload(seed);
        seed = grid.generator.GetSeed();
    }

}
