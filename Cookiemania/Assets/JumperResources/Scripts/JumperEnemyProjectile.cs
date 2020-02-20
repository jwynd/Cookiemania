using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEnemyProjectile : JumperGeneralThreat
{   
    [SerializeField]
    protected float rotationSpeed = 3.0f;
    [SerializeField]
    protected float lifeTime = 2.0f;

    protected bool thrownOnce = false;
    protected Rigidbody2D rb;
    protected Collider2D coll;
    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        rb.isKinematic = true;
        coll.enabled = false;
        coll.isTrigger = true;
        rb.gravityScale = 0;
    }

    protected void FixedUpdate()
    {
        if (thrownOnce)
        {
            transform.Rotate(new Vector3(0, 0, rotationSpeed));
            lifeTime -= Time.fixedDeltaTime;
            if (lifeTime < 0)
            {
                Remove();
            }
        }
        
    }

    public void ThrowProjectile(Vector2 throwVector)
    {
        if (thrownOnce) { return; }
        thrownOnce = true;
        coll.enabled = true;
        rb.isKinematic = false;
        rb.gravityScale = 1;
        rb.velocity = Vector2.zero;
        rb.AddForce(throwVector, ForceMode2D.Impulse);
    }

    public override void Remove(bool isImmediate = true)
    {
        Destroy(gameObject);
    }

    public override void PlatformDestroyed(float a, float b)
    {
        //this guy doesnt care about platform destruction
        return;
    }

    //projectile so should call remove when it hits
    //default is that it calls nothing
    public override void RemoveOnDamage()
    {
        Remove();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(jm.GetGroundTag()))
        {
            Remove();
        }
    }
}
