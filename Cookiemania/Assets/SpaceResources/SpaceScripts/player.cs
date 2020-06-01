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

    private void Start()
    {
        
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
            
            GameObject go = Instantiate(theplayerfire, transform.position, Quaternion.identity);
            go.transform.parent = transform;
            go.transform.parent = null;
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.playerfire);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            GameObject go = Instantiate(theplayerfire, transform.position, Quaternion.identity);
            go.transform.parent = transform;
            go.transform.parent = null;
            soundmanager.Instance.PlayOneShot(soundmanager.Instance.shield);
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
        if (!General_LevelTransition.Instance)
        {
            return;
        }
        if (General_LevelTransition.Instance.LoadedScene == null)
        {
            return;
        }
        General_LevelTransition.Instance.SceneTransition(2);
        General_LevelTransition.Instance.calling();
    }
}
