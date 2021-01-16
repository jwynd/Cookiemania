using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Website : MonoBehaviour
{
    [SerializeField]
    protected GameObject websiteCustomizationPrefab = null;
    [SerializeField]
    protected GameObject websitePrefab = null;

    protected bool CustomizedOnce 
    { 
        get 
        { 
            if (CustomizationUI.Instance)
                return CustomizationUI.Instance.Initialized;
            return true;
        } 
    }
    protected GameObject websiteRef = null;
    protected GameObject websiteCustomizationRef = null;
    protected Canvas websiteCanvas = null;
    protected Canvas customizationCanvas = null;

    private void Awake()
    {
        websiteRef = Instantiate(websitePrefab);
        websiteCustomizationRef = Instantiate(websiteCustomizationPrefab);
        //dont parent
        websiteCanvas = websiteRef.GetComponent<Canvas>();
        customizationCanvas = websiteCustomizationRef.GetComponent<Canvas>();
        customizationCanvas.enabled = false;
        websiteCanvas.enabled = false;
    }

    private void Start()
    {
        websiteCanvas.transform.SetParent(SiteCanvas.Instance.transform);
    }

    private void OnEnable()
    {
        if (!CustomizedOnce)
        {
            CustomizationUI.Instance.SetDisableCanvases(new List<GameObject>
                { websiteRef });
            CustomizationUI.Instance.CustomizationStart(new System.Action 
                (websiteRef.GetComponent<WebsiteUI>().SetUpFromCustomization));
        }
        else
        {
            //go to website instead
            websiteCanvas.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (customizationCanvas)
            customizationCanvas.enabled = false;
        if (websiteCanvas)
            websiteCanvas.enabled = false;
    }
}
