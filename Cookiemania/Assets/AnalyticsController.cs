using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsController : MonoBehaviour
{
    [SerializeField]
    protected GameObject AnalyticsUI = null;
    [SerializeField]
    protected GameObject homeTab = null;

    protected GameObject websiteCustomizationRef = null;
    [SerializeField]
    protected GameObject ShopCanvas = null;
    protected Canvas customizationCanvas = null;

    protected void ReturnFunction()
    {
        homeTab.GetComponent<General_TabButton>().click();
    }

    private void Awake()
    {
        ShopCanvas.SetActive(false);
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        ShopCanvas.SetActive(true);
    }

    private void OnDisable()
    {
        if (ShopCanvas)
            ShopCanvas.SetActive(false);
    }
}
