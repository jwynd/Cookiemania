using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;

    List<GameObject> Enemies = new List<GameObject>();
    float randX;
    Vector2 whereToSpawn;
    public float spawnRate = 5f;
    float nextSpawn = 1f;


    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(enemy);
        Enemies.Add(enemy2);
        Enemies.Add(enemy3);
        Enemies.Add(enemy4);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(-14.4f, 14.4f);
            whereToSpawn = new Vector2(randX, transform.position.y);
            GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
            eTemp.transform.parent = transform;
            eTemp.transform.parent = null;
        }
    }
}
