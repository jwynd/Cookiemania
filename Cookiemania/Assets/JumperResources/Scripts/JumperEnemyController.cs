using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemyController : JumperGeneralThreat
{
    #region variables
    
    [SerializeField]
    protected Vector2 jump = new Vector2(2f, 4f);

    [SerializeField]
    protected float jumpDelay = 0.75f;

    [SerializeField]
    protected LayerMask groundLayer;

    [SerializeField]
    protected float buffer = 1f;

    

    protected float parentLeft;
    protected float parentRight;
    protected float direction = 1;
    protected float jumpDirection = 1;
    protected float originalMaxVelocity;
    protected Rigidbody2D rb;
    protected bool jumpToMyDeath = false;
    protected bool hasJumped = false;
    protected Vector3 originalScale;
    protected Animator anim;
    
    #endregion

    #region startup
    protected override void Awake()
    {
        base.Awake();
        originalScale = transform.localScale;
        originalMaxVelocity = maxVelocity;
    }

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        JumperGeneralPlatform dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        dad.enemyChild = this;
        float diff = jm.GetDifficultyMultiplier();
        damage *= diff;
        maxHealth *= diff;
        currentHealth = maxHealth;
        acceleration *= diff;
        maxVelocity *= diff;
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
        transform.parent = null;
        transform.position = new Vector2(Random.Range(parentLeft, parentRight), transform.position.y);
        jumpDirection = direction;
    }
    #endregion

    #region fixedUpdate

    // Update is called once per frame
    protected void FixedUpdate()
    {
        UpdateHelper();              
    }

    protected void UpdateHelper()
    {
        if (!jumpToMyDeath)
        {
            CheckBounds();
            Walk();
        }
        else if (jm.player == null)
        {
            Destroy(gameObject);
        }
        else if (!hasJumped)
        {
            float playerx = RunToPlayer();
           // CheckBoundsTrackingPlayer();
            Walk();
            JumpToPlayer(playerx);
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

    protected void Jump()
    {
        //disabled when direction == 0
        if ((direction < 0 && rb.position.x < parentLeft) || (direction > 0 && rb.position.x > parentRight))
        {
            JumpHelper();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jump.y), ForceMode2D.Impulse);
        }
    }

    protected void JumpToPlayer(float playerx)
    {
        if (Mathf.Abs(playerx - rb.position.x) < 2.5f || (direction < 0 && rb.position.x < parentLeft) || (direction > 0 && rb.position.x > parentRight))
        {
            JumpHelper();
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(maxVelocity * direction, jump.y), ForceMode2D.Impulse);
            hasJumped = true;
          //  StartCoroutine(RejumpDelay());
            direction = 0;
        }
    }

    private void JumpHelper()
    {
        rb.isKinematic = false;
        rb.gravityScale = 1;
        if (anim != null) { anim.SetTrigger("Jump"); }
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
        jumpDirection = direction;

    }

    protected void Walk()
    { 
        //direction set to 0 disables this
        rb.velocity = new Vector2(Mathf.Clamp(direction * acceleration + rb.velocity.x, -maxVelocity, maxVelocity), rb.velocity.y);
    }

    protected float RunToPlayer()
    {
        float playerX = jm.player.transform.position.x;
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
        jumpDirection = direction;

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
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.5f, 0.1f));
    }

    public override void PlatformDestroyed(float timer, float flashPeriod)
    {
        StartCoroutine(PlatformDestroyedHelper(timer));
    }
    #endregion

    #region coroutine
    protected IEnumerator PlatformDestroyedHelper(float timer)
    {
        yield return new WaitForSeconds(timer / 5);
        jumpToMyDeath = true;
        yield return new WaitForSeconds(timer * 4 / 5);
        Remove();
    }

    protected IEnumerator RejumpDelay()
    {
        yield return new WaitForSeconds(jumpDelay);
        Renderer rend = GetComponent<Renderer>();
        float offset = rend.bounds.extents.x;
        float yoffset = rend.bounds.extents.y * 1.02f;
        hasJumped = !Physics2D.OverlapArea(new Vector2(transform.position.x - offset, transform.position.y - yoffset),
            new Vector2(transform.position.x + offset, transform.position.y - yoffset + 0.01f), groundLayer);
    }
    #endregion
}
