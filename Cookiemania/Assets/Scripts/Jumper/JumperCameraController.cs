﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperCameraController : MonoBehaviour
{
    // Start is called before the first frame update


    private Rigidbody2D playerRB;
    private Rigidbody2D rb;



    public Vector2 lookAhead = new Vector2(2.0f, 1.0f); 
    public Vector2 catchUpSpeed = new Vector2(1.0f, 10.0f);
    public float buffer = 1.0f;
    public float zoomSpeed = 1.0f;
    public float acquireTargetSpeed = 3.0f;
    private Vector2 targetPos = Vector2.zero;



    public bool isFollowing;

    //runs before start is run
    private void Awake()
    {

        playerRB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        if (!playerRB) { Debug.LogError("No player in game on start"); }
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void Start()
    {        
        isFollowing = true;

        rb.position = new Vector3(playerRB.position.x + lookAhead.x, playerRB.position.y + lookAhead.y, transform.position.z);
        targetPos.y = playerRB.position.y + lookAhead.y;
    }

    void FixedUpdate()
    {
        Vector2 velocity = Vector2.zero;

        if (isFollowing)
        {
            //x calculations
            if (playerRB.velocity.x > 0)
            {
                targetPos.x = playerRB.position.x + lookAhead.x;
                if (rb.position.x < targetPos.x)
                {
                    velocity.x = playerRB.velocity.x + catchUpSpeed.x;
                }
                else
                {
                    velocity.x = playerRB.velocity.x;
                }
            }
            else if (playerRB.velocity.x < 0)
            {
                targetPos.x = playerRB.position.x - lookAhead.x;
                if (rb.position.x > targetPos.x)
                {
                    velocity.x = playerRB.velocity.x - catchUpSpeed.x;
                }
                else
                {
                    velocity.x = playerRB.velocity.x;
                }
            }
            //y calculations, fuck actual gravity just need to go towards the player position + look ahead
            targetPos.y = playerRB.position.y + lookAhead.y;

            if (targetPos.y - buffer > rb.position.y )
            {
               // velocity.y = rb.velocity.y < 0 ? catchUpSpeed.y * 2: catchUpSpeed.y ;
               
                velocity.y = (targetPos.y - rb.position.y) * catchUpSpeed.y;
            }
            else if (targetPos.y + buffer < rb.position.y)
            {
               // velocity.y = rb.velocity.y < 0 ? -catchUpSpeed.y * 2 : -catchUpSpeed.y;
                velocity.y = (targetPos.y - rb.position.y) * catchUpSpeed.y;

            }
        }

        rb.velocity = velocity;
    }

    public void ZoomIn(float targetCameraSize = 3f)
    {
        StartCoroutine(ZoomCamera(targetCameraSize));
    }

    public void ZoomInToTarget(Transform t, float targetCameraSize = 3f)
    {
        StartCoroutine(ZoomCameraToTarget(t, targetCameraSize));
    }

    public void PlayerDestroyed()
    {
        isFollowing = false;
        playerRB = null;
        rb.velocity = Vector2.zero;
    }

    private IEnumerator ZoomCameraToTarget(Transform t, float size)
    {
        Camera myCam = GetComponent<Camera>();
        PlayerDestroyed();
        while (myCam.orthographicSize > size && rb.position != (Vector2)t.position)
        {          
            myCam.orthographicSize = Mathf.MoveTowards(myCam.orthographicSize, size, zoomSpeed * Time.fixedDeltaTime);
            rb.position = Vector2.MoveTowards(rb.position, t.position, acquireTargetSpeed * Time.fixedDeltaTime);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    private IEnumerator ZoomCamera(float targetSize)
    {
        Camera myCam = GetComponent<Camera>();
        while (myCam.orthographicSize > targetSize)
        {
            yield return new WaitForSeconds(Time.fixedDeltaTime);
            myCam.orthographicSize = Mathf.MoveTowards(myCam.orthographicSize, targetSize, zoomSpeed * Time.fixedDeltaTime);
        }
    }
}
