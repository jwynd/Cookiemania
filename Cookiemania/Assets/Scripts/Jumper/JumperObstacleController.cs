using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperObstacleController : JumperGeneralThreat
{
    #region variables
    [SerializeField]
    private float rotationSpeed = 3.0f;

    private float parentLeft;
    private float parentRight;
    private Rigidbody2D rb;
    protected JumperGeneralPlatform dad;
    protected JumperGeneralPlatform uncle;
    #endregion

    #region startup
    protected override void Start()
    {
        base.Start();
        SetBaseParameters();
        dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        dad.enemyChild = this;
        Vector3 pBounds = dad.GetHorizontalBounds();
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        transform.parent = null;
        transform.position = new Vector2(UnityEngine.Random.Range(parentLeft, parentRight), transform.position.y);
        IEnumerator coroutine = DelayedGetUncle(dad);
        StartCoroutine(coroutine);
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

    void FixedUpdate()
    {
        
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
