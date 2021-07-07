﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPickupController : JumperGeneralPickup
{
    // Start is called before the first frame update
    [SerializeField]
    protected float explosionTimer = 0.75f;
    [SerializeField]
    protected int explosionFlashCount = 3;
    [SerializeField]
    protected int explosionFrames = 5;
    [SerializeField]
    protected float explosionSize = 3f;

    protected Rigidbody2D myRb;
    protected string enemyTag;
    protected string obstacleTag;
    protected float parentLeft;
    protected float parentRight;
    protected bool exploding;

    protected override void Awake()
    {
        base.Awake();
        myRb = GetComponent<Rigidbody2D>();
        myRb.isKinematic = true;
        myRb.gravityScale = 0;
        enemyTag = JumperManagerGame.Instance.GetEnemyTag();
        obstacleTag = JumperManagerGame.Instance.GetObstacleTag();

    }
    protected override void Start()
    {
        base.Start();
        //change this to x when we switch sprites lol
        JumperGeneralPlatform dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        Vector3 pBounds = dad.GetHorizontalBounds();

        //get info from parent, set parent child to this, then 
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        transform.position = new Vector2(Random.Range(parentLeft, parentRight), transform.position.y);


    }

    //unparent, then activate gravity, kinematics and enable normal collisions
    //would probably want an animation that shows the trap/bomb is active now
    public void Thrown(Vector2 strength)
    {
        RemoveFromParent();
        transform.parent = null;
        myRb.isKinematic = false;
        myRb.gravityScale = 1;
        gameObject.tag = "Untagged";
        exploding = true;
        myRb.AddForce(strength, ForceMode2D.Impulse);
        StartCoroutine(Explode());
    }

    protected IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosionTimer);
        for (int i = 0; i < explosionFlashCount; i++)
        {
            transform.localScale *= explosionSize;
            GetComponent<BoxCollider2D>().size *= explosionSize;
            yield return new WaitForSeconds(Time.fixedDeltaTime * explosionFrames);
            transform.localScale /= explosionSize;
            GetComponent<BoxCollider2D>().size /= explosionSize;
            yield return new WaitForSeconds(Time.fixedDeltaTime * explosionFrames);
        }
        Remove();
    }


    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(obstacleTag) || collision.gameObject.CompareTag(enemyTag))
        {
            CollisionHelper(collision.gameObject);
        }
    }




    protected void RemoveFromParent()
    {
        if (transform.parent != null)
        {
            if (transform.parent.TryGetComponent<JumperPlayerController>(out JumperPlayerController p))
            {
                p.PickupDestroyed();
            }
        }
    }

    protected void CollisionHelper(GameObject collided)
    {
        JumperGeneralThreat script = collided.GetComponent<JumperGeneralThreat>();
        if (script != null)
        {
            script.TakesDamage(damage);
            if (!exploding)
            {
                Remove();
            }
        }
    }

    public override void Remove()
    {
        RemoveFromParent();
        base.Remove();
    }
}
