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
    private Transform target;
    public Transform Player;
    private Vector2 movement;
    public float moveSpeed = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(changeEnemySprite());
        baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
        InvokeRepeating("Launch", 4f, 3f);
       // target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }


    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("fire"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            Destroy(gameObject);
        }

        if (col.gameObject.CompareTag("shield"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            Destroy(gameObject);
        }

        if (col.gameObject.CompareTag("Player"))
        {
            int lives = col.gameObject.GetComponent<health>().lives;
            if(lives > 1)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            } else if(lives == 1) {
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            }
            if(lives <= 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
                col.gameObject.GetComponent<SpriteRenderer>().sprite = playerdeathImage;
                Destroy(gameObject);
                Destroy(col.gameObject, .05f); //.05f
            }

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

    void Update()
    {
        Vector3 direction = Player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rigidBody.rotation = angle;
        direction.Normalize();
        movement = direction;
    }
    private void FixedUpdate()
    {
        moveCharacter(movement);
        if (Time.time > baseFireWaitTime)
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

    void moveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }
}
