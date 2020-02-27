﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyfire : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    public float speed = 3;
    public Transform enemy;

    public Sprite PExplosionImage;
    // Start is called before the first frame update
    void Start()
    {

        enemy = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        Vector3 direction = enemy.position - transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            //SoundManager.Instance.PlayOneShot(SoundManager.Instance.playerdeath);
            col.GetComponent<SpriteRenderer>().sprite = PExplosionImage;
            Destroy(this.gameObject);
            Destroy(col.gameObject, 0.5f); //0.5f

        }

        if (col.gameObject.CompareTag("shield")) 
         {
             Destroy(this.gameObject);
             Destroy(col.gameObject);
         }

        if (col.gameObject.CompareTag("wall"))
        {
            Destroy(this.gameObject);
        }
       
        if (col.gameObject.CompareTag("fire"))
        {
            Destroy(this.gameObject);
            Destroy(col.gameObject);
        }

    }
    void OnBecomeInvisible()
    {
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, 3.5f);
    }
}
