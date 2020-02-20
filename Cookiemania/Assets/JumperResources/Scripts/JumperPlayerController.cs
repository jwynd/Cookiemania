using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * REQUIRED AXES : "Pickup" mapped to something... like j?
 * "Throw" mapped to something else
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
    public float jumpSpeed = 9f;
    public float airJumpSpeed = 6f;
    public LayerMask groundLayer;
    public Vector2 throwStrength = new Vector2(4.0f, 4.0f);
    public float maxHealth = 5;
    public float flashTime = 0.15f;
    public float damageTimerMax = 1.5f;
    public AudioClip jumpSound;
    public JumperSelfDestruct jumpParticles;
    [Tooltip("Axis for left and right movements")]
    public string horizontalAxis = "Horizontal";
    [Tooltip("Axis for jumping")]
    public string jumpAxis = "Jump";
    [Tooltip("Axis for picking up items")]
    public string pickupAxis = "Pickup";
    [Tooltip("Axis for throwing items")]
    public string throwAxis = "Throw";
    [Tooltip("EMPTY INPUT axis, should still exist but no keys mapped to it")]
    public string dummyAxis = "Dummy";

    protected Rigidbody2D rb;
    protected Renderer rend;
    protected Vector2 velocity = Vector2.zero;
    protected bool grounded = true;
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
    protected AudioSource audioPlayer = null;
    protected float maxHeightReached = 0f;
    protected float points = 0f;
    protected JumperManagerGame jm;
    protected float movementDirection = 1;
    protected Vector3 originalScale;
    protected Color damagedColor = Color.red * Color.white;
    protected Color defaultColor = Color.white;
    protected bool throwStuff;
    protected bool canAirJump = true;
    protected string groundTag;
    protected string enemyTag;
    protected string obstacleTag;
    protected string collectiblesTag;
    protected string myTag;

    #endregion

    #region startup
    private void Awake()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        audioPlayer = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        rb.velocity = Vector2.zero;
        currentHealth = maxHealth;
    }
    void Start()
    {
        jm = JumperManagerGame.Instance;
        GetTags();
    }

    void GetTags()
    {
        groundTag = jm.GetGroundTag();
        myTag = jm.GetPlayerTag();
        collectiblesTag = jm.GetCollectiblesTag();
        enemyTag = jm.GetEnemyTag();
        obstacleTag = jm.GetObstacleTag();
        gameObject.tag = myTag;
    }
    #endregion

    #region update
    protected void Update()
    {
        //baseline is previous velocity 
        //all my inputs that are done on a per frame basis 
        //need top be in update
        
        jumped = JumpInput(jumpAxis);
        ItemInput(pickupAxis, throwAxis);
        // Trap();
    }

    bool JumpInput(string jump)
    {
        //better way to set the velocity of the player
        /*
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
        */

        if (Input.GetButtonDown(jump))
        {
            if (grounded || canAirJump)
            {
                return true;
            }
        }
        return jumped;

    }

    protected void ItemInput(string pickupAx, string throwAx)
    {
        if (Input.GetAxis(pickupAx) > 0) 
        {
            pickup = true;
        }
        else
        {
            pickup = false;
        }
        if (Input.GetButtonDown(throwAx))
        {
            throwStuff = true;
        }
    }

    #endregion

    #region fixedUpdate

    protected void FixedUpdate()
    {
        UpdateHeight();
        CheckGrounded();
        HorizontalInput(horizontalAxis);
        Movement();
        ThrowItem();
        //currently, no timers in fixed update
        Timers();
    }

    void UpdateHeight()
    {
        maxHeightReached = transform.position.y > maxHeightReached ? transform.position.y : maxHeightReached;
    }

    private void CheckGrounded()
    {
        float offset = rend.bounds.extents.x;
        float yoffset = rend.bounds.extents.y * 1.02f;
        grounded = Physics2D.OverlapArea(new Vector2(transform.position.x - offset, transform.position.y - yoffset), 
            new Vector2(transform.position.x + offset, transform.position.y - yoffset + 0.01f), groundLayer);
        if (grounded)
        {
            canAirJump = true;
        }
    }

    protected void HorizontalInput(string axis)
    {
        velocity.x = rb.velocity.x;
        //can disable or enable arial controls by checking jumpcount
        //if (jumpCount == maxJumps || jumped)
        if (grounded && !jumped)
        {
            
            HorizontalInputHelper(axis, normalManeuverability, false);
        }
        //in air speeds are lowered
        else
        {
            HorizontalInputHelper(axis, arialManeuverability, true);
        }
    }

    protected void HorizontalInputHelper(string axis, float maneuverability, bool inAir)
    {
        float inputAccel = Input.GetAxis(axis) * maneuverability * acceleration;
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
            SpawnJumpParticles();
            audioPlayer.clip = jumpSound;
            audioPlayer.Play();
            if (grounded)
            {
                grounded = false;
                rb.velocity = new Vector2(velocity.x, 0);
                rb.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            }
            else if (rb.velocity.y < jumpSpeed)
            {
                canAirJump = false;
                rb.velocity = new Vector2(velocity.x, 0);
                rb.AddForce(new Vector2(0, airJumpSpeed), ForceMode2D.Impulse);
            }
            else
            {
                canAirJump = false;
                //the jump punish lol
            }
        }
        else
        {
            rb.velocity = new Vector2(velocity.x, rb.velocity.y);
        }
        jumped = false;
        grounded = false;
        transform.localScale = new Vector3(originalScale.x * movementDirection, originalScale.y, originalScale.z);
    }

    private void SpawnJumpParticles()
    {
        Vector3 jpPos = transform.position;
        jpPos.y -= rend.bounds.extents.y;
        //the jump particles handle its own animation and auto destruction
        Instantiate(jumpParticles, transform.position, Quaternion.identity);
    }


    protected void ThrowItem()
    {
        if (throwStuff && haveItem)
        {
            haveItem = false;
            Debug.Log("threw item");
            Vector2 throwTemp = throwStrength;
            throwTemp.x *= movementDirection;
            //check if velocity and movement direction are both positive/both negative
            //no need to add if velocity is 0 ofc
            if (velocity.x * movementDirection > 0)
            {
                throwTemp.x += velocity.x * 0.3f;
            }
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

    public float GetJumpStrength()
    {
        return jumpSpeed;
    }

    public float GetMaxVelocity()
    {
        return maxSpeed;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void GivePoints(float p)
    {
        points += Mathf.Abs(p);
    }

    public float GetCoinsCollected()
    {
        return points + maxHeightReached;
    }

    public void BouncePlayer(float bounceStrength)
    {
        Debug.Log("Bounce called");
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(new Vector2(0, bounceStrength * jumpSpeed), ForceMode2D.Impulse);
    }

    #endregion

    #region collision
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            JumperGeneralPlatform check = collision.gameObject.GetComponent<JumperGeneralPlatform>();
            if (check != null) 
            {
                if (collision.gameObject.transform.position.y + collision.gameObject.GetComponent<Renderer>().bounds.extents.y <
                transform.position.y + rend.bounds.extents.y)
                {
                    check.Remove();
                }
            }
        }
        else if (collision.gameObject.CompareTag(enemyTag) || collision.gameObject.CompareTag(obstacleTag))
        {
            TakesDamage(collision.gameObject);
        }
    }

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(collectiblesTag))
        {
            JumperGeneralPickup pu = collision.gameObject.GetComponent<JumperGeneralPickup>();
            if (pu.IsAutomaticPickup())
            {
                //blah blah do stuff for picking up the item
                Debug.Log("picked up " + pu.name);
            }
            else if (pickup && !haveItem)
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
        else if (collision.gameObject.CompareTag(obstacleTag) || collision.gameObject.CompareTag(enemyTag))
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
            JumperGeneralThreat obsControl = collision.GetComponent<JumperGeneralThreat>();
            if (obsControl != null)
            {
                DamageHelper(obsControl.GetDamage());
            } 
        }
    }
    

    protected void DamageHelper(float damage)
    {
        currentHealth -= Mathf.Abs(damage);
        StartCoroutine(Flasher());
        if (currentHealth < 0)
        {
            JumperManagerUI.Instance.End(false);
        }
        //player destruction handled elsewhere
    }
    
    //yikes, permanent disability right here
    protected void DisablePlayerMovement()
    {
        horizontalAxis = dummyAxis;
        jumpAxis = dummyAxis;
        pickupAxis = dummyAxis;
        throwAxis = dummyAxis;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
    }

    public void RunDeathSequence()
    {
        //okay this function removes all input ability from player
        //disables gravity
        //stops them from moving
        //and uhhh kills em
        DisablePlayerMovement();
        DeathAnimation();
    }

    //TODO get a death animation
    private void DeathAnimation()
    {
        return;
    }

    public void RunVictorySequence()
    {
        //this function also removes ability to input 
        //but a victory animation instead
        DisablePlayerMovement();
        DanceAnimation();
    }

    //TODO get a dance animation
    private void DanceAnimation()
    {
        return;
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
