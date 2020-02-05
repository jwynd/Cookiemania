using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPickupController : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage = 5.0f;

    private Collider2D myCollider;
    private Rigidbody2D myRb;
    
    

    void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        myRb = gameObject.GetComponent<Rigidbody2D>();
        myCollider.isTrigger = true;
        myRb.isKinematic = true;
        myRb.gravityScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
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
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //destroys if destructible            
            JumperEnemyController script = collision.gameObject.GetComponent<JumperEnemyController>();
            if (script != null)
            {
                if (script.GetDestructable())
                {
                    Destroy(collision.gameObject);
                    
                }
                // i imagine there'd be other options at some point, like if its not destructable
                //then maybe it gives a ton of points and destroys this object, or it just does some 
                //damage to it
                else
                {
                    script.TakesDamage(damage);
                    
                }
                Destroy(gameObject);
            }
            
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
                    myRb.isKinematic = true;
                    myCollider.isTrigger = true;
                    myRb.gravityScale = 0;
                }                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //we'll just say all obstacles can be immediately destroyed
            //currently destroys obstacles it runs into even when held
            //obviously if this spawns and touches an obstacle even before the player 
            //picks it up, itll destroy it
            Destroy(collision.gameObject);
            Destroy(gameObject);

        }
    }
}
