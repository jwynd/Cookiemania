using System;
using System.Collections.Generic;
using UnityEngine;

public class JumperMagnet : MonoBehaviour
{

    // sucks in coins
    // and any threats / enemies
    // disables their colliders, sucks em in and at end of suck coroutine
    // gives player their coin value

    // uncomment threat code to re-enable threat sucking interaction
    [SerializeField]
    protected JumperPlayerController myPlayer = null;
    [SerializeField]
    protected Transform suckInEnd = null;
    [SerializeField]
    protected float defaultSuckTime = 0.5f;
    // this likely should be on the parent object
    [SerializeField]
    protected Animator magnetAnimator = null;

    [SerializeField]
    protected AnimationCurve lerpCurve = null;

//    List<Tuple<JumperGeneralThreat, Vector3, Vector3>> threats = new List<Tuple<JumperGeneralThreat, Vector3, Vector3>>();
    List<Tuple<JumperGeneralPickup, Vector3, Vector3, float>> pickups = new List<Tuple<JumperGeneralPickup, Vector3, Vector3, float>>();

    protected float lifeTime = 0f;
    protected float lerpTime = 0f;
    protected Vector3 small = new Vector3(0.01f, 0.01f, 0.01f);
    protected Collider2D coll;
    protected Animator animator;
    protected float accumulatedPoints = 0f;

    public static float InterpolateOverCurve(AnimationCurve curve, float from, float to, float t)
    {
        return from + curve.Evaluate(t) * (to - from);
    }

    public static Vector3 InterpolateOverCurve(AnimationCurve curve, Vector3 from, Vector3 to, float t)
    {
        return from + curve.Evaluate(t) * (to - from);
    }

    protected void Awake()
    {
        coll = GetComponent<Collider2D>();
        coll.enabled = false;
        gameObject.SetActive(false);
    }

    public void ActivateMagnet(float time = 0f)
    {
        pickups.Clear();
        accumulatedPoints = 0;
        //      threats.Clear();
        lifeTime = time > 0.01f ? time : defaultSuckTime;
        lerpTime = 0f;
        gameObject.SetActive(true);
        coll.enabled = true;
        magnetAnimator.Play("magnet_start", -1, 0f);
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
            if (!pu.Item1) continue;
            pu.Item1.transform.position = InterpolateOverCurve(lerpCurve, pu.Item2, suckInEnd.position, lerpPoint);
            pu.Item1.transform.localScale = InterpolateOverCurve(lerpCurve, pu.Item3, small, lerpPoint);
        }
        /*foreach (var th in threats)
        {
            th.Item1.transform.position = Vector3.Lerp(th.Item2, suckInEnd.position, lerpPoint);
            th.Item1.transform.localScale = Vector3.Lerp(th.Item3, small, lerpPoint);
        }*/
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
            accumulatedPoints += pu.Item4;
            if (!pu.Item1) continue;
            pu.Item1.Remove();
        }
        /*foreach (var th in threats)
        {
            myPlayer.GivePoints(th.Item1.GetPointValue());
            th.Item1.Remove();
        }*/
        pickups.Clear();
        //threats.Clear();
        myPlayer.GivePoints(accumulatedPoints);
        accumulatedPoints = 0;
        coll.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var endDoor = collision.gameObject.GetComponent<JumperEndDoorController>();
        if (endDoor)
            return;
        //var threat = collision.gameObject.GetComponent<JumperGeneralThreat>();
        var pickup = collision.gameObject.GetComponent<JumperGeneralPickup>();
        if (/*threat ||*/ pickup)
        {
            var collider = collision.gameObject.GetComponent<Collider2D>();
            if (collider) collider.enabled = false;
            collision.transform.parent = null;
        }
        /*if (threat)
        {
            threats.Add(new Tuple<JumperGeneralThreat, Vector3, Vector3>(
                threat, threat.transform.position, threat.transform.localScale));
            accumulatedPoints += threats.GetPointValue();
        }*/
        if (pickup)
        {
            pickups.Add(new Tuple<JumperGeneralPickup, Vector3, Vector3, float>(
                pickup, pickup.transform.position, pickup.transform.localScale, pickup.DepletePickupPoints()));
        }
    }
}
