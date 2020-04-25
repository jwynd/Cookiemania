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
[RequireComponent(typeof(JumperInputComponent))]
public class JumperPlayerController : MonoBehaviour
{
    #region variables
    public float acceleration = 1f;
    public float maxSpeed = 5.5f;
    public float jumpSpeed = 7.5f;
    public float airJumpSpeed = 5.5f;
    public LayerMask groundLayer;
    public Vector2 throwStrength = new Vector2(4.0f, 4.0f);
    public float maxHealth = 5;
    public float flashTime = 0.15f;
    public float damageTimerMax = 1.5f;
    public AudioClip jumpSound;
    public JumperSelfDestruct jumpParticles;
    [SerializeField]
    [Tooltip("Axis for left and right movements")]
    protected string horizontalAxis = "Horizontal";
    [SerializeField]
    [Tooltip("Axis for jumping")]
    protected string jumpAxis = "Jump";
    
    
    [SerializeField]
    [Tooltip("Axis for picking up items")]
    protected string pickupAxis = "Pickup";
    [SerializeField]
    [Tooltip("Axis for throwing items")]
    protected string throwAxis = "Throw";
    [SerializeField]
    [Tooltip("EMPTY INPUT axis, should still exist but no keys mapped to it")]
    protected string dummyAxis = "Dummy";

    protected JumperInputComponent input = null;

    //allowing other objects to listen to player's inputs
    public JumperInputComponent Input
    {
        get
        {
            return input;
        }
    }

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
    
    protected float aerialManeuverability = 0.5f;
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
    protected float horizontalInput = 0f;
    protected float originalJumpSpeed;
    #endregion

    #region startup
    protected void Awake()
    {
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody2D>();
        audioPlayer = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        RebindInputController();
        rb.velocity = Vector2.zero;
        currentHealth = maxHealth;
        originalJumpSpeed = jumpSpeed;
    }
    protected void Start()
    {
        jm = JumperManagerGame.Instance;
        rb.gravityScale = jm.GetAlteredGravity();
        jumpSpeed *= rb.gravityScale;
        airJumpSpeed *= rb.gravityScale;
        GetTags();
    }

