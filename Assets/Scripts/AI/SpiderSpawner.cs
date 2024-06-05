using UnityEngine;
using Scellecs.Morpeh;
using System.Collections.Generic;
using Assets.Scripts.Room;
using ECS;
using Scellecs.Morpeh.Providers;

public class SpiderSpawner : MonoBehaviour
{
    public GameObject spiderPrefab; // Префаб паука
    public DungeonRoomManager dungeonRoomManager; // Ссылка на менеджер комнат

    public int n; 

    private List<Vector2Int> roomCenters; // Список центров комнат

    void Start()
    {
        // Получение центров всех комнат
        roomCenters = GetRoomCenters(dungeonRoomManager.rooms);

        // Спавн пауков в случайных комнатах
        SpawnSpiders(n); // Например, спавним 5 пауков
    }

    private List<Vector2Int> GetRoomCenters(DungeonRoom[] rooms)
    {
        List<Vector2Int> centers = new List<Vector2Int>();
        foreach (var room in rooms)
        {
            int centerX = Mathf.RoundToInt(room.centerX);
            int centerY = Mathf.RoundToInt(room.centerY);
            centers.Add(new Vector2Int(centerX, centerY));
        }
        return centers;
    }

    private void SpawnSpiders(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (roomCenters.Count == 0) break;

            // Выбор случайной комнаты
            int randomIndex = Random.Range(0, roomCenters.Count);
            Vector2Int spawnPosition = roomCenters[randomIndex];
            roomCenters.RemoveAt(randomIndex);

            // Создание экземпляра паука
            GameObject spider = Instantiate(spiderPrefab, new Vector3(spawnPosition.x, spawnPosition.y, 0), Quaternion.identity);

            // Получение сущности из EntityProvider
            var entityProvider = spider.GetComponent<EntityProvider>();
            
            var entity = entityProvider.Entity;
            
            // Инициализация компонента позиции
            var positionStash = World.Default.GetStash<PositionComponent>();
            if (positionStash.Has(entity))
            {
                ref var positionComponent = ref positionStash.Get(entity);
                positionComponent.position = spawnPosition;
                positionStash.Set(entity, positionComponent);
            }

            // Добавляем EntityProviderComponent
            var providerStash = World.Default.GetStash<EntityProviderComponent>();
            ref var providerComponent = ref providerStash.Add(entity);
            providerComponent.entityProvider = entityProvider;


        }
    }
}
