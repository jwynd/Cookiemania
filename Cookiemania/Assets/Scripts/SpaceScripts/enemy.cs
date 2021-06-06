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
    public float minFireRateTime = 5.0f;
    public float maxFireRateTime = 10.0f;
    public static float baseFireWaitTime = 3f;
    public Sprite playerdeathImage;
    public Sprite playerdeath2;
    public Sprite enemydeathimage1;
    public Sprite enemydeathimage2;
    private Transform target;
    public Transform Player;
    public Transform location1;
    public Transform location2;
    public GameObject pos1;
    public GameObject pos2;
    private Vector2 movement;
    private Vector2 movement2;
    private Vector2 movement3;
    private int rand;
    public float moveSpeed = 1.5f;
    public Animator chargeanimator;

    //public float animationSpeed = 10f;
    public string aniname;
    public string idle;
    public string deathani;


    // Start is called before the first frame update
    void Start()
    {
        //chargeanimator.speed = animationSpeed;
        chargeanimator.Play(idle);
        rand = Random.Range(0, 100);
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        location1 = pos1.GetComponent<Transform>();
        location2 = pos2.GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(changeEnemySprite());
        baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
        InvokeRepeating("Launch", 4f, baseFireWaitTime);
       // target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }


    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("fire"))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = enemydeathimage1;
            gameObject.GetComponent<SpriteRenderer>().sprite = enemydeathimage2;
            death();
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            Destroy(gameObject);
        }

        if (col.gameObject.CompareTag("pierce"))
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = enemydeathimage1;
            gameObject.GetComponent<SpriteRenderer>().sprite = enemydeathimage2;
            death();
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            Destroy(gameObject);
        }

        //if (col.gameObject.CompareTag("shield"))
        //{
        //    soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
        //    Destroy(gameObject);
            
       // }

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
                death();

                Destroy(gameObject);
                Destroy(col.gameObject, .05f); //.05f
            }

        }
    }
    public void death() //test as a couroutine to see if that triggers next
    {
        chargeanimator.SetBool("moving", false);
        chargeanimator.SetTrigger("Death");
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
        Vector3 direction2 = transform.position - Player.position;
        Vector3 direction3 = location2.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rigidBody.rotation = angle;
        direction.Normalize();
        direction2.Normalize();
        direction3.Normalize();
        movement = direction;
        movement2 = direction2;
        movement3 = direction3;
        

    }
    private void FixedUpdate()
    {
        Debug.Log("move character triggered");
        moveCharacter(movement, movement2, movement3);
        if (Time.time > baseFireWaitTime)
        {
            baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
            //Instantiate(enemyFire, transform.position, Quaternion.identity);
        }
    }

    void Launch()
    {
        chargeanimator.SetBool("moving", false);
        chargeanimator.SetTrigger("fire");
        GameObject go = Instantiate(enemyFire, transform.position, Quaternion.identity);
        go.transform.parent = transform;
        go.transform.parent = null;
        chargeanimator.ResetTrigger("fire");
        chargeanimator.SetBool("moving", true);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            int lives = col.gameObject.GetComponent<health>().lives;
            if (lives > 1)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
                Destroy(gameObject);
            }
            else if (lives == 1)
            {
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            }
            if (lives <= 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
                col.gameObject.GetComponent<Animator>().SetTrigger("Death");
                Destroy(gameObject);
                Destroy(col.gameObject, .05f); //.05f
            }

        }

    }

    void moveCharacter(Vector2 direction, Vector2 direction2, Vector2 direction3)
    {
        if (GameObject.Find("lag(Clone)") != null || GameObject.Find("bosscookie(Clone)") != null)
        {
            GameObject.Find("space background").GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 5));
            Debug.Log("boss on screen");
            if (rand >= 50)
            {
                rigidBody.velocity = Vector3.zero;
                transform.position = Vector2.MoveTowards(transform.position, location1.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                rigidBody.velocity = Vector3.zero;
                transform.position = Vector2.MoveTowards(transform.position, location2.position, moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (GameObject.Find("space background").GetComponent<Renderer>().material.color != Color.white)
            {
                GameObject.Find("space background").GetComponent<Renderer>().material.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 10));
            }
            
            Debug.Log("boss not on screen");
            rigidBody.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
            GameObject.Find("space background").GetComponent<Renderer>().material.color = Color.white;
        }
    }


}
