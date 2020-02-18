﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemyController : JumperPlatformAttachables
{
    #region variables
    
    [SerializeField]
    private Vector2 jump = new Vector2(2f, 4f);

    private float parentLeft;
    private float parentRight;
    private float direction = 1;
    private Rigidbody2D rb;
    private bool jumpToMyDeath = false;
    private Animator anim;
    #endregion

    #region startup
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        JumperPlatformController dad = transform.parent.GetComponent<JumperPlatformController>();
        dad.enemyChild = this;
        float diff = jm.GetDifficultyMultiplier();
        damage *= diff;
        maxHealth *= diff;
        currentHealth = maxHealth;
        speed *= diff;
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
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
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
            Walk();
            Jump();
        }
    }

    private void Jump()
    {
        if ((direction < 0 && rb.position.x < parentLeft) || (direction > 0 && rb.position.x > parentRight))
        {
            rb.isKinematic = false;
            rb.gravityScale = 1;
            rb.AddForce(new Vector2(jump.x * direction, jump.y), ForceMode2D.Impulse);
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
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (direction > 0 && rb.position.x > parentRight)
        {
            rb.velocity = Vector2.zero;
            direction = -1;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }

    private void Walk()
    { 
        rb.velocity = new Vector2(Mathf.Clamp(direction * speed + rb.velocity.x, -maxVelocity, maxVelocity), rb.velocity.y);
    }

    #endregion

    #region public
    public override void Remove()
    {
        StartCoroutine(JumperManager.FlashThenKill(gameObject, 0.5f, 0.1f));
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
