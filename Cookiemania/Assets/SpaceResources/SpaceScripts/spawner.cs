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
    public float spawnRate = 10f;
    float nextSpawn = 1f;
    float probability;


    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(enemy);
        Enemies.Add(enemy2);
        Enemies.Add(enemy3);
        Enemies.Add(enemy4);

        switch (PlayerData.Player.spacelvl)
        {
            case 1:
               
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        spawnAlgorithmThree();
    }

    void spawnAlgorithmOne() //spawner guaranteed to spawn at spawn time for every spawner
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

    void spawnAlgorithmTwo()// spawner has 70 percent chance to spawn an enemy on time for every spawner.
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(-14.4f, 14.4f);
            whereToSpawn = new Vector2(randX, transform.position.y);
            probability = Random.Range(0, 100);
            if (probability <= 70)
            {
                GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
                eTemp.transform.parent = transform;
                eTemp.transform.parent = null;
            }
        }
    }

    void spawnAlgorithmThree()// spawner has 50 percent chance to spawn and enemy on time for each spawn site.
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            randX = Random.Range(-14.4f, 14.4f);
            whereToSpawn = new Vector2(randX, transform.position.y);
            probability = Random.Range(0, 100);
            Debug.Log(probability);
            if (probability <= 50)
            {
                GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
                eTemp.transform.parent = transform;
                eTemp.transform.parent = null;
            }
            else
            {
                Debug.Log("no spawn this turn");
            }
        }
    }

}
