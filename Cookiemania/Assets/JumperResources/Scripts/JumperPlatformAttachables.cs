using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JumperPlatformAttachables : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected float speed = 3.0f;
    [SerializeField]
    protected float maxVelocity = 5.0f;
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected float pointValue;
    [SerializeField]
    protected bool indestructable;
    [SerializeField]
    protected float maxHealth;
    protected float currentHealth;
    protected JumperManager jm;
    #endregion
    #region startup
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    protected virtual void Start()
    {
        jm = JumperManager.Instance;
    }
    #endregion
    #region public
    public virtual void TakesDamage(float dmg)
    {
        if (!IsIndestructable()) { currentHealth -= dmg; }        
        if (currentHealth <= 0)
        {
            jm.player.GivePoints(pointValue);
            Remove();
        }
    }
    public virtual float GetDamage() { return damage; }
    public virtual bool IsIndestructable() { return indestructable; }
    //probably a coroutine implementation
    public abstract void PlatformDestroyed(float totalTime, float flashPeriod);
    public virtual float GetPointValue() { return pointValue; }
    public virtual void Remove() { Destroy(gameObject); }
    #endregion
}
