using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Website : MonoBehaviour
{
    [SerializeField]
    protected GameObject websiteCustomizationPrefab = null;
    [SerializeField]
    protected GameObject websitePrefab = null;

    protected bool customizedOnce = false;
    protected GameObject websiteRef = null;
    protected GameObject websiteCustomizationRef = null;

    private void Awake()
    {
        websiteRef = Instantiate(websitePrefab);
        websiteCustomizationRef = Instantiate(websiteCustomizationPrefab);
        websiteRef.transform.parent = transform;
        websiteCustomizationRef.transform.parent = transform;
        websiteRef.SetActive(false);
        websiteCustomizationRef.SetActive(false);
    }

    private void OnEnable()
    {
        if (!customizedOnce)
        {
            CustomizationManager.Instance.CustomizationStart();
        }
        else
        {
            //go to website instead
        }
    }
}
