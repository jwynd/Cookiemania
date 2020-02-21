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
            //this is trigger, so dont' need to check height
            collision.gameObject.GetComponent<JumperPlayerController>().BouncePlayer(bounceMultiplier);
            
        }

    }
}
