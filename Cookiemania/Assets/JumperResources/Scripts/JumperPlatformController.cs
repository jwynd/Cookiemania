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
    public JumperGeneralThreat enemyChild = null;

    private bool notFlashing = true;
    private Renderer rend = null;
    #endregion

    #region startup
    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.tag = JumperManagerGame.Instance.GetGroundTag();
    }
    #endregion

    #region publicFunctions

    //left x position = .x, right x position = .z, y center position = .y 
    public Vector3 GetHorizontalBounds()
    {
        Vector3 bounds = transform.position;
        bounds.z = bounds.x + rend.bounds.extents.x;
        bounds.x -= rend.bounds.extents.x;
        return bounds;
    }

    //upper y in z, lower y bound in y, x center position in x
    public Vector3 GetVerticalBounds()
    {
        Vector3 bounds = transform.position;
        bounds.z = bounds.y + rend.bounds.extents.y;
        bounds.y -= rend.bounds.extents.y;
        return bounds;
    }

    //ensures child ALWAYS destroyed when this is destroyed. gotta make sure ya know
    public void OnDestroy()
    {
        if (enemyChild != null)
        {
            Destroy(enemyChild);
        }
    }

    public void Remove(bool immediately = false)
    {
        if (!isStartingPlatform && notFlashing)
        {
            notFlashing = false;
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, timeToRemove, flashPeriod, enemyChild));
        }
        else if(immediately)
        {
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.1f, 0.1f, enemyChild));
        }
        else
        {
            isStartingPlatform = false;
        }
    }

    #endregion
}
