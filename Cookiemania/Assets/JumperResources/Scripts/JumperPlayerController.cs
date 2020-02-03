using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * REQUIRED AXES : "Pickup" mapped to something... like j?
 */


//General_score
//has trust and stuff
//Scorekeeper

//NOTE : okay can change the jumping to be NOT using a cooldown but instead
//reset the force calculation, zero out the force then add a new force when 
//jumping

//doesnt like jumping, wants full arial control, everythings too fast
//jump speed is too fast
public class JumperPlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float acceleration = 0.5f;
    public int maxJumps = 2;
    public float jumpSpeed = 3.0f;
    public Vector2 throwStrength = new Vector2(4.0f, 4.0f);
    public float maxHealth = 5;
    public float falloffDistanceMax = 15f;

    protected Rigidbody2D rb;
    protected Vector2 velocity = Vector2.zero;
    protected int jumpCount;
    protected float damageTimer = 0;
    protected float jumpCooldown = 0;
    protected float jumpCooldownMax = 0.25f;
    protected float damageTimerMax = 1.5f;
    protected float currentHealth;
    protected bool jumped = false;

    protected bool pickup = false;
    protected float maxSpeed = 5.5f;
    protected float arialManeuverability = 0.5f;
    protected float normalManeuverability = 1;
    protected bool haveItem = false;
    protected Rigidbody2D heldItemRB = null;
    protected float maxHeightReached = 0f;
    protected JumperCameraController cameraScript;
    private float movementDirection = 1;
    private float pickupTimer = 0.0f;
    private float pickupTimerMax = 0.1f;
    private Color damagedColor = Color.red;
    private Color defaultColor = Color.white;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        jumpCount = maxJumps;
        currentHealth = maxHealth;
        cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<JumperCameraController>();
    }

    // Update is called once per frame
    private void Update()
    {
        //baseline is previous velocity 
        //all my inputs that are done on a per frame basis 
        //need top be in update
        
        jumped = JumpInput();
        pickup = PickupInput();
        CheckHeightForDeath();
        // Trap();
    }

    private void CheckHeightForDeath()
    {
        maxHeightReached = rb.position.y > maxHeightReached ? rb.position.y : maxHeightReached;
        if (rb.velocity.y < 0 && falloffDistanceMax < maxHeightReached - rb.position.y)
        {
            if (cameraScript != null)
            {
                //this method also triggers scene changing
                cameraScript.PlayerDestroyed();
            }
            Destroy(gameObject);
        }
    }

    private bool PickupInput()
    {
        if (Input.GetButtonDown("Pickup")) 
        {
            if (pickup && !haveItem)
            {
                pickupTimer = pickupTimerMax;
            }
            return true;

        }
        return pickup;
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
            movementDirection = 1;
        }
        else if (inputAccel < 0)
        {
            velocity.x = Mathf.Max(inputAccel + velocity.x, -maxSpeed);
            movementDirection = -1;
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
        ThrowItem();
        Timers();
        ResetInput();
    }

    private void ResetInput()
    {
       // if (pickupTimer == 0) { pickup = false; }           
        jumped = false;
    }

    private void ThrowItem()
    {
        if (pickup && haveItem)
        {
            pickup = false;
            haveItem = false;
            Debug.Log("threw item");
            Vector2 throwTemp = throwStrength;
            throwTemp.x *= movementDirection;
            throwTemp.x += velocity.x * 0.3f;
            heldItemRB.gameObject.GetComponent<JumperPickupController>().Thrown(throwTemp);
           // throwStrength.x = Mathf.Abs(throwStrength.x);
        }
    }

    private void Movement()
    {
        //changing the direction of the throw to the last direction moved
        

        if (jumped)
        {
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
        //if (damageTimer > 0)
      //  {
            //cant go below 0
       //     damageTimer = Mathf.Max(0, damageTimer - Time.fixedDeltaTime);
       // } 
        if (pickupTimer > 0)
        {
            pickupTimer = Mathf.Max(0, pickupTimer - Time.fixedDeltaTime);
            if (pickupTimer <= 0) { pickup = false; }
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
            /*
            float x = collision.gameObject.GetComponent<SpriteRenderer>().bounds.size.x / 2;
            if (collision.transform.position.y < transform.position.y)
            {
                //check if midpoint of object is in x bounds of platform
                //probably would work much better if it checked the actual bounds
                //of the object against the bounds of the platform
                if (collision.transform.position.x + x > transform.position.x &&
                    collision.transform.position.x - x < transform.position.x)
                {
                    jumpCount = maxJumps;
                }
            }
            */
            //the below method allows wall jumps sometimes, kinda jank, but less buggy if you're 
            //cool with wall jumps
            jumpCount = transform.position.y > collision.transform.position.y ? maxJumps : jumpCount;
            collision.gameObject.GetComponent<JumperPlatformController>().Remove();
            
        }
        else if (collision.gameObject.CompareTag("Enemy")) 
        {
            TakesDamage(collision.gameObject);
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
            if (pickup && !haveItem)
            {
                //gonna parent the held item to the player then put it in the held item position
                //modified by the size of the object in future
                Transform blockTransform = collision.transform;
                blockTransform.transform.parent = transform;
                blockTransform.transform.position = transform.Find("HeldItem").transform.position;
                heldItemRB = collision.gameObject.GetComponent<Rigidbody2D>();
                pickup = false;
                haveItem = true;
            }        
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            //does the damage to the player or something           

            TakesDamage(collision.gameObject);

        }
    }

    private void TakesDamage(GameObject collision)
    {
        //requires damage and destroyable parameters in obstacle controller
        if (damageTimer <= 0)
        {
            JumperObstacleController obsControl = collision.GetComponent<JumperObstacleController>();
            if (obsControl != null)
            {
                DamageHelper(obsControl.damage);
            }
            else
            {
                JumperEnemyController enemyControl = collision.GetComponent<JumperEnemyController>();
                if (enemyControl != null)
                {
                    DamageHelper(enemyControl.damage);
                    
                }
            }
            
        }
    }
    

    private void DamageHelper(float damage)
    {
        Debug.Log("ow");
        currentHealth -= damage;
        StartCoroutine(Flasher());
        if (currentHealth <= 0)
        {
            //change scene cuz we ded
            if (cameraScript != null)
            {
                //this method also triggers scene changing
                cameraScript.PlayerDestroyed();
            }
            Destroy(gameObject);
        }
    }

    //credit: https://answers.unity.com/questions/838194/make-your-player-flash-when-hit.html
    IEnumerator Flasher()
    {
        damageTimer = damageTimerMax;
        Renderer rend = GetComponent<Renderer>();
        for (int i = 0; i < 5; i++)
        {
            rend.material.color = damagedColor;
            yield return new WaitForSeconds(.1f);
            rend.material.color = defaultColor;
            yield return new WaitForSeconds(.1f);
        }
        damageTimer = 0;
    }
}
