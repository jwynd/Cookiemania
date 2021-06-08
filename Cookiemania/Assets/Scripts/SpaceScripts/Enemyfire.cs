using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyfire : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    public float speed = 2;
    public Transform enemy;

    public Sprite PExplosionImage;
    public Sprite E2;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3.5f);
        enemy = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        Vector3 direction = enemy.position - transform.position;
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<health>().takedamage();
            Destroy(gameObject);
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

}
