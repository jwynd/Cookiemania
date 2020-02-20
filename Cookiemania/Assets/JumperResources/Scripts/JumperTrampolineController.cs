using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperTrampolineController : JumperGeneralPlatform
{
    [SerializeField]
    private float bounceMultiplier = 3f;

    private string playerTag;

    protected override void Awake()
    {
        base.Awake();
        GetComponent<Collider2D>().isTrigger = true;
    }

    protected override void Start()
    {
        base.Start();
        playerTag = jm.GetPlayerTag();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            if (collision.gameObject.transform.position.y + collision.gameObject.GetComponent<Renderer>().bounds.extents.y >=
                transform.position.y + rend.bounds.extents.y)
            {
                collision.gameObject.GetComponent<JumperPlayerController>().BouncePlayer(bounceMultiplier);
            }
        }

    }
}
