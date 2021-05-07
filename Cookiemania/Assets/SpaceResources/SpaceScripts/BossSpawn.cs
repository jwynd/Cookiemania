using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject boss1;

    List<GameObject> Enemies = new List<GameObject>();
    float randX;
    Vector2 whereToSpawn;
    float spawning = 60f;
    private float secondsCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(boss1);
        if(PlayerData.Player.spacelvl <= 1)
        {
            spawning = 60f;
        } else if (PlayerData.Player.spacelvl == 2 || PlayerData.Player.spacelvl == 3)
        {
            spawning = 45f;
        } else
        {
            spawning = 30f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        secondsCount = Time.deltaTime;
        if (secondsCount > spawning)
        {
            randX = Random.Range(-14.4f, 14.4f);
            whereToSpawn = new Vector2(randX, transform.position.y);
            GameObject go = Instantiate(boss1, whereToSpawn, Quaternion.identity);
            go.transform.parent = transform;
            go.transform.parent = null;
            spawning = spawning + spawning;
        }
    }
}
