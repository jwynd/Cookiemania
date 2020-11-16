using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsTabController : MonoBehaviour
{
    [SerializeField]
    protected GameObject homeTab = null;
    [SerializeField]
    protected GameObject canvasPrefab = null;

    protected Canvas controlledCanvas;
    protected GameObject controlledObj;

    protected void ReturnFunction()
    {
        controlledCanvas.enabled = false;
        homeTab.GetComponent<General_TabButton>().click();
    }

    private void Awake()
    {
        controlledObj = Instantiate(canvasPrefab);
        controlledCanvas = controlledObj.GetComponent<Canvas>();
        controlledCanvas.enabled = false;
    }

    private void Start()
    {
        controlledObj.transform.SetParent(SiteCanvas.Instance.transform);
    }

    private void OnEnable()
    {
        if (controlledCanvas)
            controlledCanvas.enabled = true;
    }

    private void OnDisable()
    {
        if (controlledCanvas)
            controlledCanvas.enabled = false;
    }
}
