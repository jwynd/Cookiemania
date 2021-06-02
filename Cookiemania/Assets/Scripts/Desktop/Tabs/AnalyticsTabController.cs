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
        controlledCanvas.gameObject.SetActive(false);
        controlledCanvas.enabled = false;
        homeTab.GetComponent<General_TabButton>().Click();
    }

    private void Awake()
    {
        controlledObj = Instantiate(canvasPrefab);
        controlledCanvas = controlledObj.GetComponent<Canvas>();
        controlledCanvas.gameObject.SetActive(false);
        controlledCanvas.enabled = false;
    }

    private void Start()
    {
        controlledObj.transform.SetParent(SiteCanvas.Instance.transform);
    }

    private void OnEnable()
    {
        if (!controlledCanvas)
            return;
        controlledCanvas.gameObject.SetActive(true);
        controlledCanvas.enabled = true;
    }

    private void OnDisable()
    {
        if (!controlledCanvas)
            return;
        controlledCanvas.gameObject.SetActive(false);
        controlledCanvas.enabled = false;
    }
}
