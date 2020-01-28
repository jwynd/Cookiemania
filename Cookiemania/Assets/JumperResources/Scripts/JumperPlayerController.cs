using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * REQUIRED AXES : "Pickup" mapped to something... like j?
 */

//NOTE : okay can change the jumping to be NOT using a cooldown but instead
//reset the force calculation, zero out the force then add a new force when 
//jumping

//doesnt like jumping, wants full arial control, everythings too fast
//jump speed is too fast
public class JumperPlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    float acceleration = 0.5f;
    int maxJumps = 2;
    float jumpSpeed = 3.0f;


    protected Rigidbody2D rb;
    protected Vector2 velocity = Vector2.zero;
    protected int jumpCount;
    protected float damageTimer = 0;
    protected float jumpCooldown = 0;
    protected float jumpCooldownMax = 0.25f;
    protected float damageTimerMax = 1.5f;
    protected float currentHealth;
    protected float maxHealth = 5;
    protected bool jumped = false;
    protected bool pickup = false;
    protected float maxSpeed = 5.5f;
    protected float arialManeuverability = 0.5f;
    protected float normalManeuverability = 1;
    protected bool haveItem = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        jumpCount = maxJumps;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    private void Update()
    {
        //baseline is previous velocity 
        //all my inputs that are done on a per frame basis 
        //need top be in update
        
        jumped = JumpInput();
        pickup = PickupInput();
        // Trap();
    }

    private bool PickupInput()
    {
        if (Input.GetAxis("Pickup") > 0) 
        {
            return true;
        }
        return false;
    }

    private void HorizontalInput()
    {
        velocity.x = rb.velocity.x;
        //can disable or enable arial controls by checking jumpcount
        if (jumpCount == maxJumps || jumped)
        {
            HorizontalInputHelper(normalManeuverability, false);
        }
        //in air speeds are lowered
        else
        {
            HorizontalInputHelper(arialManeuverability, true);
        }
    }

    private void HorizontalInputHelper(float maneuverability, bool inAir)
    {
        float inputAccel = Input.GetAxis("Horizontal") * maneuverability * acceleration;
        if (inputAccel > 0)
        {
            velocity.x = Mathf.Min(inputAccel + velocity.x, maxSpeed);
        }
        else if (inputAccel < 0)
        {
            velocity.x = Mathf.Max(inputAccel + velocity.x, -maxSpeed);
        }
        //lower their velocity if they're on the ground and not going forward or back
        else if (inputAccel == 0 && !inAir)
        {
            float sign = Mathf.Abs(velocity.x);
            if (Mathf.Abs(velocity.x) < acceleration)
            {
                velocity.x = 0;
            }
            else
            {
                //only gets to here if abs(velocity.x) is greater than a non-negative value
                //cant div by zero
                sign /= velocity.x;
                velocity.x += (-sign * maneuverability * acceleration);
            }
            
        }
    }

    private void FixedUpdate()
    {
        HorizontalInput();
        Movement();
        Timers();
    }

    private void Movement()
    {
        if (jumped)
        {
            jumped = false;
            rb.velocity = new Vector2(velocity.x, jumpSpeed * (jumpCount+1.3f));
            //rb.AddForce(new Vector2(0, jumpSpeed * (jumpCount+1.3f)), ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }
       // float vely = rb.velocity.y;
      //  rb.velocity = new Vector2(velocity.x, vely);
    }

    //to be run in fixed update
    private void Timers()
    {
        if (damageTimer > 0)
        {
            //cant go below 0
            damageTimer = Mathf.Max(0, damageTimer - Time.fixedDeltaTime);
        } 
    }

    private void Trap()
    {
        print("unimplemented, set trap");
    }

    bool JumpInput()
    {
        //better way to set the velocity of the player
        
        if (Input.GetButtonDown("Jump") )//&& jumpCooldown <= 0)
        {
            if (jumpCount > 1)
            {
                jumpCount--;
                return true;
                //rb.AddForce(new Vector2(0, maxJumpSpeed), ForceMode2D.Impulse);
            }
            else if (jumpCount > 0)
            {
                
                jumpCount--;
                return true;
                //rb.AddForce(new Vector2(0, maxJumpSpeed/1.7f), ForceMode2D.Impulse);
            }
        }
        return jumped;
       // float yvel = rb.velocity.y;
       // rb.velocity = new Vector2(Input.GetAxis("Horizontal") * acceleration, rb.velocity.y);// = Input.GetAxis("Horizontal") * acceleration * Time.deltaTime;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //platform first cuz we constantly hitting platforms
        if (collision.gameObject.CompareTag("Platform"))
        {
            //resetting the jumpcount on collision with platforms
            jumpCount = transform.position.y > collision.transform.position.y ? maxJumps : jumpCount;
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            //does the damage to the player or something           

            TakesDamage(collision);
                
        }
 
        /*
         * if (collision.gameObject.CompareTag("WorldSides"))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
         */
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pickup"))
        {
            if (pickup)
            {
                //gonna parent the held item to the player then put it in the held item position
                //modified by the size of the object in future
                Transform blockTransform = collision.transform;
                blockTransform.transform.parent = transform;
                blockTransform.transform.position = transform.Find("HeldItem").transform.position;
                haveItem = true;
            }        
        }
    }

    private void TakesDamage(Collision2D collision)
    {
        //requires damage and destroyable parameters in obstacle controller
        if (damageTimer <= 0)
        {
            JumperObstacleController obsControl = collision.gameObject.GetComponent<JumperObstacleController>();

            damageTimer = damageTimerMax;
            currentHealth -= obsControl.damage;
            if (currentHealth <= 0)
            {
            //change scene cuz we ded
            }
            if (obsControl.destroyable)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