    protected void GetTags()
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
        //need to be in update
        GetInputs();
    }

    protected void GetInputs()
    {
        if (Time.timeScale == 0)
        {
            jumped = false;
            return;
        }
        jumped = JumpInput(input.Jump);
        pickup = input.Pickup > 0f;
        throwStuff = input.Throw > 0f ? true : throwStuff;
        horizontalInput = input.Horizontal;
    }

    protected void ResetInputForCollisions()
    {
        pickup = false;
    }

    protected void ResetInputForFixedUpdate()
    {
        jumped = false;
        throwStuff = false;
        horizontalInput = 0f;
    }

    //match up disables to the enables in enable player movement
    protected void DisablePlayerInput(bool stopMovement = true)
    {
        rb.gravityScale = stopMovement ? 0 : rb.gravityScale;
        rb.velocity = stopMovement ? Vector2.zero : rb.velocity;
        rb.isKinematic = true;
        ResetInputForFixedUpdate();
        ResetInputForCollisions();
        enabled = false;
    }

    public void EnablePlayerInput()
    {
        rb.gravityScale = jm.GetAlteredGravity();
        rb.isKinematic = false;
        enabled = true;
    }

    bool JumpInput(float inputVal)
    {
        if (inputVal > 0f)
        {
            if (grounded || canAirJump)
            {
                return true;
            }
        }
        return jumped;
    }

    #endregion

    #region fixedUpdate

    protected void FixedUpdate()
    {
        UpdateHeight();
        UpdateJumpCapability();
        Movement();
        ThrowItem();
        //currently, no timers on character
        Timers();
        ResetInputForFixedUpdate();
    }

    protected void UpdateJumpCapability()
    {
        grounded = IsGrounded();
        canAirJump = grounded ? true : canAirJump;
    }

    void UpdateHeight()
    {
        maxHeightReached = transform.position.y > maxHeightReached ? transform.position.y : maxHeightReached;
    }

    //this function requires the collider be the same size as the renderer or smaller
    protected bool IsGrounded()
    {
        float offset = rend.bounds.extents.x;
        float yoffset = rend.bounds.extents.y * 1.02f;
        return Physics2D.OverlapArea(new Vector2(transform.position.x - offset, transform.position.y - yoffset), 
            new Vector2(transform.position.x + offset, transform.position.y - yoffset + 0.01f), groundLayer);
    }

    protected float HorizontalMovement()
    {
        float tempManeuver = !grounded || jumped ? aerialManeuverability : normalManeuverability;
        return HorizontalMovementHelper(rb.velocity.x, horizontalInput, tempManeuver, !grounded || jumped);
    }

    protected float HorizontalMovementHelper(float currentV, float direction, float maneuverability, bool inAir)
    {
        float inputAccel = direction * maneuverability * acceleration;
        if (inputAccel > 0)
        {
            currentV = Mathf.Min(inputAccel + currentV, maxSpeed);
            movementDirection = 1;
        }
        else if (inputAccel < 0)
        {
            currentV = Mathf.Max(inputAccel + currentV, -maxSpeed);
            movementDirection = -1;
        }
        //lower their velocity if they're on the ground and not going forward or back
        else if (inputAccel == 0 && !inAir)
        {
            currentV = ApplyFriction(currentV, maneuverability);
        }
        transform.localScale = new Vector3(originalScale.x * movementDirection, originalScale.y, originalScale.z);
        return currentV;
    }

    protected float ApplyFriction(float currentV, float maneuverability)
    {
        float sign = Mathf.Abs(currentV);
        if (Mathf.Abs(currentV) < acceleration)
        {
            currentV = 0;
        }
        else
        {
            //only gets to here if abs(velocity.x) is greater than a non-negative value
            //cant div by zero
            sign /= currentV;
            currentV += (-sign * maneuverability * acceleration);
        }
        return currentV;
    }

    protected void Movement()
    {
        //changing the direction of the throw to the last direction moved
        float horizontal = HorizontalMovement();
        if (jumped) { Jump(horizontal); }
        else { rb.velocity = new Vector2(horizontal, rb.velocity.y); }        
    }

    protected void Jump(float horizontal)
    {
        SpawnJumpParticles();
        audioPlayer.clip = jumpSound;
        audioPlayer.Play();
        if (grounded)
        {
            JumpHelper(horizontal, jumpSpeed);
        }
        //air jump only procs when our jump speed has slowed down enough
        else if (rb.velocity.y < jumpSpeed)
        {
            //this is a physics bool, needs to be reset by physics functions
            JumpHelper(horizontal, airJumpSpeed);
            canAirJump = false;
        }
    }

    protected void JumpHelper(float horizontalSpeed, float verticalSpeed)
    {
        rb.velocity = new Vector2(horizontalSpeed, 0);
        rb.AddForce(new Vector2(0, verticalSpeed), ForceMode2D.Impulse);
    }

    protected void SpawnJumpParticles()
    {
        //spawn at my feet :)
        Vector3 jpPos = transform.position;
        jpPos.y -= rend.bounds.extents.y;
        //the jump particles handle its own animation and auto destruction
        Instantiate(jumpParticles, transform.position, Quaternion.identity);
    }

    //run after movement
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
            if (rb.velocity.x * movementDirection > 0)
            {
                throwTemp.x += rb.velocity.x * 0.3f;
            }
            heldItemRB.gameObject.GetComponent<JumperPickupController>().Thrown(throwTemp);
        }
        throwStuff = false;
    }

    

    //to be run in fixed update
    protected void Timers()
    {
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

    public float GetOriginalJumpStrength()
    {
        return originalJumpSpeed;
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

    public string GetHorizontalAxis()
    {
        return horizontalAxis;
    }
    public string GetJumpAxis()
    {
        return jumpAxis;
    }
    public string GetPickupAxis()
    {
        return pickupAxis;
    }
    public string GetThrowAxis()
    {
        return throwAxis;
    }
    public string GetDummyAxis()
    {
        return dummyAxis;
    }

    public bool HasThrowable()
    {
        return haveItem;
    }

    public void RebindInputController()
    {
        input = GetComponent<JumperInputComponent>();
        if (input == null)
        {
            input = gameObject.AddComponent<JumperInputKeyboard>();
        }
    }

    public void BouncePlayer(float bounceStrength)
    {
        Debug.Log("Bounce called");
        JumpHelper(rb.velocity.x, bounceStrength * jumpSpeed);
        canAirJump = true;
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
            obsControl.RemoveOnDamage();

        }
    }
    

    protected void DamageHelper(float damage)
    {
        //when damage is too high, gets reduced to 90% of players max hp
        float sanitizedDamage = Mathf.Min(Mathf.Abs(damage), maxHealth);
        currentHealth -= sanitizedDamage;
        StartCoroutine(Flasher());
        if (currentHealth <= 0)
        {
            JumperManagerUI.Instance.End(false);
        }
        //player destruction handled elsewhere
    }
    
    

    public void RunDeathSequence()
    {
        //okay this function removes all input ability from player
        //disables gravity
        //stops them from moving
        //and uhhh kills em
        DisablePlayerInput();
        DeathAnimation();
    }

    //TODO get a death animation
    protected void DeathAnimation()
    {
        return;
    }

    public void RunVictorySequence()
    {
        //this function also removes ability to input 
        //but a victory animation instead
        DisablePlayerInput();
        DanceAnimation();
    }

    //TODO get a dance animation
    protected void DanceAnimation()
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
