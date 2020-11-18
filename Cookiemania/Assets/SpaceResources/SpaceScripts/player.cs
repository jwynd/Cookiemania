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
    public Rigidbody2D rigidBody;
    public Transform Player, direct1, direct2, direct3, direct4, direct5;
    public static int bulletlevel = 0;
    public static int bulletpiercelvl = 0;
    public static int shieldWidth = 2;

    private void Start()
    {
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
    private void FixedUpdate()
    {
        //float horzmove = Input.GetAxisRaw("Horizontal");
       // GetComponent<Rigidbody2D>().velocity = new Vector2(horzmove, 0) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }
        Vector3 direction = transform.position - Player.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rigidBody.rotation = angle;
        //direction.Normalize();
        if (Input.GetButtonDown("Jump"))
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
            } else if (bulletlevel == 2) // checks spread level
            {
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
        if (Input.GetButtonDown("Fire2"))
        {
            if (shieldWidth < 1)
            {
                GameObject go = Instantiate(theplayershield, transform.position, Quaternion.identity);
                go.transform.parent = transform;
                go.transform.parent = null;
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
            } else if (shieldWidth == 1)
            {
                GameObject go = Instantiate(theplayershield2, transform.position, Quaternion.identity);
                go.transform.parent = transform;
                go.transform.parent = null;
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
            } else if (shieldWidth == 2)
            {
                GameObject go = Instantiate(theplayershield3, direct5.position, Quaternion.identity);
                go.transform.parent = transform;
                go.transform.parent = null;
                soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
            }
            else
            {
                Debug.Log("ERROR");
                Debug.Log(shieldWidth);
            }
        }
    }

    public void wait()
    {
        StartCoroutine(waiting());
    }
    IEnumerator waiting()
    {
        yield return new WaitForSeconds(3);
    }

    void OnDestroy()
    {
        FindObjectOfType<spaceManager>().EndGame();
    }
}
