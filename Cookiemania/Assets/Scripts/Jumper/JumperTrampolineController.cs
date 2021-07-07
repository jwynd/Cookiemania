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

    protected IEnumerator Sproing()
    {
        var maxHeightDiff = -1.0f;
        var originalHeight = transform.position.y;
        var jiggleTime = 0.2f;
        for (int i = 0; i < 6; i++)
        {
            float transformedHeight;
            if (i % 2 == 1)
            {
                maxHeightDiff *= 0.5f;
                transformedHeight = originalHeight;
            }
            else
            {
                transformedHeight = maxHeightDiff + originalHeight;
            }
            var target = new Vector3(transform.position.x, transformedHeight, transform.position.z);
            while (transform.position.y != transformedHeight)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, jiggleTime);
                yield return new WaitForFixedUpdate();
            }
            jiggleTime *= 0.5f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            //this is trigger, so dont' need to check height
            StopCoroutine(Sproing());
            JumperSounds.Instance.Bounce();
            collision.gameObject.GetComponent<JumperPlayerController>().BouncePlayer(bounceMultiplier);
            StartCoroutine(Sproing());
        }

    }
}
