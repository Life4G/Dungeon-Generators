using Assets.Scripts.Fraction;
using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEngine;


public class SceneObjectManager : MonoBehaviour
{
    public List<SceneObject> sceneObjects;
    public DungeonRoomManager roomManager;
    public FractionManager fractionManager;
    public RoomStyleManager roomStyleManager;

    public void CalculateObjectsForRooms()
    {
        foreach (SceneObject sceneObject in sceneObjects)
        {
            if (sceneObject.min > sceneObject.max || sceneObject.min < 0 || sceneObject.max < 0 || sceneObject.maxRoom < 0 ||
                sceneObject.prefab == null ||
                sceneObject.fractionIds < 0 || sceneObject.styleIds <0)
            {
                Debug.Log($"SceneObject with name {sceneObject.name} has incorrect parametres.");
            }
            else
            {
                int objectsToSpawn = Random.Range(sceneObject.min, sceneObject.max);
                List<int> fractions = sceneObject.GetFractionIds();
                List<int> styles = sceneObject.GetStyleIds();

                List<DungeonRoom> rooms = new List<DungeonRoom>();
                foreach (DungeonRoom room in roomManager.rooms)
                {
                    if ((fractions.Contains(room.fractionIndex) || sceneObject.fractionIds == 0) && (styles.Contains(room.styleId) || sceneObject.styleIds == 0) && !room.isCorridor)
                        rooms.Add(room);
                }
                    foreach (DungeonRoom room in rooms)
                    {
                        for (int objectsInRoom = Random.Range(1, sceneObject.maxRoom); objectsInRoom > 0 && objectsToSpawn > 0; objectsInRoom--, objectsToSpawn--)
                            Instantiate(sceneObject.prefab, new Vector3(room.centerX, room.centerY, 0), Quaternion.identity);
                    }
            }
        }
    }
}