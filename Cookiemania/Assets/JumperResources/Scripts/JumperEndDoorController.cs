using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperEndDoorController : JumperGeneralPickup
{
    [SerializeField]
    protected float controlSpeed = 1.0f;
    [SerializeField]
    protected Sprite openedDoor = null;

    protected Sprite closedDoor;
    protected string playerTag;
    protected bool runOnce = false;

    //this can be run in awake cuz jumper manager spawns pickups
    protected override void Awake()
    {
        base.Awake();
        closedDoor = GetComponent<SpriteRenderer>().sprite;
        playerTag = JumperManagerGame.Instance.GetPlayerTag();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //switch this back to playerTag when game manager starts spawning this
        if (collision.gameObject.CompareTag(playerTag) && !runOnce)
        {
            //take control of player to walk through door during end
            runOnce = true;
            StartCoroutine(PlayerWalkThroughDoor(collision.gameObject));
            JumperManagerUI.Instance.End(true, true, transform);
        }
    }

    protected IEnumerator PlayerWalkThroughDoor(GameObject pc)
    {
        GetComponent<SpriteRenderer>().sprite = openedDoor;
        while (transform.position != pc.transform.position)
        {
            pc.transform.position = Vector3.MoveTowards(pc.transform.position, transform.position, controlSpeed * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.05f);
        pc.GetComponent<Renderer>().enabled = false;
        foreach(Transform child in pc.GetComponentsInChildren<Transform>()) {
            Renderer childRend = child.GetComponent<Renderer>();
            if (childRend != null)
            {
                childRend.enabled = false;
            }
        }
        GetComponent<SpriteRenderer>().sprite = closedDoor;
    }
}
