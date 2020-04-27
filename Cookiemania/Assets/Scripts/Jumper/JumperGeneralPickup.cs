﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JumperGeneralPickup : MonoBehaviour
{
    #region variables
    [Tooltip("For pickups that deal damage")]
    [SerializeField]
    protected float damage = 25.0f;
    
    [Tooltip("For pickups that give points")]
    [SerializeField]
    protected float pointsOnPickup = 5f;
    [SerializeField]
    protected bool automaticallyPickup = false;


    protected Collider2D myCollider;
    
    #endregion

    #region overridable
    protected virtual void Awake()
    {
        myCollider = GetComponent<Collider2D>();
        myCollider.isTrigger = true;      
    }

    protected virtual void Start()
    {
        gameObject.tag = JumperManagerGame.Instance.GetCollectiblesTag();
    }
    #endregion

    #region noOverride
    public bool IsAutomaticPickup()
    {
        return automaticallyPickup;
    }
    #endregion

}
