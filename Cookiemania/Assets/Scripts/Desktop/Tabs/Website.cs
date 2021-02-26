using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Website : MonoBehaviour
{
    [SerializeField]
    protected GameObject websitePrefab = null;
    [SerializeField]
    protected CharacterPrefab charPrefab = null;
    protected GameObject websiteRef = null;
    protected Canvas websiteCanvas = null;
    protected WebsiteUI ui = null;

    private void Awake()
    {
        websiteRef = Instantiate(websitePrefab);
        //dont parent
        websiteCanvas = websiteRef.GetComponent<Canvas>();
        websiteCanvas.enabled = false;
        ui = websiteRef.GetComponent<WebsiteUI>();
    }

    private void Start()
    {
        websiteCanvas.transform.SetParent(SiteCanvas.Instance.transform);
        ui.SetUpFromCharPrefab(charPrefab);
        ui.AttachWeekListener();
    }

    private void OnEnable()
    {
        websiteCanvas.enabled = true;
        ui.AnimateWeather();
    }

    private void OnDisable()
    {
        if (websiteCanvas)
            websiteCanvas.enabled = false;
    }
}