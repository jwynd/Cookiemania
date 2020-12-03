using System;
using System.Collections;
using UnityEngine;

public class JumperObstacleController : JumperGeneralThreat
{
    #region variables
    
    [SerializeField]
    private float secondsBetweenFlashes = 1.2f;

    private Rigidbody2D rb;
    protected JumperGeneralPlatform dad;
    protected JumperGeneralPlatform uncle;
    protected JumperGeneralPlatform currentlyOn;
    protected Animator animator;
    protected Collider2D coll2D;
    protected float flashCountdown = 0f;
    private bool flashing = false;
    private float heightOffPlatform = 1.5f;
    #endregion

    #region startup
    protected override void Start()
    {
        base.Start();
        coll2D = GetComponent<Collider2D>();
        SetBaseParameters();
        animator = GetComponent<Animator>();
        dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        dad.enemyChild = null;
        heightOffPlatform = GetComponent<SpriteRenderer>().bounds.extents.y * 
            transform.localScale.magnitude;
        transform.parent = null;
        currentlyOn = dad;
        FlashOntoPlatform(this, transform, dad, uncle, heightOffPlatform);
        flashCountdown = secondsBetweenFlashes * UnityEngine.Random.Range(1f, 2f);
        secondsBetweenFlashes *= UnityEngine.Random.Range(0.3f, 1.4f);
        GetUncle(dad);
    }

    private static void FlashOntoPlatform(
        JumperGeneralThreat self,
        Transform trans, 
        JumperGeneralPlatform newDaddio,
        JumperGeneralPlatform oldDad,
        float heightOffPlatform)
    {
        if (!newDaddio)
            return;
        if (oldDad)
            oldDad.secondaryChildren.Remove(self);
        newDaddio.secondaryChildren.Add(self);
        Vector3 pBounds = newDaddio.GetHorizontalBounds();
        var parentLeft = pBounds.x;
        var parentRight = pBounds.z;
        pBounds = newDaddio.GetVerticalBounds();
        var parentTop = pBounds.z;
        trans.parent = null;
        trans.position = new Vector3(UnityEngine.Random.Range(parentLeft, parentRight), 
            parentTop + heightOffPlatform, 0);
    }

    private void SetBaseParameters()
    {
        rb = GetComponent<Rigidbody2D>();
        float diff = jm.GetDifficultyMultiplier();
        damage *= diff;
        maxHealth *= diff;
        currentHealth = maxHealth;
        acceleration *= diff;
        maxVelocity *= diff;
        rb.velocity = Vector2.zero;
        //takes collisions, doesnt initiate
        rb.isKinematic = true;
        //no gravity cuz we traveling along perimeter of platform
        rb.gravityScale = 0;
    }

    protected override string SetTag()
    {
        return jm.GetObstacleTag();
    }

    #endregion

    #region fixedUpdate

    void Update()
    {
        if (flashing)
            return;
        flashCountdown -= Time.deltaTime;
        if (flashCountdown <= 0)
        {
            Flash();
        }
    }

    private void Flash()
    {
        flashing = true;
        coll2D.enabled = false;
        animator.SetTrigger("Teleport");
    }

    // should be called by an event through the animator
    public void TeleportReappear()
    {
        currentlyOn = currentlyOn == dad ? uncle : dad;
        var oldDad = currentlyOn == dad ? uncle : dad;
        if (currentlyOn)
        {
            FlashOntoPlatform(this, transform, currentlyOn, oldDad, heightOffPlatform);
            animator.SetTrigger("FinishTeleport");
            flashing = false;
            flashCountdown = secondsBetweenFlashes;
        }
        else
        {
            Remove(true);
        }
    }

    public void TeleportComplete()
    {
        // all we're doing is re-enabling the collider, will also 
        // be called by the animator
        coll2D.enabled = true;
        if (!currentlyOn)
            Remove(true);
    }

    #endregion

    #region public


    public override void PlatformDestroyed(float totalTime, float flashInterval)
    {
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, totalTime, flashInterval));
    }

    public override void Remove(bool isImmediate)
    {
        rb.isKinematic = true;
        if (isImmediate)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.5f, 0.1f));
    }
    #endregion

    private void OnDestroy()
    {
        // no destroying this script without destroying the whole object
        Destroy(gameObject);
    }

    #region coroutine
    private void GetUncle(JumperGeneralPlatform dad)
    {
        uncle = dad.GetClosestPlatform();
    }
    #endregion
}
