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
    protected float flashCountdown = 0f;
    private bool flashing = false;
    private float heightOffPlatform = 1.5f;
    #endregion

    #region startup
    protected override void Start()
    {
        base.Start();
        SetBaseParameters();
        animator = GetComponent<Animator>();
        dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        heightOffPlatform = GetComponent<SpriteRenderer>().bounds.extents.y * 
            transform.localScale.magnitude;
        transform.parent = null;
        currentlyOn = dad;
        FlashOntoPlatform(transform, dad, uncle, heightOffPlatform);
        flashCountdown = secondsBetweenFlashes * UnityEngine.Random.Range(1f, 2f);
        secondsBetweenFlashes *= UnityEngine.Random.Range(0.3f, 1.4f);
        IEnumerator coroutine = DelayedGetUncle(dad);
        StartCoroutine(coroutine);
    }

    private static void FlashOntoPlatform(Transform trans, 
        JumperGeneralPlatform newDaddio,
        JumperGeneralPlatform oldDad,
        float heightOffPlatform)
    {
        if (oldDad)
            oldDad.enemyChild = null;
        newDaddio.enemyChild = trans.GetComponent<JumperObstacleController>();
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
        animator.SetTrigger("Teleport");
    }

    // should be called by an event through the animator
    public void TeleportComplete()
    {
        animator.SetTrigger("FinishTeleport");
        currentlyOn = currentlyOn == dad ? uncle : dad;
        var oldDad = currentlyOn == dad ? uncle : dad;
        if (currentlyOn != null)
        {
            FlashOntoPlatform(transform, currentlyOn, oldDad, heightOffPlatform);
        }
        flashing = false;
        flashCountdown = secondsBetweenFlashes;
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
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.05f, 0.05f));

        }
        StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.5f, 0.1f));
    }
    #endregion

    #region coroutine
    private IEnumerator DelayedGetUncle(JumperGeneralPlatform dad)
    {
        // waiting a couple frames, then we're looking for our uncle
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        uncle = dad.GetClosestPlatform();
    }
    #endregion
}
