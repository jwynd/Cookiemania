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
    private float parentTop;
    private float parentBottom;
    private float direction = 1;
    private float ydirection = 0;
    private Rigidbody2D rb;
    #endregion

    #region startup
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        JumperGeneralPlatform dad = transform.parent.GetComponent<JumperGeneralPlatform>();
        dad.enemyChild = this;
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

    protected override string SetTag()
    {
        return jm.GetObstacleTag();
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
        rb.velocity = new Vector2(Mathf.Clamp(direction * acceleration + rb.velocity.x, -maxVelocity, maxVelocity), 
            Mathf.Clamp(ydirection * acceleration + rb.velocity.y, -maxVelocity, maxVelocity));
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

    #endregion
}
