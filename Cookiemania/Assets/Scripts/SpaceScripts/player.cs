using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed = 20;
    public GameObject theplayerfire;
    public GameObject piercebullet;
    public GameObject theplayershield;
    public GameObject theplayershield2;
    public GameObject theplayershield3;
    public GameObject biscuit;
    public Rigidbody2D rigidBody;
    public Transform Player, direct1, direct2, direct3, direct4, direct5;
    public static int bulletlevel = 0;
    public static int bulletpiercelvl = 0;
    public static int shieldWidth = 0;
    private float cooldowntime = 2f;
    private float nextskilltime;
    private float cooldownbullets = .18f;
    private float nextbullettime;
    public Animator fireanimator;
    public Animator shieldplayer;

    

    public float animationSpeed = 10f;
    private string aniname = "bullet_ani";

    private void Awake()
    {
        fireanimator.speed = animationSpeed;
    }

    private void Start()
    {
        this.GetComponent<health>().SetDeathFunction(zerohealth);
        if (PlayerData.Player != null)
        {
            Debug.Log("This is a full game playthrough PlayerData is Active");
            bulletlevel = PlayerData.Player.GunSpread;
            bulletpiercelvl = PlayerData.Player.Pierce;
            shieldWidth = PlayerData.Player.ShieldWidth;
        }
        else
        {
            Debug.Log("This is a direct space scene playthrough PlayerData is inactive");
        }
    }
    //private void FixedUpdate()
    //{
        //float horzmove = Input.GetAxisRaw("Horizontal");
        // GetComponent<Rigidbody2D>().velocity = new Vector2(horzmove, 0) * speed;
    //}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        //Vector3 direction = transform.position - Player.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rigidBody.rotation = angle;
        //direction.Normalize();
        if (InputAxes.Instance.Jump.triggered)
        {
            if (Time.time > nextbullettime)
            {
                fireanimator.Play(aniname, -1, 0f);
                Fire();
                nextbullettime = Time.time + cooldownbullets;
            }
        }
        if (InputAxes.Instance.Action1.triggered)
        {
            if (Time.time > nextskilltime)
            {
                Shield();
                shieldplayer.Play("shieldcooldown", -1, 0f);
                nextskilltime = Time.time + cooldowntime;
            }
        }
    }

    private void Shield()
    {
        GameObject go;
        if (shieldWidth < 1)
        {
            go = Instantiate(theplayershield, transform.position, Quaternion.identity);
        }
        else if (shieldWidth == 1)
        {
            go = Instantiate(theplayershield2, transform.position, Quaternion.identity);
        }
        else
        {
            cooldowntime = 3f;
            go = Instantiate(theplayershield3, direct5.position, Quaternion.identity);
        }
        go.transform.parent = transform;
        go.transform.parent = null;
        
        soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
    }

    private void Fire()
    {
        if (bulletpiercelvl >= 1) //if the bullet pierce is upgraded the main bullet will be pierce
        {
            GameObject go = Instantiate(piercebullet, transform.position, transform.rotation);
        }
        else // fire regular bullet
        {
            GameObject go = Instantiate(theplayerfire, transform.position, transform.rotation);
            go.transform.parent = transform;
            go.transform.parent = null;
        }

        if (bulletlevel == 1)// if bullet level is one shoot a spread of 3 bullets (add 2 bullets)
        {
            cooldownbullets = .35f;
            if (bulletpiercelvl >= 2)// check for piercing bullets
            {
                GameObject d1 = Instantiate(piercebullet, direct1.position, transform.rotation);
                GameObject d2 = Instantiate(piercebullet, direct2.position, transform.rotation);
            }
            else
            {
                GameObject d1 = Instantiate(theplayerfire, direct1.position, transform.rotation);
                GameObject d2 = Instantiate(theplayerfire, direct2.position, transform.rotation);
            }
        }
        else if (bulletlevel == 2) // checks spread level
        {
            cooldownbullets = .7f;
            if (bulletpiercelvl >= 2) // checkfor pierce !! change to 3 if 3rd level
            {
                GameObject d1 = Instantiate(piercebullet, direct1.position, transform.rotation);
                GameObject d2 = Instantiate(piercebullet, direct2.position, transform.rotation);
                GameObject d3 = Instantiate(piercebullet, direct3.position, transform.rotation);
                GameObject d4 = Instantiate(piercebullet, direct4.position, transform.rotation);
            }
            else
            {
                GameObject d1 = Instantiate(theplayerfire, direct1.position, transform.rotation);
                GameObject d2 = Instantiate(theplayerfire, direct2.position, transform.rotation);
                GameObject d3 = Instantiate(theplayerfire, direct3.position, transform.rotation);
                GameObject d4 = Instantiate(theplayerfire, direct4.position, transform.rotation);
            }
        }
        soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerfire);
    }

    public void wait()
    {
        StartCoroutine(waiting());
    }
    IEnumerator waiting()
    {
        yield return new WaitForSeconds(3);
    }

    private void zerohealth()
    {
        if (biscuit)
        {
            biscuit.GetComponent<Animator>().SetTrigger("Death");
            Destroy(gameObject, 3);
            GetComponent<Collider2D>().enabled = false;
        }
    }

    void OnDestroy()
    {
       
        var manager = FindObjectOfType<spaceManager>();
        if (manager != null)
            manager.EndGame();
    }
}
