using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemy2;
    public GameObject enemy3;
    public GameObject enemy4;
    public Transform spawner1;
    public Transform spawner2;
    public Transform spawner3;

    List<GameObject> Enemies = new List<GameObject>();
    float randX;
    float randY;
    Vector2 whereToSpawn;
    public float spawnRate = 10f;
    float nextSpawn = 1f;
    float probability;
    int algorithm;


    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(enemy);
        Enemies.Add(enemy2);
        Enemies.Add(enemy3);
        Enemies.Add(enemy4);
        if(PlayerData.Player == null)
        {
            algorithm = 1;
            spawnRate = 10f;
            return;
        }
        switch (PlayerData.Player.spacelvl)
        {
            case 0:
                algorithm = 1;
                break;
            case 1:
                algorithm = 1;
                spawnRate = 7f;
                break;
            case 2:
                algorithm = 2;
                spawnRate = 10f;
                break;
            case 3:
                algorithm = 2;
                spawnRate = 7f;
                break;
            case 4:
                algorithm = 3;
                spawnRate = 10f;
                break;
            case 5:
                algorithm = 3;
                spawnRate = 7f;
                break;
            default:
                algorithm = 1;
                spawnRate = 10f;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        algorithmFilter(algorithm);
    }

    void spawnAlgorithmOne() //spawner guaranteed to spawn at spawn time for every spawner
    {
        if (Time.time > nextSpawn)
        {
            if(transform.position == spawner1.position)
            {
                nextSpawn = Time.time + spawnRate;
                randX = Random.Range(-14.4f, 14.4f);
                whereToSpawn = new Vector2(randX, transform.position.y);
                GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
                eTemp.transform.parent = transform;
                eTemp.transform.parent = null;
            }
            else
            {
                nextSpawn = Time.time + spawnRate;
                randY = Random.Range(-7.5f, 7.5f);
                whereToSpawn = new Vector2(transform.position.x, randY);
                GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
                eTemp.transform.parent = transform;
                eTemp.transform.parent = null;
            }      
        }
    }
    void spawnAlgorithmTwo()// spawner has 70 percent chance to spawn an enemy on time for every spawner.
    {
        if (Time.time > nextSpawn)
        {
            if(transform.position == spawner1.position)
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
            else
            {
                nextSpawn = Time.time + spawnRate;
                randY = Random.Range(-7.5f, 7.5f);
                whereToSpawn = new Vector2(transform.position.x, randY);
                probability = Random.Range(0, 100);
                if (probability <= 70)
                {
                    GameObject eTemp = Instantiate((Enemies[Random.Range(0, Enemies.Count)]), whereToSpawn, Quaternion.identity);
                    eTemp.transform.parent = transform;
                    eTemp.transform.parent = null;
                }
            }
        }
    }
    void spawnAlgorithmThree()// spawner has 50 percent chance to spawn and enemy on time for each spawn site.
    {
        if (Time.time > nextSpawn)
        {
            if (transform.position == spawner1.position)
            {
                nextSpawn = Time.time + spawnRate;
                randX = Random.Range(-14.4f, 14.4f);
                whereToSpawn = new Vector2(randX, transform.position.y);
                probability = Random.Range(0, 100);
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
            else
            {
                nextSpawn = Time.time + spawnRate;
                randY = Random.Range(-7.5f, 7.5f);
                whereToSpawn = new Vector2(transform.position.x, randY);
                probability = Random.Range(0, 100);
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
    void algorithmFilter(int filter) //difficulty alg 3 is the lowest and one is highest.
    {
        switch(filter){
            case 1:
                spawnAlgorithmThree();
                break;
            case 2:
                spawnAlgorithmTwo();
                break;
            case 3:
                spawnAlgorithmOne();
                break;
        }
    }

}
