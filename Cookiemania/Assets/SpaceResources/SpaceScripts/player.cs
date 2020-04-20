using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed = 20;
    public GameObject theplayerfire;
    public GameObject theplayershield;
    public Rigidbody2D rigidBody;
    public Transform Player;

    private void FixedUpdate()
    {
        //float horzmove = Input.GetAxisRaw("Horizontal");
       // GetComponent<Rigidbody2D>().velocity = new Vector2(horzmove, 0) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = transform.position - Player.position;
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //rigidBody.rotation = angle;
        //direction.Normalize();
        if (Input.GetButtonDown("Jump"))
        {
            
            Instantiate(theplayerfire, transform.position, Quaternion.identity);
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerfire);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(theplayershield, transform.position, Quaternion.identity);
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
        }
    }

    void OnDestroy()
    {
        GameObject temp = GameObject.Find("LevelController");
        if (temp)
        {
            temp.GetComponent<General_LevelTransition>().returnDesktop();
        }
    }
}
