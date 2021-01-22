using System;
using System.Collections.Generic;
using UnityEngine;

public class JumperMagnet : MonoBehaviour
{

    // sucks in coins
    // and any threats / enemies
    // disables their colliders, sucks em in and at end of suck coroutine
    // gives player their coin value
    [SerializeField]
    protected JumperPlayerController myPlayer = null;
    [SerializeField]
    protected Transform suckInEnd = null;
    [SerializeField]
    protected float defaultSuckTime = 0.5f;

    List<Tuple<JumperGeneralThreat, Vector3, Vector3>> threats = new List<Tuple<JumperGeneralThreat, Vector3, Vector3>>();
    List<Tuple<JumperGeneralPickup, Vector3, Vector3>> pickups = new List<Tuple<JumperGeneralPickup, Vector3, Vector3>>();

    protected float lifeTime = 0f;
    protected float lerpTime = 0f;
    protected Vector3 small = new Vector3(0.01f, 0.01f, 0.01f);
    protected Collider2D coll;

    protected void Start()
    {
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
    }

    public void ActivateMagnet(float time = 0f)
    {
        pickups.Clear();
        threats.Clear();
        lifeTime = time > 0.01f ? time : defaultSuckTime;
        lerpTime = 0f;
        gameObject.SetActive(true);
        coll.enabled = true;
    }

    private void Update()
    {
        if (lifeTime <= Time.deltaTime)
        {
            DestroyObjects();
            gameObject.SetActive(false);
            return;
        }
        lifeTime -= Time.deltaTime;
        var lerpPoint = lerpTime / lifeTime;
        foreach (var pu in pickups)
        {
            pu.Item1.transform.position = Vector3.Lerp(pu.Item2, suckInEnd.position, lerpPoint);
            pu.Item1.transform.localScale = Vector3.Lerp(pu.Item3, small, lerpPoint);
        }
        foreach (var th in threats)
        {
            th.Item1.transform.position = Vector3.Lerp(th.Item2, suckInEnd.position, lerpPoint);
            th.Item1.transform.localScale = Vector3.Lerp(th.Item3, small, lerpPoint);
        }
        lerpTime += Time.deltaTime;
    }

    private void OnDisable()
    {
        DestroyObjects();
    }

    private void DestroyObjects()
    {
        foreach (var pu in pickups)
        {
            myPlayer.GivePoints(pu.Item1.PointsOnPickup());
            pu.Item1.Remove();
        }
        foreach (var th in threats)
        {
            myPlayer.GivePoints(th.Item1.GetPointValue());
            th.Item1.Remove();
        }
        pickups.Clear();
        threats.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var endDoor = collision.gameObject.GetComponent<JumperEndDoorController>();
        if (endDoor)
            return;
        var threat = collision.gameObject.GetComponent<JumperGeneralThreat>();
        var pickup = collision.gameObject.GetComponent<JumperGeneralPickup>();
        if (threat || pickup)
        {
            var collider = collision.gameObject.GetComponent<Collider2D>();
            if (collider) collider.enabled = false;
            collision.transform.parent = null;
        }
        if (threat)
        {
            threats.Add(new Tuple<JumperGeneralThreat, Vector3, Vector3>(
                threat, threat.transform.position, threat.transform.localScale));
        }
        if (pickup)
        {
            pickups.Add(new Tuple<JumperGeneralPickup, Vector3, Vector3>(
                pickup, pickup.transform.position, pickup.transform.localScale));
        }
    }
}
