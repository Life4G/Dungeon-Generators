using Assets.Scripts.Map;
using Assets.Scripts.Room;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject monsterPrefab;
    public float cooldown = 10;
    public float cooldownRange = 5;

    private GridManager gridManager;
    private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();
        timer = cooldown + Random.Range(-cooldownRange, cooldownRange);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 )
        {
            timer = cooldown + Random.Range(-cooldownRange, cooldownRange);
            SpawnMonster();
        }
    }

    private void SpawnMonster()
    {
        DungeonMap map = gridManager.GetDungeonMap();
        int myx = (int)transform.position.x;
        int myy = (int)transform.position.y;

        List<Vector2> spawnPositions = new List<Vector2>();
        for (int x = myx - 1; x <= myx + 1; x++)
            for (int y = myy - 1; y <= myy + 1; y++)
            {
                if (x == myx && y == myy) continue;
                if (map.tiles[x, y].isPassable)
                    spawnPositions.Add(new Vector2(x, y));
            }
        if (spawnPositions.Count == 0)
            return;
        var pos = spawnPositions[Random.Range(0, spawnPositions.Count)];
        Instantiate(monsterPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
    }
}
