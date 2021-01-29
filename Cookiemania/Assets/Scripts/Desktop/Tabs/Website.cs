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

    private void Awake()
    {
        websiteRef = Instantiate(websitePrefab);
        //dont parent
        websiteCanvas = websiteRef.GetComponent<Canvas>();
        websiteCanvas.enabled = false;
    }

    private void Start()
    {
        websiteCanvas.transform.SetParent(SiteCanvas.Instance.transform);
        websiteRef.GetComponent<WebsiteUI>().SetUpFromCharPrefab(charPrefab);
    }

    private void OnEnable()
    {
        websiteCanvas.enabled = true;
    }

    private void OnDisable()
    {
        if (websiteCanvas)
            websiteCanvas.enabled = false;
    }
}