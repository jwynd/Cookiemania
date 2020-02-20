using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperThrowingEnemy : JumperGeneralThreat
{
    #region variables
    [SerializeField]
    protected float buffer = 1f;

    [SerializeField]
    protected JumperEnemyProjectile projectile = null;

    [SerializeField]
    protected float verticalThrowStrength = 6.0f;

    [SerializeField]
    protected float horizontalThrowStrengthMax = 2.0f;

    [Tooltip("This should be a child gameobject in the ready position when the character is facing right, will swap sides" +
        " when character is facing left")]
    [SerializeField]
    protected Transform projectileReadyPosition = null;

    [SerializeField]
    protected float engageDistance = 6f;

    [SerializeField]
    protected float fireDelay = 1.0f;

    [SerializeField]
    protected float rearmDelay = 1.0f;

    protected float parentLeft;
    protected float parentRight;
    protected float direction = 1;
    protected float originalMaxVelocity;
    protected bool projectileReadied = false;
    protected bool projectileCoroutineCalled = true;
    protected JumperEnemyProjectile instantiatedProjectile = null;
    protected Vector2 throwStrength = Vector2.zero;
    protected Vector2 currentPlayerPos = Vector2.zero;
    protected float throwDirection = 0.0f;
    protected Rigidbody2D rb;
    protected Vector3 originalScale;
    protected Animator anim;
    //needs to be updated when used
    protected Transform playerTransform;
    #endregion

    #region startup
    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
        originalMaxVelocity = maxVelocity;
        throwStrength.y = verticalThrowStrength;
    }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerTransform = jm.player.transform;
        JumperGeneralPlatform dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        dad.enemyChild = this;
        float diff = jm.GetDifficultyMultiplier();
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        rb.gravityScale = 0;
        Vector3 pBounds = dad.GetHorizontalBounds();
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        if (transform.parent.position.x < transform.position.x)
        {
            direction = -1;
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
        throwDirection = direction;
        transform.parent = null;
        transform.position = new Vector2(Random.Range(parentLeft, parentRight), transform.position.y);
        StartCoroutine(ReadyProjectile());
    }
    #endregion

    #region fixedUpdate
    protected void FixedUpdate()
    {
        UpdateHelper();
    }

    protected void UpdateHelper()
    {
        if (playerTransform == null)
        {
            Destroy(gameObject);
        }
        else
        {
            //this actually has to go in an else lol
            currentPlayerPos = playerTransform.position;
        }
        if (!projectileCoroutineCalled)
        {
            StartCoroutine(ReadyProjectile());
        }
        if (PlayerNearby())
        {
            RunToPlayer();
            CheckBoundsTrackingPlayer();
            Walk();
            if (projectileReadied)
            {
                ThrowProjectile();
            }
        }
        else
        {
            CheckBounds();
            Walk();
        }
        
        
    }

    protected void ThrowProjectile()
    {
        
        instantiatedProjectile.transform.parent = null;
        throwStrength.x = Mathf.Min(Mathf.Abs(currentPlayerPos.x - transform.position.x), horizontalThrowStrengthMax) * throwDirection;
        instantiatedProjectile.ThrowProjectile(throwStrength);
        instantiatedProjectile = null;
        projectileReadied = false;
        StartCoroutine(RearmCoroutine());        
    }

    protected bool PlayerNearby()
    {
        return Vector2.Distance(transform.position, currentPlayerPos) < engageDistance;   
    }

    protected void CheckBounds()
    {
        if (direction < 0 && rb.position.x < parentLeft)
        {
            rb.velocity = Vector2.zero;
            direction = 1;
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        }
        else if (direction > 0 && rb.position.x > parentRight)
        {
            rb.velocity = Vector2.zero;
            direction = -1;
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
        }
    }

    protected void CheckBoundsTrackingPlayer()
    {
        if (direction <= 0 && rb.position.x < parentLeft)
        {
            direction = 0;
            rb.velocity = Vector2.zero;
        }
        else if (direction >= 0 && rb.position.x > parentRight)
        {
            direction = 0;
            rb.velocity = Vector2.zero;
        }
    }

    protected void Walk()
    { 
        //direction set to 0 disables this
        rb.velocity = new Vector2(Mathf.Clamp(direction * acceleration + rb.velocity.x, -maxVelocity, maxVelocity), rb.velocity.y);
    }

    protected float RunToPlayer()
    {
        float playerX = currentPlayerPos.x;
        //sprint speed hehe
        maxVelocity = originalMaxVelocity * 1.5f;

            if (rb.position.x > playerX + buffer)
            {
                direction = -1;
                transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);
            }
            else if (rb.position.x + buffer < playerX)
            {
                direction = 1;
                transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
            }
        throwDirection = direction;

        return playerX;
    }

    #endregion

    #region public
    public override void Remove(bool isImmediate = false)
    {
        rb.isKinematic = true;
        if (isImmediate)
        {
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.05f, 0.05f));
        }
        //dont need to worry about instantiated projectile, should auto destroy when parent dies
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.5f, 0.1f));
    }

    public override void PlatformDestroyed(float timer, float flashPeriod)
    {
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, timer, flashPeriod));
    }
    #endregion

    #region coroutine
    protected IEnumerator ReadyProjectile()
    {
        projectileCoroutineCalled = true;
        EquipProjectile();
        yield return new WaitForSeconds(fireDelay);
        projectileReadied = true;
    }

    protected IEnumerator RearmCoroutine()
    {
        yield return new WaitForSeconds(rearmDelay);
        projectileCoroutineCalled = false;
    }
    protected void EquipProjectile()
    {
        instantiatedProjectile = Instantiate(projectile);
        instantiatedProjectile.transform.parent = transform;
        Vector3 newLocalPos = projectileReadyPosition.position;
        instantiatedProjectile.transform.position = newLocalPos;
        newLocalPos = instantiatedProjectile.transform.localScale;
        instantiatedProjectile.transform.localScale = new Vector3(newLocalPos.x * throwDirection, newLocalPos.y, newLocalPos.z);
    }
    #endregion
}
