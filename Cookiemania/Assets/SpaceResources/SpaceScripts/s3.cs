using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s3 : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;

    List<GameObject> Enemies = new List<GameObject>();
    float randY;
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
            randY = Random.Range(-5f, 5f);
            whereToSpawn = new Vector2(transform.position.x, randY);
            Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
        }
    }
}
