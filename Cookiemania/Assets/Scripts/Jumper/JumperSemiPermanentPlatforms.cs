using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperSemiPermanentPlatforms : JumperGeneralPlatform
{
    #region variables
    [SerializeField]
    protected float playerHeightToRemoveThis = 15f;
    [SerializeField]
    protected bool neverRemove = false;
    protected bool removeCheck = true;
    
    #endregion

    #region overrides
    public override void Remove(bool immediately = false)
    {
        if (neverRemove) { return; }
        if (immediately)
        {
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.1f, 0.1f, enemyChild));
        }
        else if (!removeCheck && notFlashing)
        {
            notFlashing = false;
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, timeToRemove, flashPeriod, enemyChild));
        }
        else
        {
            removeCheck = false;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!neverRemove)
        {
            if (jm.GetMaxHeightReached() > playerHeightToRemoveThis)
            {
                Destroy(gameObject);
            }
        }
        
    }
    #endregion
}
