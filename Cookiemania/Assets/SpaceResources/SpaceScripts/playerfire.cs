﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerfire : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D rigidBody;
    public Sprite EExplosionImage;
    public Transform Player;
    public GameObject user;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        user = GameObject.FindGameObjectWithTag("Player");
        Vector3 direction = transform.position - Player.position;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    } 

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.gameObject.CompareTag("Enemy"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            increaseTextUIScore();

            col.GetComponent<SpriteRenderer>().sprite = EExplosionImage;
            Destroy(gameObject);
            Destroy(col.gameObject, 0.2f); //DestroyObject(col.gameObject, 0.5f)

        }

       /* if (col.gameObject.CompareTag("shield")) 
        {
            Destroy (gameObject);
            Destroy(col.gameObject);
        } */
        
    }
    void OnBecomeInvisible()
    {
        Destroy(gameObject);
    }

    void increaseTextUIScore()
    {
        var textUIComp = GameObject.Find("Score").GetComponent<Text>();
        int score = int.Parse(textUIComp.text);

        score += 10;

        if(score%100 == 0)
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.gainlife);
            user.gameObject.GetComponent<health>().gainlife();
        }

        textUIComp.text = score.ToString();
    }

    private void Update()
    {
        Destroy(this.gameObject, 2f);
    }
}
