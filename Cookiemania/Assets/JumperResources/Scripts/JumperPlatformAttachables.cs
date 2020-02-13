using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class JumperPlatformAttachables : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected bool destructable;
    [SerializeField]
    protected float health;
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected float pointValue;
    protected JumperManager jm;
    #endregion
    #region public
    public abstract void TakesDamage(float dmg);
    public virtual float GetDamage() { return damage; }
    public virtual bool GetDestructable() { return destructable; }
    //probably a coroutine implementation
    public abstract void PlatformDestroyed(float totalTime, float flashPeriod);
    public virtual float GetPointValue() { return pointValue; }
    #endregion
}
