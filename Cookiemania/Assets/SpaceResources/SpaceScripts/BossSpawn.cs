﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject boss1;

    List<GameObject> Enemies = new List<GameObject>();
    float randX;
    Vector2 whereToSpawn;
    float spawning = 30f;

    // Start is called before the first frame update
    void Start()
    {
        Enemies.Add(boss1);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > spawning)
        {
            randX = Random.Range(-14.4f, 14.4f);
            whereToSpawn = new Vector2(randX, transform.position.y);
            GameObject go = Instantiate(boss1, whereToSpawn, Quaternion.identity);
            go.transform.parent = transform;
            go.transform.parent = null;
            spawning = spawning + 60f;
        }
    }
}
