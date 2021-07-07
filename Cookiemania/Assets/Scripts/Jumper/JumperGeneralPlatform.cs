﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this general class is instantiatable
public class JumperGeneralPlatform : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected float timeToRemove = 2f;
    [SerializeField]
    protected float flashPeriod = 0.25f;
    [SerializeField]
    protected bool isVertical = false;
    [SerializeField]
    protected Transform verticalPoint;

    [HideInInspector]
    public JumperGeneralThreat enemyChild = null;

    [HideInInspector]
    public HashSet<JumperGeneralThreat> secondaryChildren = new HashSet<JumperGeneralThreat>();

    [HideInInspector]
    public HashSet<JumperGeneralPickup> tertiaryChildren = new HashSet<JumperGeneralPickup>();

    [SerializeField]
    protected bool placePlatformsAbove = true;

    protected JumperManagerGame jm;
    protected bool notFlashing = true;
    protected SpriteRenderer rend = null;
    private JumperGeneralPlatform myChild;
    protected string pickupTag = "";
    #endregion

    #region startup
    protected virtual void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        jm = JumperManagerGame.Instance;
        gameObject.tag = jm.GetGroundTag();
        pickupTag = jm.GetCollectiblesTag();
        if (IsVertical)
        {
            myChild = GetComponentsInChildren<JumperGeneralPlatform>()[1];
        }
    }
    #endregion

    #region publicFunctions

    public bool IsVertical { get { return isVertical; } }

    public float Top()
    {
        return verticalPoint.position.y;
    }

    //left x position = .x, right x position = .z
    public Vector3 GetHorizontalBounds()
    {
        Vector3 bounds = transform.position;
        bounds.z = bounds.x + rend.bounds.extents.x;
        bounds.x -= rend.bounds.extents.x;
        return bounds;
    }

    //upper y in z, lower y bound in y
    public Vector3 GetVerticalBounds()
    {
        Vector3 bounds = transform.position;
        bounds.z = bounds.y + rend.bounds.extents.y;
        bounds.y -= rend.bounds.extents.y;
        return bounds;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(pickupTag))
        {
            collision.gameObject.GetComponent<JumperGeneralPickup>().BounceMeOut();
        }
    }

    public JumperGeneralPlatform GetClosestPlatform(bool trampolineAllowed = false)
    {
        var brethren = transform.parent.GetComponentsInChildren<Transform>();
        Transform nearest = null;
        var currentDistance = Mathf.Infinity;
        foreach (var trans in brethren)
        {
            if (trans == transform)
                continue;
            if (trans.gameObject.activeSelf != true)
                continue;
            if (!trampolineAllowed && 
                trans.GetComponent<JumperTrampolineController>() != null)
                continue;
            var distance = Vector3.Distance(transform.position, trans.position);
            if (distance < currentDistance)
            {
                currentDistance = distance;
                nearest = trans;
            }
        }
        return nearest?.GetComponent<JumperGeneralPlatform>();
    }

    public bool CanPlacePlatformsAbove()
    {
        return placePlatformsAbove;
    }

    //ensures child ALWAYS destroyed when this is destroyed. gotta make sure ya know
    public void OnDestroy()
    {
        
        if (enemyChild)
            Destroy(enemyChild);
        foreach(var child in secondaryChildren)
            if (child)
                Destroy(child);
        foreach (var child in tertiaryChildren)
            if (child)
                Destroy(child);
        secondaryChildren.Clear();
    }

    public virtual void Remove(bool immediately = false)
    {
        JumperSounds.Instance?.PlatformCollapse();
        var tRemoveTime = 0.01f;
        var tFlashPeriod = 0.01f;
        myChild?.Remove();
        if (!immediately)
        {
            tRemoveTime = timeToRemove;
            tFlashPeriod = flashPeriod;
        }
        if (notFlashing)
        {
            notFlashing = false;
            StartCoroutine(JumperManagerGame.FlashThenKill(
                gameObject, tRemoveTime, tFlashPeriod, enemyChild));
            foreach(var child in secondaryChildren)
                if (child)
                    StartCoroutine(JumperManagerGame.FlashThenKill(
                        child.gameObject, tRemoveTime, tFlashPeriod));
            foreach(var child in tertiaryChildren)
                if (child)
                    StartCoroutine(JumperManagerGame.FlashThenKill(
                        child.gameObject, tRemoveTime, tFlashPeriod));
        }
    }
    #endregion
}
