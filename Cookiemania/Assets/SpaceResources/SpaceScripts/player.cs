using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public float speed = 20;
    public GameObject theplayerfire;
    private void FixedUpdate()
    {
        float horzmove = Input.GetAxisRaw("Horizontal");
        GetComponent<Rigidbody2D>().velocity = new Vector2(horzmove, 0) * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Instantiate(theplayerfire, transform.position, Quaternion.identity);
            //SoundManager.Instance.PlayOneShot(SoundManager.Instance.playerfire);
        }
    }

    void OnDestroy()
    {
        GameObject.Find("LevelController").GetComponent<General_LevelTransition>().returnDesktop();
    }
}
