using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperStartingPlatform : JumperGeneralPlatform
{
    #region variables
    [SerializeField]
    private float playerHeightToKill = 15f;
    private bool isStartingPlatform = true;
    #endregion

    #region overrides
    public override void Remove(bool immediately = false)
    {
        if (!isStartingPlatform && notFlashing)
        {
            notFlashing = false;
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, timeToRemove, flashPeriod, enemyChild));
        }
        else if (immediately)
        {
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.1f, 0.1f, enemyChild));
        }
        else
        {
            isStartingPlatform = false;
        }
    }
    private void FixedUpdate()
    {
        if (jm.GetMaxHeightReached() > playerHeightToKill)
        {
            Destroy(gameObject);
        }
    }
    #endregion
}
