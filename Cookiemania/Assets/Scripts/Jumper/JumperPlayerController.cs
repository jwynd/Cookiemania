using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPlayerController : MonoBehaviour
{
    #region variables
    private const float MAGNET_BASE_SIZE = 1.8f;
    private const float MAGNET_BASE_SMALL_SIZE = 0.2f;
    private const float MAGNET_BASE_EXTEND_TIME = 0.18f;
    private const float MAGNET_FLAT_CD_REDUCTION = 1f;
    private const float MAGNET_FLAT_DURATION_INCREASE = .15f;
    private const float MAGNET_FLAT_RANGE_INCREASE = 0.2f;
    private const float HEALTH_PERCENT_DR_INCREASE = .1f;
    private const float HEALTH_PERCENT_INCREASE = .1f;
    private const float HEALTH_PERCENT_DR_CAP = .8f;
    private const float SHIELD_FLAT_CD_REDUCTION = 2f;

    public float acceleration = 1f;
    public float maxSpeed = 5.5f;
    public float jumpSpeed = 7.5f;
    public float airJumpSpeed = 5.5f;
    public LayerMask groundLayer;
    public Vector2 throwStrength = new Vector2(4.0f, 4.0f);
    public int maxHealth = 5;
    public float flashTime = 0.15f;
    public float damageTimerMax = 1.5f;

    public float magnetCooldown = 10f;
    public float magnetDuration = 0.5f;
    // levelable character properties

    public AudioClip jumpSound;
    public JumperSelfDestruct jumpParticles;
    public GameObject aiPrefab;
    public Transform aiAttach;
    public GameObject magnet;
    public GameObject magnetAnimator;
    public GameObject shield;
    public TextParticleSystem texts;

    public AbilityCooldown magnetcdSymbol;
    public AbilityCooldown shieldcdSymbol;

    protected Rigidbody2D rb;
    protected Renderer rend;
    protected Vector2 velocity = Vector2.zero;
    protected bool grounded = true;
    protected float damageTimer = 0;
    protected int magnetLevel = 0;
    protected int shieldLevel = 0;
    protected int jumpLevel = 0;
    protected float damageReduction = 0.0f;
    protected JumperMagnet magnetController;
    protected int currentHealth;

    protected bool jumpInput = false;
    protected bool shieldInput = false;

    protected float aerialManeuverability = 0.5f;
    protected float normalManeuverability = 1;

    protected float currentMagnetCD = 0f;
    protected float currentMagnet = 0f;
    protected bool canMagnet = true;
    protected bool isMagnetic = false;

    protected float currentShieldCD = 0f;
    protected float currentShield = 0f;
    protected float shieldCooldown = 10f;
    protected float shieldDuration = 1.5f;
    protected bool canShield = true;
    protected bool isShielded = false;
    protected bool hasAI = false;
    protected JumperAI aiRef = null;
    protected Rigidbody2D heldItemRB = null;
    protected SpriteRenderer heldItemSprite = null;
    protected AudioSource audioPlayer = null;
    //protected float maxHeightReached = 0f;
    protected float points = 0f;
    protected JumperManagerGame jm;
    protected float movementDirection = 1;
    protected Vector3 originalScale;
    protected Color damagedColor = Color.red * Color.white;
    protected Color defaultColor = Color.white;
    protected bool magnetInput;
    protected bool canAirJump = true;
    protected int coinJump = 0;
    protected string groundTag;
    protected string enemyTag;
    protected string obstacleTag;
    protected string collectiblesTag;
    protected string myTag;
    protected float horizontalInput = 0f;
    protected float originalJumpSpeed;
    protected CapsuleCollider2D coll;
    protected PolygonCollider2D shieldColl;
    protected SpriteExpander magnetEffect;
    private float magnetRange;

    #endregion

    #region startup
    protected void Awake()
    {
        SetRequiredComponentRefs();
        SetAwakeAvailableInits();
    }

    private void SetAwakeAvailableInits()
    {
        originalScale = transform.localScale;
        rb.velocity = Vector2.zero;
        currentHealth = maxHealth;
        originalJumpSpeed = jumpSpeed;
        shieldColl.enabled = false;
        coll.enabled = true;
    }

    private void SetRequiredComponentRefs()
    {
        rb = GetComponent<Rigidbody2D>();
        audioPlayer = GetComponent<AudioSource>();
        rend = GetComponent<Renderer>();
        coll = GetComponent<CapsuleCollider2D>();
        shieldColl = GetComponent<PolygonCollider2D>();
    }

    protected void Start()
    {
        jm = JumperManagerGame.Instance;
        magnetAnimator.SetActive(false);
        magnetController = magnet.GetComponent<JumperMagnet>();
        magnetEffect = magnet.GetComponentInChildren<SpriteExpander>();
        shield.SetActive(false);
        SetFromManagerStoredInfo();
    }

    private void SetFromManagerStoredInfo()
    {
        rb.gravityScale = jm.GetAlteredGravity();
        GetTags();
        magnetLevel = jm.MagnetAvailable;
        shieldLevel = jm.Shield;
        jumpLevel = jm.CoinJump;
        jumpSpeed *= rb.gravityScale;
        airJumpSpeed *= rb.gravityScale;
        // each level of health enhances damage reduction and small amount
        // of flat hp
        // 10% more hp per level
        maxHealth += (int)(jm.Health * HEALTH_PERCENT_INCREASE * maxHealth);
        currentHealth = maxHealth;
        damageReduction = Mathf.Min(HEALTH_PERCENT_DR_CAP,
            HEALTH_PERCENT_DR_INCREASE * jm.Health);
        if (jm.AI > 0)
        {
            var instance = Instantiate(aiPrefab, transform);
            instance.transform.parent = null;
            instance.transform.localScale = Vector3.one;
            instance.transform.position = aiAttach.position;
            aiRef = instance.GetComponent<JumperAI>();
            aiRef.SetFollowPoint(aiAttach, transform);
        }
        magnetCooldown -= jm.MagnetCD * MAGNET_FLAT_CD_REDUCTION;
        magnetDuration += (jm.MagnetAvailable - 1) * MAGNET_FLAT_DURATION_INCREASE;
        magnetRange = jm.MagnetRange * MAGNET_FLAT_RANGE_INCREASE;
        shieldCooldown -= (jm.Shield - 1) * SHIELD_FLAT_CD_REDUCTION;

        if (magnetLevel < 1)
        {
            magnetcdSymbol.gameObject.SetActive(false);
        }
        if (shieldLevel < 1)
        {
            shieldcdSymbol.gameObject.SetActive(false);
        }
        
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
            jumpInput = false;
            return;
        }
        jumpInput = JumpInput(InputAxes.Instance.Jump.triggered);
        shieldInput = InputAxes.Instance.Action1.triggered || shieldInput;
        magnetInput = InputAxes.Instance.Action2.triggered || magnetInput;
        horizontalInput = InputAxes.Instance.Horizontal.ReadValue<float>();
    }

    protected void ResetInputForCollisions()
    {
        shieldInput = false;
    }

    protected void ResetInputForFixedUpdate()
    {
        jumpInput = false;
        magnetInput = false;
        shieldInput = false;
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

    bool JumpInput(bool jump)
    {
        return (jump && (grounded || canAirJump || coinJump > 0)) || jumpInput;
    }

    #endregion

    #region fixedUpdate

    protected void FixedUpdate()
    {
        // UpdateHeight();
        UpdateJumpCapability();
        Movement();
        UseMagnet();
        ActivateShield();
        Timers();
        ResetInputForFixedUpdate();
    }

    protected void UpdateJumpCapability()
    {
        grounded = IsGrounded();
        canAirJump = grounded ? true : canAirJump;
        // lose the coin jump when you get grounded
        coinJump = grounded ? 0 : coinJump;
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
        float tempManeuver = !grounded || jumpInput ? aerialManeuverability : normalManeuverability;
        return HorizontalMovementHelper(rb.velocity.x, horizontalInput, tempManeuver, !grounded || jumpInput);
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
        if (jumpInput) { Jump(horizontal); }
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
            if (coinJump > 0 && !canAirJump)
                coinJump -= 1;
            else
            {
                canAirJump = false;
            }
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
    protected void UseMagnet()
    {
        if (magnetInput && canMagnet && magnetLevel > 0)
        {
            // activate magnet
            magnetcdSymbol.StartAppear(() => { currentMagnetCD = 0f; canMagnet = true; }, magnetCooldown);
            currentMagnet = magnetDuration;
            isMagnetic = true;
            magnetController.ActivateMagnet(magnetDuration);
            magnetAnimator.SetActive(true);
            magnetEffect.RequestResize(
                MAGNET_BASE_EXTEND_TIME, MAGNET_BASE_SMALL_SIZE, MAGNET_BASE_SIZE + magnetRange);
            canMagnet = false;
        }
        if (!isMagnetic)
        {
            magnetAnimator.SetActive(false);
            magnet.SetActive(false);
        }
        magnetInput = false;
    }

    protected void ActivateShield()
    {
        if (shieldInput && canShield && shieldLevel > 0)
        {
            Debug.LogWarning("shield up!");
            shieldcdSymbol.StartAppear(() => { currentShieldCD = 0f; canShield = true; }, shieldCooldown);
            currentShield = shieldDuration;
            isShielded = true;
            shield.SetActive(true);
            shieldColl.enabled = true;
            canShield = false;
        }
        if (shield.activeSelf && !isShielded)
        {
            shield.SetActive(false);
            shieldColl.enabled = false;
        }
        shieldInput = false;
    }



    //to be run in fixed update
    protected void Timers()
    {
        // decrement timers
        // they're floats: i would need to run the game for years to underflow
        // and precision stops mattering once im under 0 :)
        currentShield -= Time.fixedDeltaTime;
        currentMagnet -= Time.fixedDeltaTime;
        // set timer flags
        isShielded = currentShield > 0f;
        isMagnetic = currentMagnet > 0f;
    }

    #endregion

    #region public
    public void PickupDestroyed()
    {
        /*heldItemRB = null;
        haveItem = false;*/
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

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void GivePoints(float p)
    {
        var toAdd = Mathf.Abs(p);
        points += toAdd;
        if (toAdd >= 1f)
            texts.EmitEasy(2f, ((int)toAdd).ToString(), Color.yellow);
    }

    public float GetCoinsCollected()
    {
        return points;
    }

    public void BouncePlayer(float bounceStrength)
    {
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
            if (!pu) return;
            if (pu.IsAutomaticPickup())
            {
                //blah blah do stuff for picking up the item
                GivePoints(pu.DepletePickupPoints());
                coinJump = jumpLevel;
                if (pu.IsDestroyOnPickup())
                    pu.Remove();
            }
            /*else if (pickup && !haveItem)
            {
                //gonna parent the held item to the player then put it in the held item position
                //modified by the size of the object in future
                Transform blockTransform = collision.transform;
                blockTransform.transform.parent = transform;
                blockTransform.transform.position = transform.Find("HeldItem").transform.position;
                heldItemRB = collision.gameObject.GetComponent<Rigidbody2D>();
                pickup = false;
                haveItem = true;
            } */
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
        JumperGeneralThreat obsControl = collision.GetComponent<JumperGeneralThreat>();
        if (obsControl == null) return;
        if (damageTimer <= 0 && !isShielded)
        {
            DamageHelper(obsControl.GetDamage());
            obsControl.RemoveOnDamage();
        }
        else if (isShielded)
        {
            texts.EmitEasy(1f, "Blocked", Color.cyan);
            GivePoints(obsControl.GetPointValue());
            obsControl.Remove(true);
        }
    }


    protected void DamageHelper(float damage)
    {
        //when damage is too high, gets reduced to 100% - healthLevel * 10%
        // e.g. healthlevel = 1, max damage taken is 90%, for 2 max damage is 80% of maxhealth
        int sanitizedDamage = (int)Mathf.Min(Mathf.Abs(damage), maxHealth * (1f - damageReduction));
        currentHealth -= sanitizedDamage;
        float intensity = (sanitizedDamage / (float)maxHealth) * 4f;
        texts.EmitEasy(intensity, sanitizedDamage.ToString(), Color.red);
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
        aiRef?.Die();
        return;
    }

    public void RunVictorySequence()
    {
        //this function also removes ability to input 
        //but a victory animation instead
        DisablePlayerInput();
        DanceAnimation();
    }

    public void OnDestroy()
    {
        if (aiRef)
            Destroy(aiRef.gameObject);
    }

    public void OnDisable()
    {
        if (aiRef)
            aiRef.enabled = false;
    }

    public void OnEnable()
    {
        if (aiRef)
            aiRef.enabled = true;
    }

    //TODO get a dance animation
    protected void DanceAnimation()
    {
        aiRef?.Dance();
        return;
    }

    //credit: https://answers.unity.com/questions/838194/make-your-player-flash-when-hit.html
    IEnumerator Flasher()
    {
        damageTimer = damageTimerMax;
        int interval = (int)(damageTimerMax / flashTime);
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
