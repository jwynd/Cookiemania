using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperAI : MonoBehaviour
{

    protected Transform leader = null;
    protected Transform lookPoint = null;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    private SpriteRenderer rend;

    private void Awake()
    {
        if (leader == null)
            enabled = false;
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetFollowPoint(Transform toFollow, Transform focusPoint)
    {
        leader = toFollow;
        lookPoint = focusPoint;
        enabled = true;
    }

    public void Die()
    {
        // animate this
    }

    public void Dance()
    {
        // animate this
    }

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.position, leader.position, ref velocity, smoothTime);
        rend.flipX = transform.position.x - lookPoint.position.x > 0;
    }
}
