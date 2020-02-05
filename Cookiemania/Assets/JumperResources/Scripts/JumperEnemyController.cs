using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemyController : MonoBehaviour
{
    #region variables
    [SerializeField]
    private bool destructable = true;
    [SerializeField]
    private float health = 5f;
    [SerializeField]
    private float damage = 2.0f;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float maxVelocity = 5.0f;

    //usage. once player gets 15 units above this object, kill it
    private float originalHeight;
    private float parentLeft;
    private float parentRight;
    private float direction = 1;
    private Rigidbody2D rb;
    private JumperManager jm;
    private bool fallingToMyDeath = false;
    #endregion

    #region startup
    void Start()
    {
        jm = JumperManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        transform.parent.GetComponent<JumperPlatformController>().enemyChild = this;
        float parentXBound = GetComponentInParent<Collider2D>().bounds.extents.x;
        float xBound = GetComponent<Collider2D>().bounds.extents.x;
        float diff = jm.GetDifficultyMultiplier();
        damage *= diff;
        health *= diff;
        speed *= diff;
        originalHeight = transform.position.y;
        rb.velocity = Vector2.zero;

        //get info from parent, set parent child to this, then 
        parentLeft = transform.parent.position.x  - (parentXBound + xBound);
        parentRight = parentLeft + (parentXBound + xBound) * 2;
        Debug.Log(parentLeft + " to " + parentRight);
        if (transform.parent.position.x < transform.position.x)
        {
            direction = -1;
        }
        transform.parent = null;
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
        if (!fallingToMyDeath)
        {
            CheckBounds();
            Walk();
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
    public void TakesDamage(float damage)
    {
        health -= damage;
        if (health <= 0) { Destroy(gameObject); }
    }
    
    public float GetDamage()
    {
        return damage;
    }

    public bool GetDestructable()
    {
        return destructable;
    }

    public void PlatformDestroyed(float timer, float jumpDirection)
    {
        StartCoroutine(PlatformDestroyedHelper(timer, jumpDirection));
    }
    #endregion

    #region coroutine
    private IEnumerator PlatformDestroyedHelper(float timer, float jumpDirection)
    {
        JumpOff(jumpDirection);
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

    private void JumpOff(float direction)
    {
        fallingToMyDeath = true;
        //moves to the edge of the ledge and jumps off
    }

    #endregion
}
