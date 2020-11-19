using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this general class is instantiatable
public class JumperGeneralPlatform : MonoBehaviour
{
    #region variables
    [SerializeField]
    protected float timeToRemove = 1.5f;
    [SerializeField]
    protected float flashPeriod = 0.25f;

    [HideInInspector]
    public JumperGeneralThreat enemyChild = null;

    [SerializeField]
    protected bool placePlatformsAbove = true;

    protected JumperManagerGame jm;
    protected bool notFlashing = true;
    protected SpriteRenderer rend = null;
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
    }
    #endregion

    #region publicFunctions

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
        if (enemyChild != null)
        {
            Destroy(enemyChild);
        }
    }

    public virtual void Remove(bool immediately = false)
    {
        if (notFlashing)
        {
            notFlashing = false;
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, timeToRemove, flashPeriod, enemyChild));
        }
        else if (immediately)
        {
            StartCoroutine(JumperManagerGame.FlashThenKill(gameObject, 0.1f, 0.1f, enemyChild));
        }
    }
    #endregion
}
