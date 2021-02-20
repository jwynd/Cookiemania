using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperAI : MonoBehaviour
{

    protected Transform leader = null;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        if (leader == null)
            enabled = false;
    }

    public void SetFollowPoint(Transform toFollow)
    {
        leader = toFollow;
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
    }
}
