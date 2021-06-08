using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    private const float TIME_UNTIL_FIRST_SHOT = 4f;

    public float speed = 10;
    private Rigidbody2D rigidBody;
    public float secBeforeSpriteChange = .5f;
    public GameObject enemyFire;
    public float minFireRateTime = 5.0f;
    public float maxFireRateTime = 10.0f;
    public static float baseFireWaitTime = 3f;
    public Transform Player;
    private Transform location1;
    private Transform location2;
    public GameObject pos1;
    public GameObject pos2;
    private Vector2 movement;
    private Vector2 movement2;
    private Vector2 movement3;
    private int rand;
    public float moveSpeed = 1.5f;
    private Animator chargeanimator;

    private bool Death = false;
    private bool launch = false;

    void Start()
    {
        rand = Random.Range(0, 100);
        Player = GameObject.FindGameObjectWithTag("treasure").GetComponent<Transform>();
        location1 = pos1.GetComponent<Transform>();
        location2 = pos2.GetComponent<Transform>();
        rigidBody = GetComponent<Rigidbody2D>();
        rigidBody.velocity = new Vector2(1, 0) * speed;
        baseFireWaitTime = baseFireWaitTime + Random.Range(minFireRateTime, maxFireRateTime);
        InvokeRepeating("Launch", TIME_UNTIL_FIRST_SHOT, baseFireWaitTime);
        chargeanimator = GetComponent<Animator>();
    }

    public void death() //test as a couroutine to see if that triggers next
    {
        if (Death) return;
        chargeanimator.SetBool("moving", false);
        chargeanimator.SetTrigger("Death");
        chargeanimator.ResetTrigger("fire");
        GetComponent<Collider2D>().enabled = false;
        CancelInvoke();
        Death = true;
        enabled = false;
        rigidBody.velocity = Vector3.zero;
        Destroy(gameObject, 1f);
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
        if (launch) launch = false;
        else chargeanimator.SetBool("moving", true);
    }

    private void FixedUpdate()
    {
        moveCharacter(movement, movement2, movement3);
    }

    void Launch()
    {
        chargeanimator.SetTrigger("fire");
        chargeanimator.SetBool("moving", false);
        GameObject go = Instantiate(enemyFire, transform.position, Quaternion.identity);
        launch = true;
        go.transform.parent = transform;
        go.transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (Death) return;
        if (col.gameObject.CompareTag("fire"))
        {
            col.gameObject.GetComponent<playerfire>().increaseTextUIScore();
            Destroy(col.gameObject);
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
            death();
        }
        else if (col.gameObject.CompareTag("pierce"))
        {
            col.gameObject.GetComponent<pierce>().increaseTextUIScore();
            death();
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.enemydies);
        }
        else if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<health>().takedamage();
            death();
        }

    }

    void moveCharacter(Vector2 direction, Vector2 direction2, Vector2 direction3)
    {
        if (GameObject.Find("lag(Clone)") != null || GameObject.Find("bosscookie(Clone)") != null)
        {
            GameObject.Find("space background").GetComponent<Renderer>().material.color =
                Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 5));
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
                GameObject.Find("space background").GetComponent<Renderer>().material.color =
                    Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time, 10));
            }
            rigidBody.MovePosition((Vector2)transform.position + (direction * moveSpeed * Time.deltaTime));
            GameObject.Find("space background").GetComponent<Renderer>().material.color = Color.white;
        }
    }
}
