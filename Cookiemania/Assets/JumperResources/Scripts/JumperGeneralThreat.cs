using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JumperGeneralThreat : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected float acceleration = 3.0f;
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
    protected JumperManagerGame jm;
    #endregion
    #region startup
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }
    protected virtual void Start()
    {
        jm = JumperManagerGame.Instance;
        //change if obstacle
        gameObject.tag = SetTag();
    }
    protected virtual string SetTag() { return jm.GetEnemyTag(); }

    #endregion
    #region public
    public virtual void TakesDamage(float dmg)
    {
        if (!IsIndestructable()) { currentHealth -= Mathf.Abs(dmg); }        
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
    public virtual void Remove(bool isImmediate = true) { Destroy(gameObject); }
    public virtual void RemoveOnDamage() { return; }
    #endregion
}
