using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPickupController : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage = 5.0f;
    [SerializeField]
    private float explosionTimer = 0.75f;

    private Collider2D myCollider;
    private Rigidbody2D myRb;
    private float parentLeft;
    private float parentRight;
    

    void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        myRb = gameObject.GetComponent<Rigidbody2D>();
        myCollider.isTrigger = true;
        myRb.isKinematic = true;
        myRb.gravityScale = 0;
        //change this to x when we switch sprites lol
        JumperPlatformController dad = transform.parent.GetComponent<JumperPlatformController>();

        Vector3 pBounds = dad.GetHorizontalBounds();

        //get info from parent, set parent child to this, then 
        parentLeft = pBounds.x;
        parentRight = pBounds.z;
        transform.position = new Vector2(UnityEngine.Random.Range(parentLeft, parentRight), transform.position.y);
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }



    //unparent, then activate gravity, kinematics and enable normal collisions
    //would probably want an animation that shows the trap/bomb is active now
    public void Thrown(Vector2 strength)
    {

        transform.parent = null;
        myRb.isKinematic = false;
        myCollider.isTrigger = false;
        myRb.gravityScale = 1;
        myRb.AddForce(strength, ForceMode2D.Impulse);
        RemoveFromParent();
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosionTimer);
        transform.localScale *= 3;
        GetComponent<BoxCollider2D>().size *= 3;
        yield return new WaitForSeconds(Time.fixedDeltaTime * 5);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {           
            CollisionHelper(collision.gameObject);          
        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            float x = collision.gameObject.GetComponent<Collider2D>().bounds.size.x / 2;
            float myX = GetComponent<Collider2D>().bounds.size.x / 2;
            if (collision.transform.position.y < transform.position.y)
            {
                //check if midpoint of object is in x bounds of platform
                if (collision.transform.position.x + x > transform.position.x - myX && 
                    collision.transform.position.x - x < transform.position.x + myX)
                {
                    myRb.velocity = Vector2.zero;
                    myRb.isKinematic = false;
                    myCollider.isTrigger = true;
                    myRb.gravityScale = 0;
                }                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            //we'll just say all obstacles can be immediately destroyed
            //currently destroys obstacles it runs into even when held
            //obviously if this spawns and touches an obstacle even before the player 
            //picks it up, itll destroy it

            RemoveFromParent();
            Destroy(collision.gameObject);
            Destroy(gameObject);

        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            CollisionHelper(collision.gameObject);

        }
    }

    private void RemoveFromParent()
    {
        if (transform.parent != null)
        {
            if (transform.parent.TryGetComponent<JumperPlayerController>(out JumperPlayerController p))
            {
                p.PickupDestroyed();
            }
        }
    }

    private void CollisionHelper(GameObject collided)
    {
        JumperEnemyController script = collided.GetComponent<JumperEnemyController>();
        if (script != null)
        {
            if (script.GetDestructable())
            {
                Destroy(collided.gameObject);

            }
            // i imagine there'd be other options at some point, like if its not destructable
            //then maybe it gives a ton of points and destroys this object, or it just does some 
            //damage to it
            else
            {
                script.TakesDamage(damage);

            }
            RemoveFromParent();
            Destroy(gameObject);
        }
    }
}
