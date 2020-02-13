using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperObstacleController : JumperPlatformAttachables
{
    #region variables
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float maxVelocity = 5.0f;
    [SerializeField]
    private float rotationSpeed = 3.0f;

    private float parentLeft;
    private float parentRight;
    private float parentTop;
    private float parentBottom;
    private float direction = 1;
    private float ydirection = 0;
    private Rigidbody2D rb;
    #endregion

    #region startup
    void Start()
    {
        jm = JumperManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        JumperPlatformController dad = transform.parent.GetComponent<JumperPlatformController>();
        dad.enemyChild = this;
        float diff = jm.GetDifficultyMultiplier();
        damage *= diff;
        health *= diff;
        speed *= diff;
        maxVelocity *= diff;
        rb.velocity = Vector2.zero;
        //takes collisions, doesnt initiate
        rb.isKinematic = true;
        //no gravity cuz we traveling along perimeter of platform
        rb.gravityScale = 0;
        Vector3 pBounds = dad.GetHorizontalBounds();
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        pBounds = dad.GetVerticalBounds();
        parentTop = pBounds.z;
        parentBottom = pBounds.y;
        Debug.Log(parentLeft + " to " + parentRight);
        transform.parent = null;
        transform.position = new Vector2(UnityEngine.Random.Range(parentLeft, parentRight), transform.position.y);

    }
    #endregion

    #region fixedUpdate

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckBounds();
        TraversePlatform();
        Spin();
    }

    

    private void CheckBounds()
    {
        if (direction < 0 && rb.position.x < parentLeft)
        {
            //goes up
            rb.velocity = Vector2.zero;
            ydirection = 1;
            direction = 0;
            //probably a 90* rotation each time
        }
        else if (direction > 0 && rb.position.x > parentRight)
        {
            //goes down
            rb.velocity = Vector2.zero;
            direction = 0;
            ydirection = -1;
        }
        else if (ydirection > 0 && rb.position.y > parentTop)
        {
            //goes right
            rb.velocity = Vector2.zero;
            direction = 1;
            ydirection = 0;

        }
        else if (ydirection < 0 && rb.position.y < parentBottom)
        {
            //goes left
            rb.velocity = Vector2.zero;
            direction = -1;
            ydirection = 0;
        }
    }

    private void Spin()
    {
        transform.Rotate(new Vector3(0, 0, -rotationSpeed));
    }

    private void TraversePlatform()
    {
        rb.velocity = new Vector2(Mathf.Clamp(direction * speed + rb.velocity.x, -maxVelocity, maxVelocity), 
            Mathf.Clamp(ydirection * speed + rb.velocity.y, -maxVelocity, maxVelocity));
    }

    #endregion

    #region public
    public override void TakesDamage(float damage)
    {
        health -= damage;
        if (health <= 0) 
        {
            StartCoroutine(JumperManager.FlashThenKill(gameObject, 0.5f, 0.1f));
        }
    }


    public override void PlatformDestroyed(float totalTime, float flashInterval)
    {
        StartCoroutine(JumperManager.FlashThenKill(gameObject, totalTime, flashInterval));
    }
    #endregion

    #region coroutine

    #endregion
}
