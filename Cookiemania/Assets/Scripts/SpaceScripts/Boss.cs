using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
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
    // private Transform target;
    public Transform Player;
    private Vector2 movement;
    public float moveSpeed = 3f;
    List<Transform> moveSpots = new List<Transform>();
    private int randomSpot;
    private float waitTime;
    public float startwaitTime;
    private int bosslives = 3;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        //rigidBody.velocity = new Vector2(1, 0) * speed;
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(changeEnemySprite());
        baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
        InvokeRepeating("Launch", 4f, 3f);
        // target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        moveSpots.Add(GameObject.Find("MoveSpot").transform);
        moveSpots.Add(GameObject.Find("MoveSpot1").transform);
        moveSpots.Add(GameObject.Find("MoveSpot2").transform);
        moveSpots.Add(GameObject.Find("MoveSpot3").transform);
        moveSpots.Add(GameObject.Find("MoveSpot4").transform);
        moveSpots.Add(GameObject.Find("MoveSpot5").transform);
        moveSpots.Add(GameObject.Find("MoveSpot6").transform);
        moveSpots.Add(GameObject.Find("MoveSpot7").transform);
        moveSpots.Add(GameObject.Find("MoveSpot8").transform);
        moveSpots.Add(GameObject.Find("MoveSpot9").transform);
        moveSpots.Add(GameObject.Find("MoveSpot10").transform);
        moveSpots.Add(GameObject.Find("MoveSpot11").transform);
        moveSpots.Add(GameObject.Find("MoveSpot12").transform);

        randomSpot = Random.Range(0, moveSpots.Count);

        if(PlayerData.Player.spacelvl <= 1)
        {
            bosslives = 2;
        } else if (PlayerData.Player.spacelvl == 2)
        {
            bosslives = 3;
        } else if (PlayerData.Player.spacelvl == 3)
        {
            bosslives = 5;
        } else if (PlayerData.Player.spacelvl >= 4)
        {
            bosslives = 7;
        }
        Debug.Log(bosslives);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("fire"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }
        }

        if (col.gameObject.CompareTag("shield"))
        {
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }
            Destroy(col.gameObject);
        }

        if (col.gameObject.CompareTag("Player"))
        {
            int lives = col.gameObject.GetComponent<health>().lives;
            if (lives > 1)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            }
            else if (lives == 1)
            {
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            }
            if (lives <= 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
                col.gameObject.GetComponent<SpriteRenderer>().sprite = playerdeathImage;
            }
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }



        }
    }
    public IEnumerator changeEnemySprite()
    {
        while (true)
        {
            if (spriteRenderer.sprite == startingImage)
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
        //Vector3 direction = Player.position - transform.position;
        //loat angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rigidBody.rotation = angle;
        //direction.Normalize();
        //movement = direction;

        transform.position = Vector2.MoveTowards(transform.position, moveSpots[randomSpot].position, moveSpeed * Time.deltaTime);
        if(Vector2.Distance(transform.position, moveSpots[randomSpot].position) < 0.2f)
        {
            if(waitTime <= 0)
            {
                randomSpot = Random.Range(0, moveSpots.Count);
                waitTime = startwaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
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
        GameObject go = Instantiate(enemyFire, transform.position, Quaternion.identity);
        go.transform.parent = transform;
        go.transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }
            int lives = col.gameObject.GetComponent<health>().lives;
            if (lives > 1)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.loseheart);
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
               
            }
            else if (lives == 1)
            {
                col.gameObject.GetComponent<health>().takedamage();
                lives = col.gameObject.GetComponent<health>().lives;
            }
            if (lives <= 0)
            {
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerdies);
                col.gameObject.GetComponent<SpriteRenderer>().sprite = playerdeathImage;
                Destroy(col.gameObject, .05f); //.05f
            }

        }
        if (col.gameObject.CompareTag("fire"))
        {
            StartCoroutine(FlashSprite(spriteRenderer, 0, 255, 1, 3));
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }
            Destroy(col.gameObject);
            
        }

        if (col.gameObject.CompareTag("pierce"))
        {
            bosslives = bosslives - 1;
            if (bosslives <= 0)
            {
                Destroy(gameObject);
            }
            Destroy(col.gameObject);
        }

    }
    public static IEnumerator FlashSprite(SpriteRenderer renderer, float minAlpha, float maxAlpha, float interval, float duration)
    {
        Color colorNow = renderer.color;
        Color minColor = new Color(renderer.color.r, renderer.color.g, renderer.color.b, minAlpha);
        Color maxColor = new Color(renderer.color.r, renderer.color.g, renderer.color.b, maxAlpha);

        float currentInterval = 0;
        while (duration > 0)
        {
            float tColor = currentInterval / interval;
            renderer.color = Color.Lerp(minColor, maxColor, tColor);

            currentInterval += Time.deltaTime;
            if (currentInterval >= interval)
            {
                Color temp = minColor;
                minColor = maxColor;
                maxColor = temp;
                currentInterval = currentInterval - interval;
            }
            duration -= Time.deltaTime;
            yield return null;
        }

        renderer.color = colorNow;
    }

    void moveCharacter(Vector2 direction)
    {
        rigidBody.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
    }
}
