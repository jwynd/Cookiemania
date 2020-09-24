using System.Collections;
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
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
            col.gameObject.GetComponent<health>().takedamage();
            int lives = col.gameObject.GetComponent<health>().lives;
            if (lives <= 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
                col.gameObject.GetComponent<SpriteRenderer>().sprite = PExplosionImage;
                Destroy(gameObject);
                Destroy(col.gameObject, .05f); //.05f
            }

        }

        if (col.gameObject.CompareTag("shield")) 
         {
             Destroy(this.gameObject);
            // Destroy(col.gameObject);
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
        if (col.gameObject.CompareTag("pierce"))
        {
            Destroy(this.gameObject);
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
