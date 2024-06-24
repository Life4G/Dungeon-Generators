using System;
using Scellecs.Morpeh;
using UnityEngine;
using ECS;
using System.Collections.Generic;
using Assets.Scripts.Room;

[CreateAssetMenu(fileName = "Wander", menuName = "BehaviorTree/ActionDelegates/Wander")]
public class Wander : ActionDelegate
{
    private DungeonRoomManager dungeonRoomManager; // Ссылка на менеджер комнат

    private List<Vector2Int> roomCenters;
    private Dictionary<Entity, Vector2Int> entityTargets = new Dictionary<Entity, Vector2Int>();

    private void OnEnable()
    {
    }

    public override NodeState Execute(Entity entity)
    {
        if (dungeonRoomManager == null)
        {
            dungeonRoomManager = FindObjectOfType<DungeonRoomManager>();
            roomCenters = GetRoomCenters(dungeonRoomManager.rooms);
            Debug.Log("Список центров комнат инициализирован.");
        }

        var moveStash = World.Default.GetStash<MoveComponent>();
        var positionStash = World.Default.GetStash<PositionComponent>();

        if (!positionStash.Has(entity))
        {
            return NodeState.FAILURE;
        }

        ref var positionComponent = ref positionStash.Get(entity);

        Vector2Int targetPosition;
        int maxAttempts = 10;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            if (!entityTargets.TryGetValue(entity, out targetPosition) || positionComponent.position == targetPosition)
            {
                // Если цели нет или цель достигнута, выбираем новую
                targetPosition = roomCenters[UnityEngine.Random.Range(0, roomCenters.Count)];
                entityTargets[entity] = targetPosition;
                //Debug.Log("Новая цель для сущности: " + targetPosition);
            }

            List<Vector2Int> path = PathfindingAStar.FindPath(positionComponent.position, targetPosition, MoveSystem.map);

            if (path != null && path.Count > 0)
            {
                var moveComponent = new MoveComponent
                {
                    path = path
                };

                moveStash.Set(entity, moveComponent);
                return NodeState.RUNNING;
            }
            else
            {
                Debug.LogWarning("Не удалось найти путь до цели: " + targetPosition + ". Попробуем другую цель.");
                attempts++;
                // Удаляем текущую цель, чтобы выбрать новую на следующей итерации
                entityTargets.Remove(entity);
            }
        }

        Debug.LogError("Не удалось найти путь после нескольких попыток.");
        return NodeState.FAILURE;
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
}
