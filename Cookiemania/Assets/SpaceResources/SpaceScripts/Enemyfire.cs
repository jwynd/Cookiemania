using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemyfire : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    public float speed = 5;

    public Sprite PExplosionImage;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector2.down * speed;
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

    }
    void OnBecomeInvisible()
    {
        GameObject.Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        Destroy(this.gameObject, 1.5f);
    }
}
