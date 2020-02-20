using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemyController : JumperGeneralThreat
{
    #region variables
    
    [SerializeField]
    private Vector2 jump = new Vector2(2f, 4f);

    [SerializeField]
    private float buffer = 1f;

    private float parentLeft;
    private float parentRight;
    private float direction = 1;
    private float originalMaxVelocity;
    private Rigidbody2D rb;
    private bool jumpToMyDeath = false;
    private Vector3 originalScale;
    private Animator anim;
    
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

    }
    #endregion

    #region fixedUpdate

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateHelper();              
    }

    private void UpdateHelper()
    {
        if (!jumpToMyDeath)
        {
            CheckBounds();
            Walk();
        }
        else
        {
            float playerx = RunToPlayer();
            /*
             * default is walk then jump
             */
            Walk();
            JumpToPlayer(playerx);
            //Jump();
        }
    }

    private void Jump()
    {
        //disabled when direction == 0
        if ((direction < 0 && rb.position.x < parentLeft) || (direction > 0 && rb.position.x > parentRight))
        {
            rb.isKinematic = false;
            rb.gravityScale = 1;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jump.y), ForceMode2D.Impulse);
            direction = 0;
            if (anim != null) { anim.SetTrigger("Jump"); }
        }
    }

    private void JumpToPlayer(float playerx)
    {
        if (Mathf.Abs(playerx - rb.position.x) < 2f && direction != 0)
        { 
            rb.isKinematic = false;
            GetComponent<Collider2D>().isTrigger = false;
            rb.gravityScale = 1;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, jump.y), ForceMode2D.Impulse);
            direction = 0;
            if (anim != null) { anim.SetTrigger("Jump"); }
        }
    }

    private void CheckBounds()
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

    private void Walk()
    { 
        //direction set to 0 disables this
        rb.velocity = new Vector2(Mathf.Clamp(direction * acceleration + rb.velocity.x, -maxVelocity, maxVelocity), rb.velocity.y);
    }

    private float RunToPlayer()
    {
        float playerX = jm.player.transform.position.x;
        //sprint speed hehe
        maxVelocity = originalMaxVelocity * 1.5f;
        if (direction != 0)
        {
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
        }
        return playerX;
    }

    #endregion

    #region public
    public override void Remove()
    {
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.5f, 0.1f));
    }

    public override void PlatformDestroyed(float timer, float flashPeriod)
    {
        StartCoroutine(PlatformDestroyedHelper(timer));
    }
    #endregion

    #region coroutine
    private IEnumerator PlatformDestroyedHelper(float timer)
    {
        yield return new WaitForSeconds(timer / 4);
        jumpToMyDeath = true;
        yield return new WaitForSeconds(timer * 3 / 4);
        Remove();
    }
    #endregion
}
