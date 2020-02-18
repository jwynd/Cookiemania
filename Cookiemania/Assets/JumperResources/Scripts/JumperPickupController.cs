﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPickupController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    protected float damage = 5.0f;
    [SerializeField]
    protected float explosionTimer = 0.75f;
    [SerializeField]
    protected int explosionFlashCount = 3;
    [SerializeField]
    protected int explosionFrames = 5;
    [SerializeField]
    protected float explosionSize = 3f;

    protected Collider2D myCollider;
    protected Rigidbody2D myRb;
    protected float parentLeft;
    protected float parentRight;
    private bool exploding;

    protected void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        myRb = gameObject.GetComponent<Rigidbody2D>();
        myCollider.isTrigger = true;
        myRb.isKinematic = true;
        myRb.gravityScale = 0;
        //change this to x when we switch sprites lol
        JumperPlatformController dad = transform.parent.GetComponent<JumperPlatformController>();

        Vector3 pBounds = dad.GetHorizontalBounds();

        //get info from parent, set parent child to this, then 
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        transform.position = new Vector2(Random.Range(parentLeft, parentRight), transform.position.y);
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {

    }



    //unparent, then activate gravity, kinematics and enable normal collisions
    //would probably want an animation that shows the trap/bomb is active now
    public void Thrown(Vector2 strength)
    {

        transform.parent = null;
        myRb.isKinematic = false;
        myCollider.isTrigger = false;
        myRb.gravityScale = 1;
        myRb.AddForce(strength, ForceMode2D.Impulse);
        RemoveFromParent();
        StartCoroutine(Explode());
    }

    protected IEnumerator Explode()
    {
        exploding = true;
        yield return new WaitForSeconds(explosionTimer);
        for (int i = 0; i < explosionFlashCount; i++)
        {
            transform.localScale *= explosionSize;
            GetComponent<BoxCollider2D>().size *= explosionSize;
            yield return new WaitForSeconds(Time.fixedDeltaTime * explosionFrames);
            transform.localScale /= explosionSize;
            GetComponent<BoxCollider2D>().size /= explosionSize;
            yield return new WaitForSeconds(Time.fixedDeltaTime * explosionFrames);
        }
        Destroy(gameObject);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {           
            CollisionHelper(collision.gameObject);          
        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            float x = collision.gameObject.GetComponent<Collider2D>().bounds.size.x / 2;
            float myX = GetComponent<Collider2D>().bounds.size.x / 2;
            if (collision.transform.position.y < transform.position.y)
            {
                //check if midpoint of object is in x bounds of platform
                if (collision.transform.position.x + x > transform.position.x - myX && 
                    collision.transform.position.x - x < transform.position.x + myX)
                {
                    myRb.velocity = Vector2.zero;
                    myRb.isKinematic = false;
                    myCollider.isTrigger = true;
                    myRb.gravityScale = 0;
                }                
            }
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            CollisionHelper(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            CollisionHelper(collision.gameObject);
        }
    }

    protected void RemoveFromParent()
    {
        if (transform.parent != null)
        {
            if (transform.parent.TryGetComponent<JumperPlayerController>(out JumperPlayerController p))
            {
                p.PickupDestroyed();
            }
        }
    }

    protected void CollisionHelper(GameObject collided)
    {
        JumperPlatformAttachables script = collided.GetComponent<JumperPlatformAttachables>();
        if (script != null)
        {
            if (!script.IsIndestructable())
            {
                script.TakesDamage(damage);
            }
            if (!exploding)
            {
                RemoveFromParent();
                Destroy(gameObject);
            } 
        }
    }
}
