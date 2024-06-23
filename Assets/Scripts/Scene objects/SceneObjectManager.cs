using Assets.Scripts.Fraction;
using Assets.Scripts.Map;
using Assets.Scripts.Room;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class SceneObjectManager : MonoBehaviour
{
    public List<SceneObject> sceneObjects;
    public DungeonRoomManager roomManager;
    public FractionManager fractionManager;
    public RoomStyleManager roomStyleManager;
    public bool useCSP;

    public void CalculateObjectsForRooms()
    {
        if (useCSP) 
        {
            Dictionary<DungeonRoom, SceneObject> AllPairs = new Dictionary<DungeonRoom, SceneObject>();
            int RealRoomNumber = roomManager.CountNonCorridorRooms();
            AllPairs = SetObjects(AllPairs, roomManager.GetGraph(), RealRoomNumber, sceneObjects);
            if (AllPairs == null)
                Debug.Log("Невозможно разместить фракции методом CSP");
            else
            {
                foreach (var pair in AllPairs)
                {
                    for (int objectsInRoom = Random.Range(1, pair.maxRoom); objectsInRoom > 0 && objectsToSpawn > 0; objectsInRoom--, objectsToSpawn--)
                        Instantiate(sceneObject.prefab, CalculatePos(pair.Key), Quaternion.identity);
                }
            }
        }
        else
        foreach (SceneObject sceneObject in sceneObjects)
        {
            if (sceneObject.min > sceneObject.max || sceneObject.min < 0 || sceneObject.max < 0 || sceneObject.maxRoom < 0 ||
                sceneObject.prefab == null ||
                sceneObject.fractionIds < -1 || sceneObject.styleIds <-1)
            {
                Debug.Log($"SceneObject with name {sceneObject.name} has incorrect parametres.");
            }
            else
            {
                int objectsToSpawn = Random.Range(sceneObject.min, sceneObject.max);
                List<int> fractions;
                if (sceneObject.fractionIds!=0)
                    fractions = sceneObject.GetFractionIds();
                else 
                    fractions = new List<int>();
                List<int> styles;
                if (sceneObject.styleIds != 0)
                    styles = sceneObject.GetStyleIds();
                else
                    styles = new List<int>();

                List<DungeonRoom> rooms = new List<DungeonRoom>();
                foreach (DungeonRoom room in roomManager.rooms)
                {
                    if ((fractions.Contains(room.fractionIndex) || sceneObject.fractionIds == -1) && (styles.Contains(room.styleId) || sceneObject.styleIds == -1) && !room.isCorridor)
                        rooms.Add(room);
                }
                    foreach (DungeonRoom room in rooms)
                    {
                        for (int objectsInRoom = Random.Range(1, sceneObject.maxRoom); objectsInRoom > 0 && objectsToSpawn > 0; objectsInRoom--, objectsToSpawn--)
                            Instantiate(sceneObject.prefab, CalculatePos(room), Quaternion.identity);
                    }
            }
        }
    }

    private Vector3 CalculatePos(DungeonRoom room)
    {
        Vector3 pos= new Vector3();
        var gridManager = GameObject.FindObjectOfType<GridManager>();
        var dungeonMap = gridManager.GetDungeonMap();

        bool passible = false;
        while(!passible)
        {

            pos.x = Random.Range(room.centerX -room.width/2, room.centerX + room.width/2);
            pos.y = Random.Range(room.centerY - room.height/2, room.centerY + room.height/2);
            DungeonTile tile = dungeonMap.GetTile((int)pos.x, (int)pos.y);
            passible = tile.roomIndex >=0 && tile.isPassable;
        }
        return pos;
    }

    private Dictionary<DungeonRoom, SceneObject> SetObjects(Dictionary<DungeonRoom, SceneObject> AllPairs, int[,] connections, int RealRoomNumber, List<SceneObject> sceneObjects)
    {
        return AllPairs;
    }
}