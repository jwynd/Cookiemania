using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyfire : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    public float speed = 30;

    public Sprite PExplosionImage;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.down * speed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "player")
        {
            //SoundManager.Instance.PlayOneShot(SoundManager.Instance.playerdeath);
            col.GetComponent<SpriteRenderer>().sprite = PExplosionImage;
            Destroy(gameObject);
            DestroyObject(col.gameObject, 0.5f); //0.5f

        }

        if (col.tag == "shield") 
         {
             Destroy (gameObject);
             DestroyObject(col.gameObject);
         }

    }
    void OnBecomeInvisible()
    {
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
