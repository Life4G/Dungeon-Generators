using System.Collections;
using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

public class WorldUpdater : MonoBehaviour
{
    public float updateInterval = 1.0f; // Интервал обновления в секундах
    private float lastUpdateTime;
    private World world;

    void Awake()
    {
        world = World.Default;
        world.UpdateByUnity = false; // Отключаем автоматическое обновление
        lastUpdateTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            world.Update(Time.deltaTime);
            world.Commit(); // Применить все изменения
            lastUpdateTime = Time.time;
        }
    }
}

