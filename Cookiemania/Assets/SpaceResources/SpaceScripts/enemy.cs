using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    public float speed = 10;
    public Rigidbody2D rigidBody;
    public Sprite startingImage;
    public Sprite altImage;
    private SpriteRenderer spriteRenderer;
    public float secBeforeSpriteChange = .5f;
    public GameObject enemyFire;
    public float minFireRateTime = .3f;
    public float maxFireRateTime = 3.0f;
    public float baseFireWaitTime = 5.0f;
    public Sprite playerdeathImage;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(changeEnemySprite());
        baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
        InvokeRepeating("Launch", 4f, 3f);

    }

    // Turn in Opposite direction
    void Turn(int direction)
    {
        Vector2 newVelocity = rigidBody.velocity;
        newVelocity.x = speed * direction;
        rigidBody.velocity = newVelocity;
    }

    //Move down after hitting wall
    void MoveDown()
    {
        Vector2 position = transform.position;
        position.y -= 1;
        transform.position = position;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
       if(col.gameObject.name == "LeftWall")
        {
            Turn(1);
            MoveDown();
        } 

       if(col.gameObject.name == "RightWall")
        {
            Turn(-1);
            MoveDown();
        }

        if (col.gameObject.name == "playerfire")
        {
            // soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydeath);
            Destroy(gameObject);
        }
    }
       public IEnumerator changeEnemySprite()
       {
           while (true)
           {
               if(spriteRenderer.sprite == startingImage)
               {
                   spriteRenderer.sprite = altImage;
                  // soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemysound);

               }
               else
               {
                   spriteRenderer.sprite = startingImage;
                  // soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemysound2);
               }

               yield return new WaitForSeconds(secBeforeSpriteChange);
           }
       }

    private void FixedUpdate()
    {
        if(Time.time > baseFireWaitTime)
        {
            baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
            //Instantiate(enemyFire, transform.position, Quaternion.identity);
        }
    }

    void Launch()
    {
        Instantiate(enemyFire, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            //soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdeath);
            col.GetComponent<SpriteRenderer>().sprite = playerdeathImage;
            Destroy(gameObject);
            Destroy(col.gameObject, .05f); //.05f

        }
    }
}
