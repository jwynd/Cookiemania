using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperPlatformController : MonoBehaviour
{
    #region variables
    [SerializeField]
    private float timeToRemove = 1.5f;
    [SerializeField]
    private float flashPeriod = 0.25f;
    [SerializeField]
    private bool isStartingPlatform = false;

    [HideInInspector]
    public JumperEnemyController enemyChild = null;
    [HideInInspector]
    public float previousLedgeDirection = 0.0f;

    private bool notFlashing = true;
    private Renderer rend = null;
    private Collider2D coll = null;
    #endregion

    #region startup
    void Awake()
    {
        coll = GetComponent<Collider2D>();
        rend = GetComponent<Renderer>();
    }
    #endregion

    #region publicFunctions

    public void Remove(bool immediately = false)
    {
        if (!isStartingPlatform && notFlashing)
        {
            StartCoroutine(FlashThenKill());
        }
        else if(immediately)
        {
            
        }
        else
        {
            isStartingPlatform = false;
        }
    }

    IEnumerator FlashThenKill()
    {
        if (enemyChild != null) { enemyChild.PlatformDestroyed(timeToRemove, previousLedgeDirection); }
        notFlashing = false;
        int timer = (int)(timeToRemove / flashPeriod);
        for (int i = 0; i < 5; i++)
        {
            rend.material.color = Color.gray;
            yield return new WaitForSeconds(flashPeriod);
            rend.material.color = Color.white;
            yield return new WaitForSeconds(flashPeriod);
        }
        //this could be changed to recycling the object
        Destroy(gameObject);
    }
    #endregion
}
