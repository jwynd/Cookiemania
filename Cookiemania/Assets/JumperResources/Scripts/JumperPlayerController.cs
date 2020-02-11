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
    #region variables
    public float acceleration = 0.5f;
    public int maxJumps = 2;
    public float jumpMultiplier = 2f;
    
    public float jumpSpeed = 3.5f;
    public Vector2 throwStrength = new Vector2(4.0f, 4.0f);
    public float maxHealth = 5;
    public float falloffDistanceMax = 15f;

    public float flashTime = 0.15f;
    public float damageTimerMax = 1.5f;

    public float totalJumpStrength { get; private set; }

    protected Rigidbody2D rb;
    protected Vector2 velocity = Vector2.zero;
    protected int jumpCount;
    protected float damageTimer = 0;
    protected float jumpCooldown = 0;
    protected float jumpCooldownMax = 0.25f;
    
    protected float currentHealth;
    protected bool jumped = false;

    protected bool pickup = false;
    protected float maxSpeed = 5.5f;
    protected float arialManeuverability = 0.5f;
    protected float normalManeuverability = 1;
    protected bool haveItem = false;
    protected Rigidbody2D heldItemRB = null;
    protected float maxHeightReached = 0f;
    protected JumperManager jm;
    protected JumperCameraController cameraScript;
    protected float movementDirection = 1;

    protected Color damagedColor = Color.red * Color.white;
    protected Color defaultColor = Color.white;
    private bool throwStuff;
    #endregion

    #region startup
    void Start()
    {
        jm = JumperManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        jumpCount = maxJumps;
        currentHealth = maxHealth;
        cameraScript = jm.mainCamera.GetComponent<JumperCameraController>();
        totalJumpStrength = jumpMultiplier * jumpSpeed + jumpSpeed * (1+jumpMultiplier);
    }
    #endregion

    #region update
    protected void Update()
    {
        //baseline is previous velocity 
        //all my inputs that are done on a per frame basis 
        //need top be in update
        
        jumped = JumpInput();
        ItemInput();
        CheckHeightForDeath();
        // Trap();
    }

    protected void CheckHeightForDeath()
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

    bool JumpInput()
    {
        //better way to set the velocity of the player

        if (Input.GetButtonDown("Jump"))//&& jumpCooldown <= 0)
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

    protected void ItemInput()
    {
        if (Input.GetAxis("Pickup") > 0) 
        {
            pickup = true;
        }
        else
        {
            pickup = false;
        }
        if (Input.GetButtonDown("Throw"))
        {
            throwStuff = true;
        }
    }

    #endregion

    #region fixedUpdate

    protected void FixedUpdate()
    {
        HorizontalInput();
        Movement();
        ThrowItem();
        Timers();
        ResetInput();
    }

    protected void HorizontalInput()
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

    protected void HorizontalInputHelper(float maneuverability, bool inAir)
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

    protected void Movement()
    {
        //changing the direction of the throw to the last direction moved


        if (jumped)
        {
            rb.velocity = new Vector2(velocity.x, jumpSpeed * (jumpCount + jumpMultiplier));
            
            //rb.AddForce(new Vector2(0, jumpSpeed * (jumpCount+jumpMultiplier)), ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }
        // float vely = rb.velocity.y;
        //  rb.velocity = new Vector2(velocity.x, vely);
    }

    protected void ResetInput()
    {
       // if (pickupTimer == 0) { pickup = false; }           
        jumped = false;
    }

    protected void ThrowItem()
    {
        if (throwStuff && haveItem)
        {
            haveItem = false;
            Debug.Log("threw item");
            Vector2 throwTemp = throwStrength;
            throwTemp.x *= movementDirection;
            throwTemp.x += velocity.x * 0.3f;
            heldItemRB.gameObject.GetComponent<JumperPickupController>().Thrown(throwTemp);
        }
        throwStuff = false;
    }

    

    //to be run in fixed update
    protected void Timers()
    {
        //if (damageTimer > 0)
      //  {
            //cant go below 0
       //     damageTimer = Mathf.Max(0, damageTimer - Time.fixedDeltaTime);
       // } 

    }

    #endregion

    #region public
    public void PickupDestroyed()
    {
        heldItemRB = null;
        haveItem = false;
    }
    #endregion

    #region collision
    protected void OnCollisionEnter2D(Collision2D collision)
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
            if (transform.position.y > collision.transform.position.y)
            {
                jumpCount = maxJumps;
                collision.gameObject.GetComponent<JumperPlatformController>().Remove();
            }
            
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

    protected void OnTriggerStay2D(Collider2D collision)
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
        else if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Enemy"))
        {
            //does the damage to the player or something           
            TakesDamage(collision.gameObject);
        }
    }

    protected void TakesDamage(GameObject collision)
    {
        //requires damage and destroyable parameters in obstacle controller
        if (damageTimer <= 0)
        {
            JumperObstacleController obsControl = collision.GetComponent<JumperObstacleController>();
            if (obsControl != null)
            {
                DamageHelper(obsControl.GetDamage());
            }
            else
            {
                JumperEnemyController enemyControl = collision.GetComponent<JumperEnemyController>();
                if (enemyControl != null)
                {
                    DamageHelper(enemyControl.GetDamage());
                    
                }
            }
            
        }
    }
    

    protected void DamageHelper(float damage)
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
        int interval = (int)( damageTimerMax / flashTime);
        Renderer rend = GetComponent<Renderer>();
        for (int i = 0; i < interval; i++)
        {
            rend.material.color = damagedColor;
            yield return new WaitForSeconds(flashTime / 2);
            rend.material.color = defaultColor;
            yield return new WaitForSeconds(flashTime / 2);
        }
        damageTimer = 0;
    }

    #endregion

    #region stubs
    protected void Trap()
    {
        print("unimplemented, set trap");
    }
    #endregion

}
