using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailTabController : MonoBehaviour
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
        controlledCanvas.gameObject.SetActive(false);
        homeTab.GetComponent<General_TabButton>().Click();
    }

    private void Awake()
    {
        controlledObj = Instantiate(canvasPrefab);
        controlledCanvas = controlledObj.GetComponent<Canvas>();
        controlledCanvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        controlledObj.transform.SetParent(SiteCanvas.Instance.transform);
        EventManager.Instance.SetEmailReference(
                controlledObj.GetComponent<EmailController>());
    }

    private void OnEnable()
    {
        if (!controlledCanvas)
            return;
        controlledCanvas.gameObject.SetActive(true);
        controlledCanvas.enabled = true;
        controlledObj.GetComponent<EmailController>().ShowYourself();

    }

    private void OnDisable()
    {
        if (!controlledCanvas)
            return;
        controlledCanvas.gameObject.SetActive(false);
        controlledCanvas.enabled = false;

    }
}
